using DieselExileTools.Common;
using DieselExileTools.ExileCore2;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using Newtonsoft.Json;
using SColor = System.Drawing.Color;
using SVector4 = System.Numerics.Vector4;
using SVector2 = System.Numerics.Vector2;


namespace ExpeditionTwo;

public class PriceOverride {
    public ListNode Recipee = new ListNode();
    public int Value = 0;
}


public sealed class Settings : ISettings {
    public ToggleNode Enable { get; set; } = new(true);

    public DXTSettings DXT { get; set; } = new();
    public bool Debug = false;

    public bool RunesHeaderOpen = true;
    public bool PriceOverridesHeaderOpen = true;

    public HashSet<string> KnownRecipes = [];

    public List<PriceOverride> PriceOverrides = [];

    public bool DisplayUnclaimedRewardRemnants = true;
    public bool DisplayCompletedRemnants = false;


    public int InGameRemnant_MaxItemsToShow = 0;
    public int InGameRemnant_MinimumValueToShow = 0;
    public bool InGameRemnant_ShowTransferred = true;
    public SVector2 InGameRemnant_RenderOffset = new(0, 5);

    public bool MinimapRemnant_Show = true;
    public bool MinimapRemnant_ShowTransferred = true;

    public SColor BG_Color = SColor.FromArgb(200, 0, 0, 0);
    public SColor ExplosiveHover_Color = SColor.FromArgb(200, 0, 0);

    public SColor RerolledBorder_Color = SColor.FromArgb(255, 82, 82);

    public SColor LabelText_Color = SColor.FromArgb(207, 216, 220);
    public SColor ValueText_Color = SColor.FromArgb(255, 61, 0);
    public SColor ValueGoodText_Color = SColor.FromArgb(255, 214, 0);
    public SColor ValueVeryGoodText_Color = SColor.FromArgb(118, 255, 3);

    public int ValueGood = 50;
    public int ValueVeryGood = 500;







}
