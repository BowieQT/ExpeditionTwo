using DieselExileTools.Common;
using DieselExileTools.Common.Structs;
using DieselExileTools.ExileCore2;
using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.Elements;
using ExileCore2.PoEMemory.FilesInMemory;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.PoEMemory.Models;
using ExileCore2.Shared.Cache;
using ExileCore2.Shared.Enums;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Xml.Linq;
using static DieselExileTools.DXT;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using RectangleF = ExileCore2.Shared.RectangleF;
using SColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;


namespace ExpeditionTwo;

public class VerisiumRemnant {

    public bool HasLabel { get; } = false;
    public Entity Entity { get; } 
    public Expedition2EncounterLabel? Expedition2EncounterLabel { get; }
    public Expedition2EncounterData? RuneData { get; }
    public RemnantStatus Status { get; private set; }

    public VerisiumRemnant(LabelOnGround labelOnGround) {
        HasLabel = true;
        Entity = labelOnGround.ItemOnGround;
        Expedition2EncounterLabel = labelOnGround.Label.AsObject<Expedition2EncounterLabel>();
        RuneData = Expedition2EncounterLabel?.Data;
    }
    public VerisiumRemnant(Entity entity) {
        HasLabel = false;
        Entity = entity;
        if (entity?.HashComponents.GetValueOrDefault((ushort)0x87B2) is not 0 and { } dataAddr) 
            RuneData = RemoteMemoryObject.GetObjectStatic<Expedition2EncounterData>(dataAddr);
    }

    public void Update() {
        Status = GetStatus();

    }
    private RemnantStatus GetStatus() {

        var status = new RemnantStatus();
        var states = Entity?.GetComponent<StateMachine>()?.States;
        if (states == null) return status;
        var placed_epk = false;
        foreach (var s in states) {
            switch (s.Name) {
                case "activated":
                    int val = (int)s.Value;
                    if (val == 3) status.IsPending = true; // Pending in Started Blast chain 
                    else if (val == 5) status.IsActive = true; // ?? actively spawning monsters 
                    else if (val == 6) status.IsUnclaimedReward = true;
                    else if (val == 7) status.IsCompleted = true;
                    else if (val == 8) status.IsNotTagged = true;
                    break;
                case "is_rerolled":
                    if ((int)s.Value == 1) status.IsRerolled = true;
                    break;
                case "in_placing_range":
                    if ((int)s.Value == 1) status.InPlacementRange = true;
                    break;
                case "placed_epk":
                    if ((int)s.Value == 1) placed_epk = true;
                    break;
            }
            if (placed_epk && !status.InPlacementRange) status.IsMarkedForDetonation = true;
        }
        var socketState = states.FirstOrDefault(x => x.Name == "sockets");
        if (socketState != null) {
            status.RuneCount = (int)socketState.Value;
        }

        return status;
    }
}

public enum ExplodableTypes {
    Unknown,
    MonsterRarity,
    CurrencyChest,
    UniquesChest,
    WeaponChest,
    ArmorChest,
    MagicChest,

    EliteRemnant,
}
public class ExplodableDefinition {
    public string LabelPath { get; init; }
    public string EntityPath { get; init; }
    public string AOBPath { get; set; }
    public ExplodableTypes Type { get; set; }
}
public class Explodable {
    public bool HasLabel { get; } = false;
    public LabelOnGround? LabelOnGround { get; }
    public Entity Entity { get; }
    public ExplodableTypes Type { get; } = ExplodableTypes.Unknown;

    public Explodable(LabelOnGround labelOnGround, Entity entity, ExplodableTypes type) {
        HasLabel = true;
        LabelOnGround = labelOnGround;
        Entity = entity;
        Type = type;
    }
    public Explodable(Entity entity, ExplodableTypes type) {
        HasLabel = false;
        Entity = entity;
        Type = type;
    }

}
public class RecipePrice {
    public bool Overridden { get; } = false;
    public bool NinjaPriced { get; } = false;
    public double PriceInExalts { get; } = 0;
    public double PriceInDivines { get; } = 0;
    public SColor Color { get; } = SColor.White;
    public RecipePrice(double valueExalts, double divineValue, bool overridden, bool ninjaPriced, SColor color) {
        PriceInExalts = valueExalts;
        PriceInDivines = divineValue;
        Overridden = overridden;
        NinjaPriced = ninjaPriced;
        Color = color;
    }
    public RecipePrice(SColor color) { Color = color; }

