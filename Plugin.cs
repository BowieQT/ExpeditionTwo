using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using DieselExileTools.ExileCore2;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.Elements;
using ExileCore2.PoEMemory.FilesInMemory;
using ExileCore2.PoEMemory.Models;
using ExileCore2.Shared.Cache;
using Newtonsoft.Json.Linq;
using System.Numerics;
using static DieselExileTools.DXT;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RectangleF = ExileCore2.Shared.RectangleF;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;


namespace ExpeditionTwo;

public class VerisiumRemnant {
    public LabelOnGround LabelOnGround { get; }
    public Expedition2EncounterLabel Expedition2EncounterLabel { get; }

    public VerisiumRemnant(LabelOnGround labelOnGround, Expedition2EncounterLabel expedition2EncounterLabel) {
        LabelOnGround = labelOnGround;
        Expedition2EncounterLabel = expedition2EncounterLabel;
    }
}

public class RecipePrice {
    public bool Overridden { get; }
    public bool NinjaPriced { get; }
    public double Value { get; }
    public SColor Color { get; }
    public RecipePrice(double value, bool overridden, bool ninjaPriced, SColor color) {
        Value = value;
        Overridden = overridden;
        NinjaPriced = ninjaPriced;
        Color = color;
    }

}

public class Plugin : BaseSettingsPlugin<Settings> {

    private readonly TimeCache<List<VerisiumRemnant>> _verisiumRemnants;
    private readonly TimeCache<Dictionary<Expedition2Recipe, RecipePrice>> _recipePrices;

    //~~| Modules |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private UserInterface _userInterface;

    public Plugin() {
        _verisiumRemnants = new TimeCache<List<VerisiumRemnant>>(() =>
            GameController.EntityListWrapper.Entities
                .Any(x => x.Metadata.StartsWith("Metadata/MiscellaneousObjects/Expedition2/Expedition2Encounter", StringComparison.Ordinal))
                ? GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible
                    .Where(x => x?.ItemOnGround?.Metadata?.StartsWith("Metadata/MiscellaneousObjects/Expedition2/Expedition2Encounter", StringComparison.Ordinal) == true)
                    .Select(x => new VerisiumRemnant(x, x.Label.AsObject<Expedition2EncounterLabel>()))
                    .ToList()
                : [],
            1000);
        _recipePrices = new TimeCache<Dictionary<Expedition2Recipe, RecipePrice>>(() => {
            var getCurrencyValue = GameController.PluginBridge.GetMethod<Func<BaseItemType, double>>("NinjaPrice.GetBaseItemTypeValue") ?? (_ => 0);
            return GameController.Files.Expedition2Recipes.EntriesList.ToDictionary(recipe => recipe, recipe => {
                double val;
                bool overridden = false;
                bool ninjaPriced = false;

                if (Settings.PriceOverrides.FirstOrDefault(po => po.Recipee.Value == recipe.Id) is { } over) {
                    val = over.Value;
                    overridden = true;
                }
                else {
                    val = (recipe.Reward == null ? 0 : getCurrencyValue(recipe.Reward)) * recipe.RewardCount;
                    ninjaPriced = recipe.Reward != null;
                }

                SColor color = val >= Settings.ValueVeryGood ? Settings.ValueVeryGoodText_Color :
                               val >= Settings.ValueGood ? Settings.ValueGoodText_Color :
                               Settings.ValueText_Color;

                return new RecipePrice(val, overridden, ninjaPriced, color);
            });
        }, 1000);
    }

    private UserInterface UserInterface => _userInterface ??= new UserInterface(this);
    //--| Initialise |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override bool Initialise() {
        CanUseMultiThreading = true;
        Initialise_DXT();





