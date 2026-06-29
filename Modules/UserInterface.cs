using DieselExileTools.Common;
using DieselExileTools.ExileCore2;
using ExileCore2;
using ExileCore2.Shared;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Security.Cryptography;
using SDColor = System.Drawing.Color;
using SVector2 = System.Numerics.Vector2;

namespace ExpeditionTwo;

public sealed class UserInterface : PluginModule {
    public UserInterface(Plugin plugin) : base(plugin) { }



    public void Draw() {
        DXT.Button.Draw("ShowDBug", ref Settings.DXT.DBug.ShowToolbar, new DXT.Button.Options {
            Label = "DBug",
            Width = 100,
            Height = 22,
        });

        //if (DXT.CollapsingHeader("Remnant Settings", ref Settings.RunesHeaderOpen)) {
        //ImGui.Indent();//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        ImGui.Text("* = Rune Rerolled");

        DXT.Checkbox.Draw("Show Remnants with Unclaimed Rewards", ref Settings.DisplayUnclaimedRewardRemnants);

        DXT.Checkbox.Draw("Show Completed Remnants", ref Settings.DisplayCompletedRemnants);

        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();

        DXT.Checkbox.Draw("Draw on minimap", ref Settings.MinimapRemnant_Show);

        DXT.Checkbox.Draw("Draw transfered Runes on minimap", ref Settings.MinimapRemnant_ShowTransferred);


        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();


        DXT.Checkbox.Draw("Draw Transfered runes in game", ref Settings.InGameRemnant_ShowTransferred);

        DXT.Slider.Draw("MinimumValueToShow", ref Settings.InGameRemnant_MinimumValueToShow, new DXT.Slider.Options { Max = 1000, Width = 100 });
        ImGui.SameLine();
        ImGui.Text($"Minimum Value recipes To Show");

        DXT.Slider.Draw("MaxItemsToShow", ref Settings.InGameRemnant_MaxItemsToShow, new DXT.Slider.Options { Max = 20, Width = 100 });
        ImGui.SameLine();
        ImGui.Text($"Max Recipees To Show");

        DXT.Slider.Draw("RenderOffsetX", ref Settings.InGameRemnant_RenderOffset.X, new DXT.Slider.Options { Max = 50, Width = 100 });
        ImGui.SameLine();
        DXT.Slider.Draw("RenderOffsetY", ref Settings.InGameRemnant_RenderOffset.Y, new DXT.Slider.Options { Max = 50, Width = 100 });
        ImGui.SameLine();
        ImGui.Text("In game render offfset");

        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();


        DXT.ColorSelect.Draw("BG_Color", "Text BG Color", ref Settings.BG_Color);
        ImGui.SameLine();
        ImGui.Text("Text BG Color");

        DXT.ColorSelect.Draw("ExplosiveHover_Color", "Explosive Hover Border Color", ref Settings.ExplosiveHover_Color);
        ImGui.SameLine();
        ImGui.Text("Explosive Hover Border Color");

        DXT.ColorSelect.Draw("LabelText_Color", "Label Color", ref Settings.LabelText_Color);
        ImGui.SameLine();
        ImGui.Text("Label Color");

        DXT.ColorSelect.Draw("ValueText_Color", "Value Color", ref Settings.ValueText_Color);
        ImGui.SameLine();
        ImGui.Text("Value Color");

        DXT.ColorSelect.Draw("ValueGoodText_Color", "Good Value Color", ref Settings.ValueGoodText_Color);
        ImGui.SameLine();
        DXT.Slider.Draw("ValueGood", ref Settings.ValueGood, new DXT.Slider.Options { Max = 5000, Width = 100 });
        ImGui.SameLine();
        ImGui.Text("Good Value");

        DXT.ColorSelect.Draw("ValueVeryGoodText_Color", "Very Good Value Color", ref Settings.ValueVeryGoodText_Color);
        ImGui.SameLine();
        DXT.Slider.Draw("ValueVeryGood", ref Settings.ValueVeryGood, new DXT.Slider.Options { Max = 5000, Width = 100 });
        ImGui.SameLine();
        ImGui.Text("Very Good Value");

        //if (DXT.Button.Draw($"logButton", new DXT.Button.Options { Label = "Dump", Width = 100 })) {
        //    var allRunes = GameController.Files.Expedition2Runes.EntriesList;
        //    var runeNames = allRunes
        //    .Select(r => r.Id)
        //    .Distinct()
        //    .ToList();
        //
        //    DBug.Log($"Unique Runes found: {string.Join(", ", runeNames)}");
        //}

        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();

        ImGui.Text("Price Overrides");

        var knownRecipes = Settings.KnownRecipes.OrderBy(x => x).ToList();

        for (int i = 0; i < Settings.PriceOverrides.Count; i++) {
            var priceOverride = Settings.PriceOverrides[i];
            ImGui.PushID(i);

            int buttonWidth = 60;
            float totalWidth = ImGui.GetContentRegionAvail().X;
            float sharedWidth = (totalWidth - buttonWidth - 10) / 2f;

            priceOverride.Recipee.SetListValues(knownRecipes);
            string recipee = priceOverride.Recipee.Value ?? "";

            ImGui.SetNextItemWidth(sharedWidth);
            priceOverride.Recipee.DrawPicker("##priceOverrideRecipee", ref recipee);
            priceOverride.Recipee.Value = recipee;

            ImGui.SameLine();
            DXT.Slider.Draw($"priceOverrideValue{i}", ref priceOverride.Value, new DXT.Slider.Options { Max = 1000, Width = (int)sharedWidth });

            ImGui.SameLine();
            if (DXT.Button.Draw($"priceOverrideRemove{i}", new DXT.Button.Options { Label = "Remove", Width = buttonWidth })) Settings.PriceOverrides.RemoveAt(i);

            ImGui.PopID();
        }
        if (DXT.Button.Draw($"AddOverride", new DXT.Button.Options { Label = "Add Override" })) Settings.PriceOverrides.Add(new PriceOverride());


        //ImGui.Unindent();//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //}





    }







}