    public string Format(int padding = 0, bool padLeft = true) {

        var displayValue = PriceInDivines > 0 ? Math.Max(PriceInDivines, 0.01) : 0;
        var text = displayValue == 0 ? "0?" : displayValue.ToString("#.##");

        if (Overridden) text = "~" + text;

        return padLeft ? text.PadLeft(padding) : text.PadRight(padding);

    }
}
public class RemnantStatus {
    public bool IsUnclaimedReward { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public bool IsPending { get; set; } = false;    
    public bool IsNotTagged { get; set; } = false;
    public bool IsRerolled { get; set; } = false;
    public bool InPlacementRange { get; set; } = false;
    public bool IsMarkedForDetonation { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public int RuneCount { get; set; } = 0;
}

public class Plugin : BaseSettingsPlugin<Settings> {

    private readonly TimeCache<List<VerisiumRemnant>> _verisiumRemnants;
    private readonly TimeCache<Dictionary<Expedition2Recipe, RecipePrice>> _recipePrices;
    private readonly TimeCache<List<Explodable>> _explodables;
    private readonly Dictionary<string, ExplodableDefinition> _labelLookups = new() {
        { "Metadata/Terrain/Gallows/Leagues/Expedition/Logbook_Peninsula/Objects/GoblinRelic", new() { Type = ExplodableTypes.MonsterRarity } },
        { "Metadata/Terrain/Gallows/Leagues/Expedition/Logbook_Wastes/Objects/Sulphite", new() { Type = ExplodableTypes.MonsterRarity } }
    };
    private readonly Dictionary<string, List<ExplodableDefinition>> _entityLookups = new() {
        { "Metadata/MiscellaneousObjects/Expedition/ExpeditionMarker", new List<ExplodableDefinition> {
            new() { AOBPath = "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestCurrency.ao", Type = ExplodableTypes.CurrencyChest },
            new() { AOBPath = "Metadata/Terrain/Doodads/Leagues/Expedition/chestmarker2.ao", Type = ExplodableTypes.MagicChest },
            new() { AOBPath = "Metadata/Terrain/Doodads/Leagues/Expedition/ChestUniques.ao", Type = ExplodableTypes.UniquesChest },
            new() { AOBPath = "Metadata/Terrain/Doodads/Leagues/Expedition/elitemarker", Type = ExplodableTypes.EliteRemnant },
            new() { AOBPath = "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestWeapon.ao", Type = ExplodableTypes.WeaponChest },
            new() { AOBPath = "Metadata/Terrain/Doodads/Leagues/Expedition/ChestMarkers/ChestArmor.ao", Type = ExplodableTypes.ArmorChest }
        }}
    };
    private BaseItemType _divineOrb;
    private BaseItemType DivineOrb {
        get {
            _divineOrb ??= GameController.Files.BaseItemTypes.Contents.FirstOrDefault(x => x.Value.BaseName == "Divine Orb").Value;
            return _divineOrb ?? throw new InvalidOperationException($"Could not find item: Divine Orb");
        }
    }
    private double DivineOrbValue = 0;

    //~~| Modules |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private UserInterface _userInterface;
    private UserInterface UserInterface => _userInterface ??= new UserInterface(this);

    //--| Initialise / construct |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public Plugin() {
        _explodables = new TimeCache<List<Explodable>>(() => {
            var results = new List<Explodable>();

            foreach (var label in GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible) {
                var path = label?.ItemOnGround?.Path;
                if (path == null) continue;
                if (_labelLookups.TryGetValue(path, out var def)) {
                    results.Add(new Explodable(label, label.ItemOnGround, def.Type));
                }
            }
            foreach (var entity in GameController.Entities) {
                if (string.IsNullOrEmpty(entity?.Path)) continue;

                if (_entityLookups.TryGetValue(entity.Path, out var defList)) {
                    foreach (var def in defList) {
                        if (!string.IsNullOrEmpty(def.AOBPath)) {
                            if (!entity.TryGetComponent<Animated>(out var animated)) continue;
                            if (animated.BaseAnimatedObjectEntity?.Path?.StartsWith(def.AOBPath, StringComparison.Ordinal) != true) continue;
                        }
                        results.Add(new Explodable(entity, def.Type));
                        break;
                    }
                }
            }
            return results;
        }, 1000);
        _verisiumRemnants = new TimeCache<List<VerisiumRemnant>>(() => {
            var labelsOnGround = GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible
                .Where(x => x?.ItemOnGround?.Metadata?.StartsWith("Metadata/MiscellaneousObjects/Expedition2/Expedition2Encounter", StringComparison.Ordinal) == true)
                .ToList();

            var entitiesWithLabels = labelsOnGround.Select(x => x.ItemOnGround).ToHashSet();

            var results = labelsOnGround.Select(x => new VerisiumRemnant(x)).ToList();

            var entitiesWithoutLabels = GameController.EntityListWrapper.Entities
                .Where(e => e.Metadata?.StartsWith("Metadata/MiscellaneousObjects/Expedition2/Expedition2Encounter", StringComparison.Ordinal) == true)
                .Where(e => !entitiesWithLabels.Contains(e));

            foreach (var e in entitiesWithoutLabels) results.Add(new VerisiumRemnant(e));

            return results;
        }, 1000);
        _recipePrices = new TimeCache<Dictionary<Expedition2Recipe, RecipePrice>>(() => {
            var getCurrencyValue = GameController.PluginBridge.GetMethod<Func<BaseItemType, double>>("NinjaPrice.GetBaseItemTypeValue") ?? (_ => 0);
            DivineOrbValue = getCurrencyValue(DivineOrb);

            var overrideDict = Settings.PriceOverrides.ToDictionary(po => po.Recipee.Value);

            return GameController.Files.Expedition2Recipes.EntriesList.ToDictionary(
                recipe => recipe,
                recipe => {
                    double priceInExalts;
                    double priceInDivines;
                    bool overridden = false;
                    bool ninjaPriced = false;

                    // 3. Use the lookup dictionary
                    if (overrideDict.TryGetValue(recipe.Id, out var over)) {
                        priceInExalts = over.Value * DivineOrbValue;
                        priceInDivines = over.Value;
                        overridden = true;
                    }
                    else {
                        var rewardVal = recipe.Reward == null ? 0 : getCurrencyValue(recipe.Reward);
                        priceInExalts = rewardVal * recipe.RewardCount;
                        priceInDivines = DivineOrbValue > 0 ? priceInExalts / DivineOrbValue : 0;
                        ninjaPriced = recipe.Reward != null;
                    }

                    SColor color = priceInDivines >= Settings.ValueVeryGood ? Settings.ValueVeryGoodText_Color :
                                   priceInDivines >= Settings.ValueGood ? Settings.ValueGoodText_Color :
                                   Settings.ValueText_Color;

                    return new RecipePrice(priceInExalts, priceInDivines, overridden, ninjaPriced, color);
                }
            );
        }, 1000);
    }

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
        //ImGui.Text($"Divine Orb: {_divineValue}");
        UserInterface.Draw();
    }
    //--| Tick |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Tick() {
        Settings.KnownRecipes.UnionWith(_recipePrices.Value.Keys.Select(x => x.Id));
    }
    //--| Render |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void Render() {
        DBug.Render();

        RenderRemnants();
        RenderExplodables();
    }
    //--| Explodables |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void RenderExplodables() {
        foreach (var explodable in _explodables.Value) {
            if (explodable == null) continue;

            var states = explodable.Entity?.GetComponent<StateMachine>()?.States;

            var inPlacementRange = states?.Any(s => s.Name == "in_placing_range" && (int)s.Value == 1) ?? false;
            var placed_epk = states?.Any(s => s.Name == "placed_epk" && (int)s.Value == 1) ?? false;
            var markedForExplosion = !inPlacementRange && placed_epk;

            if (inPlacementRange || markedForExplosion) {
                var color = inPlacementRange ? Settings.InPlacementRange_Color : Settings.MarkedForExplosion_Color;
                if (explodable.HasLabel) {
                    var labelRect = explodable.LabelOnGround.Label.GetClientRect();
                    Graphics.DrawFrame(labelRect, color, 0, 5, 0);
                }

                var rawPos = Graphics.GridToMap(explodable.Entity.GridPos, explodable.Entity.GridPos);
                var mapPos = new Vector2((int)rawPos.X, (int)rawPos.Y);

                Graphics.DrawCircle(mapPos, 12, SColor.Black, 4, 16);
                Graphics.DrawCircle(mapPos, 12, color, 2, 16);
            }
        }
        //GameController.InspectObject(_relics, "_relics");
    }
    //--| Remnants |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void DrawRemnantOnMinimap( VerisiumRemnant verisiumRemnant, RecipePrice bestRecipePrice ) {
        var remnantTransferRunePositions = verisiumRemnant.RuneData?.PassedOnRunePositions?.OrderBy(x => x).ToList() ?? [];
        var selectedRecipe = verisiumRemnant.RuneData?.SelectedRecipe;
        var selectedRecipeRuneCount = selectedRecipe?.Runes.Count ?? 0;

        var coloredText = new ColoredText(Graphics);

        if (selectedRecipeRuneCount > 0) {
            coloredText.Add($"[{selectedRecipeRuneCount}]", Settings.LabelText_Color);
            var selectedRecipePrice = GetRecipePriceOrDefault(selectedRecipe);
            coloredText.Add($"{selectedRecipePrice.Format()}", selectedRecipePrice.Color);
        }
        else {
            coloredText.Add($"[{verisiumRemnant.RuneData?.RuneCount}]", Settings.LabelText_Color);
            coloredText.Add($"{bestRecipePrice.Format()}", bestRecipePrice.Color);
        }

        if (Settings.MinimapRemnant_ShowTransferred && selectedRecipeRuneCount > 0 && remnantTransferRunePositions.Count > 0) {
            var transferredRunes = remnantTransferRunePositions
                .Select(pos => selectedRecipe.Runes.ElementAtOrDefault(pos))
                .Where(x => x != null)
                .OrderBy(x => x.Id)
                .ToList();

            if (transferredRunes.Count > 0) coloredText.Add(" [T]", Settings.LabelText_Color);

            for (int i = 0; i < transferredRunes.Count; i++) {
                var rune = transferredRunes[i];

                if (i > 0) coloredText.Add(",", Settings.LabelText_Color);

                coloredText.Add(rune.Id, GetRuneColor(rune.Id));
            }
        }

        var gridCenter = Graphics.GridToMap(verisiumRemnant.Entity.GridPos, verisiumRemnant.Entity.GridPos);
        var centeredPos = new SVector2(
            MathF.Round(gridCenter.X - (coloredText.Size.X / 2)),
            MathF.Round(gridCenter.Y - (coloredText.Size.Y / 2))
        );

        var padding = new DXTPadding(0, 0, 2, 3);
        var borderInset = new DXTPadding(1);
        var coloredTextOptions = new ColoredTextOptions { BgColor = Settings.BG_Color, Padding = padding };
        if (verisiumRemnant.Status.InPlacementRange) {
            coloredTextOptions.Padding = padding.Expand(2,2);
            coloredTextOptions.BorderThickness = 2;
            coloredTextOptions.BorderColor = Settings.InPlacementRange_Color;
        }
        else if (verisiumRemnant.Status.IsMarkedForDetonation) {
            coloredTextOptions.Padding = padding.Expand(2, 2);
            coloredTextOptions.BorderThickness = 2;
            coloredTextOptions.BorderColor = Settings.MarkedForExplosion_Color;
        }
        else if (verisiumRemnant.Status.IsRerolled) {
            coloredTextOptions.Padding = padding.Expand(2, 2);
            coloredTextOptions.BorderThickness = 2;
            coloredTextOptions.BorderColor = Settings.RerolledBorder_Color;
        }
        else if (selectedRecipeRuneCount > 0) {
            coloredTextOptions.Padding = padding.Expand(2, 2);
            coloredTextOptions.BorderThickness = 2;
            coloredTextOptions.BorderColor = Settings.SelectedBorder_Color;
        }

        coloredText.Draw(centeredPos, coloredTextOptions);
    }
    private void DrawUnclaimedReward(VerisiumRemnant verisiumRemnant) {
        if (Settings.DisplayUnclaimedRewardRemnants) {
            var coloredText = new ColoredText(Graphics);
            coloredText.Add("Unclaimed Reward!!", Settings.LabelText_Color);

            var gridCenter = Graphics.GridToMap(verisiumRemnant.Entity.GridPos, verisiumRemnant.Entity.GridPos);
            var centeredPos = new SVector2(
                gridCenter.X - (coloredText.Size.X / 2),
                gridCenter.Y - (coloredText.Size.Y / 2)
            );

            var coloredTextOptions = new ColoredTextOptions { BgColor = Settings.BG_Color, Padding = new DXTPadding(2, 0, 2, 3) };
            coloredText.Draw(centeredPos, coloredTextOptions);
        }
    }
    private void DrawRemnantInGame(VerisiumRemnant verisiumRemnant, List<(Expedition2Recipe, RecipePrice)> remnantRecipes) {
        if (verisiumRemnant.Expedition2EncounterLabel == null) return;

        var coloredTextOptions = new ColoredTextOptions { BgColor = Settings.BG_Color, Padding = new DXTPadding(0, 0, 2, 3) };
        var remnantRect = verisiumRemnant.Expedition2EncounterLabel.GetClientRect();
        var labelStartPos = remnantRect.BottomLeft;
        labelStartPos += new Vector2(Settings.InGameRemnant_RenderOffset.X, Settings.InGameRemnant_RenderOffset.Y);
        var currentY = labelStartPos.Y;

        if (verisiumRemnant.Status.InPlacementRange) Graphics.DrawFrame(remnantRect, Settings.InPlacementRange_Color, 0, 5, 0);
        if (verisiumRemnant.Status.IsMarkedForDetonation) Graphics.DrawFrame(remnantRect, Settings.MarkedForExplosion_Color, 0, 5, 0);

        // Draw recipe list
        var shownRemnantRecipes = remnantRecipes.Take(Settings.InGameRemnant_MaxItemsToShow).ToList();
        foreach (var (recipe, recipePrice) in shownRemnantRecipes) {
            var coloredText = new ColoredText(Graphics);

            coloredText.Add(recipePrice.Format(7), recipePrice.Color);
            coloredText.Add($" {(string.IsNullOrWhiteSpace(recipe.Description) ? recipe.Reward?.BaseName : recipe.Description)}", recipePrice.Color);
            coloredText.Add($"  x{recipe.RewardCount}", recipePrice.Color);
            var size = coloredText.Draw(labelStartPos with { Y = currentY }, coloredTextOptions);
            currentY += size.Y;
        }
        currentY += shownRemnantRecipes.Count > 0 ? 2 : 0;

        // draw all possible transferred runes
        var remnantTransferRunePositions = verisiumRemnant.RuneData?.PassedOnRunePositions?.OrderBy(x => x).ToList() ?? new List<int>();
        if (remnantTransferRunePositions.Count > 0 && !verisiumRemnant.Status.IsRerolled) {
            foreach (var position in remnantTransferRunePositions) {
                var runes = remnantRecipes
                    .Select(item => item.Item1.Runes.ElementAtOrDefault(position))
                    .Where(x => x != null)
                    .Distinct()
                    .OrderBy(r => r.Id)
                    .ToList();

                var coloredText = new ColoredText(Graphics);
                coloredText.Add($"Rune {position + 1} possible Transfers: ", Settings.LabelText_Color);
                for (int i = 0; i < runes.Count; i++) {
                    if (i > 0) coloredText.Add(",", Settings.LabelText_Color);
                    coloredText.Add(runes[i].Id, GetRuneColor(runes[i].Id));
                }

                var size = coloredText.Draw(labelStartPos with { Y = currentY }, coloredTextOptions);
                currentY += size.Y;
            }
            currentY += 2;
        }
        // Draw selected / Locked Recipe
        var selectedRecipe = verisiumRemnant.RuneData?.SelectedRecipe;
        var selectedRecipeRuneCount = selectedRecipe?.Runes.Count ?? 0;

        if (selectedRecipeRuneCount > 0) {
            var selectedRecipePrice = GetRecipePriceOrDefault(selectedRecipe);
            var coloredText = new ColoredText(Graphics);

            if (verisiumRemnant.Status.IsRerolled)
                coloredText.Add($"Rolled Recipe:", Settings.LabelText_Color);
            else
                coloredText.Add($"Selected Recipe:", Settings.LabelText_Color);
            
            coloredText.Add($"{selectedRecipePrice.Format()}", selectedRecipePrice.Color);

            coloredText.Add($" {(string.IsNullOrWhiteSpace(selectedRecipe.Description) ? selectedRecipe.Reward?.BaseName : selectedRecipe.Description)}", selectedRecipePrice.Color);
            coloredText.Add($" x{selectedRecipe.RewardCount}", selectedRecipePrice.Color);

            coloredText.Add($" #Runes:{selectedRecipeRuneCount}", Settings.LabelText_Color);

            var transferredRunes = remnantTransferRunePositions
                .Select(pos => selectedRecipe.Runes.ElementAtOrDefault(pos))
                .Where(x => x != null)
                .OrderBy(x => x.Id)
                .ToList();

            if (transferredRunes.Count > 0) {
                coloredText.Add($" Transfers:", Settings.LabelText_Color);
                for (int i = 0; i < transferredRunes.Count; i++) {
                    var rune = transferredRunes[i];

                    if (i > 0) coloredText.Add(",", Settings.LabelText_Color);
                    coloredText.Add(rune.Id, GetRuneColor(rune.Id));
                }
            }
            var size = coloredText.Draw(labelStartPos with { Y = currentY }, coloredTextOptions);
            currentY += size.Y;
        }
    }

    private void RenderRemnants() {

        var verisiumRemnants = _verisiumRemnants.Value;
        var areaLevel = GameController.IngameState.Data.CurrentAreaLevel;
        var allRecipes = GameController.Files.Expedition2Recipes.EntriesList;
        var expedition2RunesWeights = GameController.Files.Expedition2RunesWeights.EntriesList;

        // Render Remnants
        foreach (var verisiumRemnant in verisiumRemnants) {
            if (verisiumRemnant == null) continue;

            verisiumRemnant.Update();

            if (verisiumRemnant.Status.IsNotTagged) continue;
            if (verisiumRemnant.Status.IsCompleted && !Settings.DisplayCompletedRemnants) continue;
            if (verisiumRemnant.Status.IsPending && !Settings.DisplayPendingRemnants) continue;
            if (verisiumRemnant.Status.IsActive && !Settings.DisplayActiveRemnants) continue;
            if (verisiumRemnant.Status.IsUnclaimedReward) {
                DrawUnclaimedReward(verisiumRemnant);
                continue;
            }

            // Get Remnants Recipes
            var remnantRecipes = GetRemanantRecipes(expedition2RunesWeights, areaLevel, allRecipes, verisiumRemnant.RuneData);

            if (remnantRecipes.Count > 0) {
                var (bestRecipe, bestRecipeePrice) = remnantRecipes[0];

                if (Settings.MinimapRemnant_Show) DrawRemnantOnMinimap(verisiumRemnant, bestRecipeePrice);

                if (Settings.InGameRemnant_Show) DrawRemnantInGame(verisiumRemnant, remnantRecipes);
            }

        }

        // Expedition2Window Prices
        if (GameController.IngameState.IngameUi.Expedition2Window is { IsVisible: true } expedition2Window) {
            var coloredTextOptions = new ColoredTextOptions { BgColor = Settings.BG_Color, Padding = new DXTPadding(0, 0, 2, 3) };
            var windowRect = expedition2Window.GetClientRectCache;
            var bookRect = expedition2Window.GetChildFromIndices(3)?.GetClientRectCache;
            var bounds = (bookRect?.Width > 0) ? bookRect.Value : windowRect;

            if (!IsDrawableRect(windowRect)) return;

            var options = expedition2Window.Options
                .Where(x => x is { IsValid: true, IsVisible: true, IsVisibleLocal: true, Recipe: not null })
                .Select(x => (x, GetRecipePriceOrDefault(x.Recipe)))
                .OrderByDescending(x => x.Item2.PriceInExalts)
                .ToList();
            foreach (var (option, recipePrice) in options) {
                var optionRect = option.GetClientRectCache;
                if (!IsDrawableRect(optionRect) ||
                    !bounds.Intersects(optionRect) ||
                    !bounds.Contains(optionRect.TopLeft)) {
                    continue;
                }

                var coloredText = new ColoredText(Graphics);

                coloredText.Add($"{recipePrice.Format(5, false)}", recipePrice.Color);
                var position = ClampTextPosition(optionRect.TopRight, coloredText.Size, bounds);
                coloredText.Draw(position, coloredTextOptions);
            }
        }
    }

    private RecipePrice GetRecipePriceOrDefault(Expedition2Recipe recipe) {
        return recipe != null && _recipePrices.Value.TryGetValue(recipe, out var recipePrice) ? recipePrice : new RecipePrice(Settings.ValueText_Color);
    }
    private List<(Expedition2Recipe recipe, RecipePrice recipePrice)> GetRemanantRecipes(List<Expedition2RunesWeight> expedition2RunesWeights, int areaLevel, List<Expedition2Recipe> allRecipes, Expedition2EncounterData? data) {
        var allowedRuneCounts = expedition2RunesWeights.Where(x => x.RuneSlot - 1 == data?.FixedRunePosition)
            .Where(runeWeight => runeWeight.Rune?.Equals(data?.FixedRune) == true)
            .Where(runeWeight => runeWeight.Level <= areaLevel)
            .Select(runeWeight => runeWeight.SlotCount)
            .ToHashSet();

        var recipes = allRecipes.Where(recipe => recipe.RuneCountRequired <= data?.RuneCount)
            .Where(recipe => allowedRuneCounts.Contains(recipe.RuneCountRequired))
            .Where(recipe => recipe.MinLevelReq <= areaLevel && recipe.MaxLevelReq >= areaLevel)
            .Where(recipe => recipe.Runes.ElementAtOrDefault(data?.FixedRunePosition ?? 0)?.Equals(data?.FixedRune) == true)
            .Select(recipe => (recipe, recipePrice: GetRecipePriceOrDefault(recipe)))
            .OrderByDescending(recipe => recipe.recipePrice.PriceInExalts)
            .ToList();

        return recipes;
    }
    private static bool IsDrawableRect(RectangleF rect) {
        return rect.Width > 1 && rect.Height > 1;
    }
    private static Vector2 ClampTextPosition(Vector2 position, Vector2 textSize, RectangleF bounds) {
        var maxX = Math.Max(bounds.Left, bounds.Right - textSize.X);
        var maxY = Math.Max(bounds.Top, bounds.Bottom - textSize.Y);
        return new Vector2(Math.Clamp(position.X, bounds.Left, maxX), Math.Clamp(position.Y, bounds.Top, maxY));
    }
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
        { "Rebirth", PurpleRuneColor }, // was blut but i think its  as good as a purple

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
       

}