        return base.Initialise();
    }
    private void Initialise_DXT() {
        //DBug.AdditionalTools.Add(new DXT.FloatingToolbar.Button {
        //    Label = "BGLog",
        //    Tooltip = DXT.Tooltip.BasicOptions("Toggle Background Action Logging"),
        //    SetChecked = (bool state) => { Settings.LogInBackground = state; },
        //    GetChecked = () => Settings.LogInBackground,
        //});


        DXT.Initialise(new DXT.Config
        {
            PluginName = Name,
            PluginDirectory = DirectoryFullName,
            GameController = GameController,
            Graphics = Graphics,
            Settings = Settings.DXT,
        });
        DBug.LogHeader = (width, height) => {
            //DXT.Button.Draw($"{Name}Friendly", ref Settings.DebugFriendlyIcon, new DXT.Button.Options { Label = "Friendly", Width = 80, Height = 22 }); ImGui.SameLine();
        };
    }

    //--| Draw Settings |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void DrawSettings() {
        UserInterface.Draw();
    }
    //--| Tick |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Tick() {
        Settings.KnownRecipes.UnionWith(_recipePrices.Value.Keys.Select(x => x.Id));
    }
    //--| Render |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Render() {
        DBug.Render();

        // Render Remnants
        var areaLevel = GameController.IngameState.Data.CurrentAreaLevel;
        var allRecipes = GameController.Files.Expedition2Recipes.EntriesList.ToLookup(x => x.RuneCountRequired);
        var remnants = _verisiumRemnants.Value;

        if (remnants.Count > 0) {
            if (allRecipes.Count > 0) {
                var expedition2RunesWeights = GameController.Files.Expedition2RunesWeights.EntriesList;
                var minimapTextpadding = new DXTPadding(1);
                var minimapColoredTextOptions =  new ColoredTextOptions { BgColor = Settings.BG_Color };
                var inGameColoredTextOptions = new ColoredTextOptions { BgColor = Settings.BG_Color };
                 
                foreach (var remnant in remnants) {
                    var entity = remnant.LabelOnGround.ItemOnGround;
                    var states = entity?.GetComponent<StateMachine>()?.States;
                    if (entity == null) continue;

                    var remnantUnclaimedReward = states != null && states.Any(s => s.Name == "activated" && ((int)s.Value == 6));
                    var remnantComplete = states != null && states.Any(s => s.Name == "activated" && ((int)s.Value == 8));
                    var remnantRerolled = states != null && states.Any(s => s.Name == "is_rerolled" && ((int)s.Value == 1));
                    var remnantHovered = states != null && states.Any(s => s.Name == "in_placing_range" && ((int)s.Value == 1));

                    if (remnantComplete) {
                        if (!Settings.DisplayCompletedRemnants) continue;
                    }
                    else if (remnantUnclaimedReward) {
                        if (!Settings.DisplayUnclaimedRewardRemnants) continue;
                    }

                    var remanantRecipes = GetRemanantRecipes(expedition2RunesWeights, areaLevel, allRecipes, remnant.Expedition2EncounterLabel?.Data);
                    var allValidRecipes = remanantRecipes.Select(remanantRecipe => remanantRecipe.recipe).ToList();

                    if (Settings.InGameRemnant_MinimumValueToShow > 0) {
                        remanantRecipes = remanantRecipes.Where(remanantRecipe => remanantRecipe.recipePrice.Value >= Settings.InGameRemnant_MinimumValueToShow).ToList();
                    }
                    if (Settings.InGameRemnant_MaxItemsToShow > 0) {
                        remanantRecipes = remanantRecipes.Take(Settings.InGameRemnant_MaxItemsToShow).ToList();
                    }
                    var remnantRect = remnant.Expedition2EncounterLabel.GetClientRect();
                    var bottomLeft = remnantRect.BottomLeft;
                    bottomLeft += new Vector2(Settings.InGameRemnant_RenderOffset.X, Settings.InGameRemnant_RenderOffset.Y);
                    var y = bottomLeft.Y;

                    var first = true;

                    // Hover
                    if (remnantHovered) Graphics.DrawFrame(remnantRect, Settings.ExplosiveHover_Color, 0, 5, 0);

                    foreach (var (recipe, recipePrice) in remanantRecipes) {
                        // Minimap Text
                        if (first && Settings.MinimapRemnant_Show) {
                            var coloredMinimapText = GetMapColoredText(recipePrice, allValidRecipes, remnant.Expedition2EncounterLabel.Data, remnantRerolled);
                            coloredMinimapText.Draw(Graphics, Graphics.GridToMap(entity.GridPos, entity.GridPos), minimapColoredTextOptions);
                        }
                        // Ingame Text
                        var coloredText = new ColoredText();
                        coloredText.Add($"{(recipePrice.Overridden ? "~" : "")}{recipePrice.Value,7:F0}", recipePrice.Color);
                        coloredText.Add($" {(string.IsNullOrWhiteSpace(recipe.Description) ? recipe.Reward?.BaseName : recipe.Description)}", recipePrice.Color);
                        coloredText.Add($"  x{recipe.RewardCount}", recipePrice.Color);
                        var size = coloredText.Draw(Graphics, bottomLeft with { Y = y }, inGameColoredTextOptions);

                        y += size.Y;

                        first = false;
                    }

                    // Ingame Trandeferred Rune Text
                    y += 2;
                    if (Settings.InGameRemnant_ShowTransferred && remnant.Expedition2EncounterLabel.Data.PassedOnRunePositions is { Count: > 0 } positions) {
                        if (!remnantRerolled) {
                            foreach (var position in positions) {
                                var runes = allValidRecipes
                                    .Select(x => x.Runes.ElementAtOrDefault(position))
                                    .Where(x => x != null)
                                    .Distinct()
                                    .OrderBy(r => r.Id)
                                    .ToList();

                                var coloredText = new ColoredText();
                                coloredText.Add($"Rune: {position + 1} Transfers: ", Settings.LabelText_Color);

                                for (int i = 0; i < runes.Count; i++) {
                                    if (i > 0) coloredText.Add(", ", Settings.LabelText_Color);

                                    coloredText.Add(runes[i].Id, GetRuneColor(runes[i].Id));
                                }

                                var size = coloredText.Draw(Graphics, bottomLeft with { Y = y }, inGameColoredTextOptions);
                                y += size.Y;
                            }
                        }
                        else if(remnant.Expedition2EncounterLabel.Data?.SelectedRecipe?.Runes != null && remnant.Expedition2EncounterLabel.Data?.PassedOnRunePositions is { Count: > 0 } rpositions) {
                            var coloredText = new ColoredText();

                            var transferredRunes = rpositions
                                .Select(pos => remnant.Expedition2EncounterLabel.Data.SelectedRecipe.Runes.ElementAtOrDefault(pos))
                                .Where(x => x != null)
                                .OrderBy(x => x.Id)
                                .ToList();

                            if (transferredRunes.Count > 0) coloredText.Add(" [T]:", Settings.LabelText_Color);

                            for (int i = 0; i < transferredRunes.Count; i++) {
                                var rune = transferredRunes[i];

                                if (i > 0) coloredText.Add(", ", Settings.LabelText_Color);

                                coloredText.Add(rune.Id, GetRuneColor(rune.Id));
                            }

                            var size = coloredText.Draw(Graphics, bottomLeft with { Y = y }, inGameColoredTextOptions);
                            y += size.Y;
                        }
                    }
                }
                // Expedition2Window Prices
                if (GameController.IngameState.IngameUi.Expedition2Window is { IsVisible: true } expedition2Window) {
                    var windowRect = expedition2Window.GetClientRectCache;
                    var bookRect = expedition2Window.GetChildFromIndices(3)?.GetClientRectCache;
                    var bounds = (bookRect?.Width > 0) ? bookRect.Value : windowRect;

                    if (!IsDrawableRect(windowRect)) return; 

                    var options = expedition2Window.Options
                        .Where(x => x is { IsValid: true, IsVisible: true, IsVisibleLocal: true, Recipe: not null })
                        .Select(x => (x, GetPriceOrDefault(x.Recipe)))
                        .OrderByDescending(x => x.Item2.Value)
                        .ToList();
                    var first = true;
                    foreach (var (option, recipePrice) in options) {
                        var optionRect = option.GetClientRectCache;
                        if (!IsDrawableRect(optionRect) ||
                            !bounds.Intersects(optionRect) ||
                            !bounds.Contains(optionRect.TopLeft)) {
                            continue;
                        }

                        var coloredText = new ColoredText();

                        coloredText.Add($"{(recipePrice.Overridden ? "~" : "")}{recipePrice.Value,5:F0}", recipePrice.Color);
                        var position = ClampTextPosition(optionRect.TopRight, coloredText.Size, bounds);
                        coloredText.Draw(Graphics, position, inGameColoredTextOptions);
                        first = false;
                    }
                }


            }
        }



    }

    //--| Remnants |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static SColor PurpleRuneColor = SColor.FromArgb(230, 105, 251);
    private static SColor BlueRuneColor = SColor.FromArgb(130, 177, 255);
    private static SColor GoldRuneColor = SColor.FromArgb(255, 255, 109);
    private static SColor SilverRuneColor = SColor.FromArgb(255, 255, 255);
    private static Dictionary<string, SColor> _runeColors = new() {
        // High-Value / Multiplier Runes (Purple)
        { "Bond", PurpleRuneColor },
        { "Death", PurpleRuneColor },
        { "Life", PurpleRuneColor },
        { "Oath", PurpleRuneColor },
        { "Power", PurpleRuneColor },
        { "Soul", PurpleRuneColor },
        { "Time", PurpleRuneColor },

        // Propagation Runes (Gold/Yellow)
        { "Bait", GoldRuneColor },
        { "Opulent", GoldRuneColor },

        // Special Runes (Silver)
        { "Wisdom", SilverRuneColor },

        // Standard / Filler Runes (Blue)
        { "Adaptive", BlueRuneColor },
        { "Arcane", BlueRuneColor },
        { "Bloodletting", BlueRuneColor },
        { "Celestial", BlueRuneColor },
        { "Cold", BlueRuneColor },
        { "Cyclonic", BlueRuneColor },
        { "Earth", BlueRuneColor },
        { "Electrocuting", BlueRuneColor },
        { "Fire", BlueRuneColor },
        { "Gasp", BlueRuneColor },
        { "Lightning", BlueRuneColor },
        { "Momentum", BlueRuneColor },
        { "Moon", BlueRuneColor },
        { "Prismatic", BlueRuneColor },
        { "Protective", BlueRuneColor },
        { "Rage", BlueRuneColor },
        { "Rebirth", BlueRuneColor },
        { "Sky", BlueRuneColor },
        { "Stone", BlueRuneColor },
        { "Tempest", BlueRuneColor },
        { "Tidal", BlueRuneColor },
        { "Toxic", BlueRuneColor },
        { "Vision", BlueRuneColor },
        { "Ward", BlueRuneColor },
    };
    private SColor GetRuneColor(string runeId) {
        return _runeColors.TryGetValue(runeId, out var color) ? color : Settings.LabelText_Color;
    }

    private ColoredText GetMapColoredText(RecipePrice recipePrice, IReadOnlyCollection<Expedition2Recipe> recipes, Expedition2EncounterData data, bool rerolled) {
        var coloredText = new ColoredText();
        var defaultColor = Settings.LabelText_Color;

        coloredText.Add($"{(rerolled ? "*" : "")}Rune[{data.RuneCount}]:", defaultColor);
        coloredText.Add($"{(recipePrice.Overridden ? "~" : "")}{recipePrice.Value:F0}", recipePrice.Color);

        if (Settings.MinimapRemnant_ShowTransferred && data?.SelectedRecipe?.Runes != null && data?.PassedOnRunePositions is { Count: > 0 } positions) {

            var transferredRunes = positions
                .Select(pos => data.SelectedRecipe.Runes.ElementAtOrDefault(pos))
                .Where(x => x != null)
                .OrderBy(x => x.Id)
                .ToList();

            if (transferredRunes.Count > 0) coloredText.Add(" [T]:", defaultColor);

            for (int i = 0; i < transferredRunes.Count; i++) {
                var rune = transferredRunes[i];

                if (i > 0) coloredText.Add(", ", defaultColor);

                coloredText.Add(rune.Id, GetRuneColor(rune.Id));
            }
        }

        return coloredText;
    }
    
    private List<(Expedition2Recipe recipe, RecipePrice recipePrice)> GetRemanantRecipes(List<Expedition2RunesWeight> expedition2RunesWeights, int areaLevel, ILookup<int, Expedition2Recipe> allRecipes, Expedition2EncounterData data) {
        var allowedRuneCounts = expedition2RunesWeights.Where(x => x.RuneSlot - 1 == data?.FixedRunePosition)
            .Where(runeWeight => runeWeight.Rune?.Equals(data?.FixedRune) == true)
            .Where(runeWeight => runeWeight.Level <= areaLevel)
            .Select(runeWeight => runeWeight.SlotCount)
            .ToHashSet();

        var recipes = allRecipes.Where(x => x.Key <= data?.RuneCount)
            .SelectMany(x => x)
            .Where(x => allowedRuneCounts.Contains(x.RuneCountRequired))
            .Where(x => x.MinLevelReq <= areaLevel && x.MaxLevelReq >= areaLevel)
            .Where(x => x.Runes.ElementAtOrDefault(data?.FixedRunePosition ?? 0)?.Equals(data?.FixedRune) == true)
            .Select(x => (x, price: GetPriceOrDefault(x)))
            .OrderByDescending(x => x.price.Value)
            .ToList();

        return recipes;
    }
    private RecipePrice GetPriceOrDefault(Expedition2Recipe recipe) {
        return recipe != null && _recipePrices.Value.TryGetValue(recipe, out var recipePrice) ? recipePrice : new RecipePrice(0, false, false, Settings.ValueText_Color);
    }
    private static bool IsDrawableRect(RectangleF rect) {
        return rect.Width > 1 && rect.Height > 1;
    }
    private static Vector2 ClampTextPosition(Vector2 position, Vector2 textSize, RectangleF bounds) {
        var maxX = Math.Max(bounds.Left, bounds.Right - textSize.X);
        var maxY = Math.Max(bounds.Top, bounds.Bottom - textSize.Y);
        return new Vector2(Math.Clamp(position.X, bounds.Left, maxX), Math.Clamp(position.Y, bounds.Top, maxY));
    }

}
