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

        //DXT.Checkbox.Draw("Show Values in Divine Orbs", ref Settings.ShowValueInDivines);

        DXT.Checkbox.Draw("Show Remnants with Unclaimed Rewards", ref Settings.DisplayUnclaimedRewardRemnants);

        DXT.Checkbox.Draw("Show Active Remnants", ref Settings.DisplayActiveRemnants);

        DXT.Checkbox.Draw("Show Pending Remnants", ref Settings.DisplayPendingRemnants);

        DXT.Checkbox.Draw("Show Completed Remnants", ref Settings.DisplayCompletedRemnants);


        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();

        DXT.Checkbox.Draw("Draw on minimap", ref Settings.MinimapRemnant_Show);

        DXT.Checkbox.Draw("Draw transfered Runes on minimap", ref Settings.MinimapRemnant_ShowTransferred);


        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();


        DXT.Checkbox.Draw("Draw on Remnant", ref Settings.InGameRemnant_Show);

        //DXT.Slider.Draw("MinimumValueToShow", ref Settings.InGameRemnant_MinimumValueToShow, new DXT.Slider.Options { Max = 1000, Width = 100 });
        //ImGui.SameLine();
        //ImGui.Text($"Minimum Value recipes To Show");

        DXT.Slider.Draw("MaxItemsToShow", ref Settings.InGameRemnant_MaxItemsToShow, new DXT.Slider.Options { Max = 20, Width = 100 });
        ImGui.SameLine();
        ImGui.Text($"Max recipees To Show on Remnant");

        DXT.Slider.Draw("RenderOffsetX", ref Settings.InGameRemnant_RenderOffset.X, new DXT.Slider.Options { Max = 50, Width = 100 });
        ImGui.SameLine();
        DXT.Slider.Draw("RenderOffsetY", ref Settings.InGameRemnant_RenderOffset.Y, new DXT.Slider.Options { Max = 50, Width = 100 });
        ImGui.SameLine();
        ImGui.Text("Remnant render offfset");

        ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();


        DXT.ColorSelect.Draw("BG_Color", "Label Background Color", ref Settings.BG_Color);
        ImGui.SameLine();
        ImGui.Text("Label Background Color");

        DXT.ColorSelect.Draw("ExplosiveHover_Color", "Explosive Hover Border Color", ref Settings.ExplosiveHover_Color);
        ImGui.SameLine();
        ImGui.Text("Explosive Hovered Border Color");

        DXT.ColorSelect.Draw("RerolledBorder_Color", "Rerolled Border Border", ref Settings.RerolledBorder_Color);
        ImGui.SameLine();
        ImGui.Text("Rerolled Border Color");

        DXT.ColorSelect.Draw("SelectedBorder_Color", "Selected Recipe Border Border", ref Settings.SelectedBorder_Color);
        ImGui.SameLine();
        ImGui.Text("Selected Recipe Border Color");

        DXT.ColorSelect.Draw("LabelText_Color", "Label Color", ref Settings.LabelText_Color);
        ImGui.SameLine();
        ImGui.Text("Label Color");

        DXT.ColorSelect.Draw("ValueText_Color", "Value Color", ref Settings.ValueText_Color);
        ImGui.SameLine();
        ImGui.Text("Value(Divines) Color");

        DXT.ColorSelect.Draw("ValueGoodText_Color", "Good Value Color", ref Settings.ValueGoodText_Color);
        ImGui.SameLine();
        DXT.Slider.Draw("ValueGood", ref Settings.ValueGood, new DXT.Slider.Options { Max = 100, Width = 100 });
        ImGui.SameLine();
        ImGui.Text("Good Value(Divines)");

        DXT.ColorSelect.Draw("ValueVeryGoodText_Color", "Very Good Value Color", ref Settings.ValueVeryGoodText_Color);
        ImGui.SameLine();
        DXT.Slider.Draw("ValueVeryGood", ref Settings.ValueVeryGood, new DXT.Slider.Options { Max = 100, Width = 100, ShiftStep = .1f });
        ImGui.SameLine();
        ImGui.Text("Very Good Value(Divines)");

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
            DXT.Slider.Draw($"priceOverrideValue{i}", ref priceOverride.Value, new DXT.Slider.Options { Max = 100, ShiftStep = .1f, Width = (int)sharedWidth });

            ImGui.SameLine();
            if (DXT.Button.Draw($"priceOverrideRemove{i}", new DXT.Button.Options { Label = "Remove", Width = buttonWidth })) Settings.PriceOverrides.RemoveAt(i);

            ImGui.PopID();
        }
        if (DXT.Button.Draw($"AddOverride", new DXT.Button.Options { Label = "Add Override" })) Settings.PriceOverrides.Add(new PriceOverride());


        //ImGui.Unindent();//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //}





    }







}






