using SColor = System.Drawing.Color;

namespace DieselExileTools.Common;
public static partial class DXTC {
    public static class Colors
    {

        public static readonly SColor WindowBackground = SColor.FromArgb(0, 0, 0);
        public static readonly SColor Border = SColor.FromArgb(0, 0, 0);
        public static readonly SColor Hover = Color.FromRGBA(255, 255, 255, 25);
        public static readonly SColor Text = Color.FromRGBA(255, 255, 255, 178);


        public static readonly SColor Panel = Color.FromHSLA(220, .15f, .15f);
        public static readonly SColor PanelInnerGlow = Color.FromRGBA(255, 255, 255, 5);

        public static readonly SColor PanelHeader = Color.FromHSLA(220, .15f, .24f);

        public static readonly SColor ControlRed = Color.FromHSLA(0, .80f, .24f);
        public static readonly SColor ControlGreen = Color.FromHSLA(120, .70f, .30f);
        public static readonly SColor ControlBlue = SColor.FromArgb(38, 127, 216);
        public static readonly SColor ControlOrange = SColor.FromArgb(173, 81, 31);
        public static readonly SColor ControlYellow = SColor.FromArgb(188, 161, 21);
        public static readonly SColor ControlPurple = SColor.FromArgb(119, 31, 173);
        public static readonly SColor ControlPink = SColor.FromArgb(173, 31, 93);
        public static readonly SColor ControlText = Text;

        public static readonly SColor TextYellow = DXTC.Palettes.Material.Yellow.Yellow500.Color;


        public static readonly SColor ControlInnerGlow = Color.FromRGBA(255, 255, 255, 12);

        public static readonly SColor Input = Color.FromHSLA(220, .15f, .08f);

        public static readonly SColor InputInnerGlow = Color.FromRGBA(255, 255, 255, 5);
        public static readonly SColor InputText = Color.FromHSLA(220, .15f, .65f);
        public static readonly SColor InputHovered = Color.FromRGBA(255, 255, 255, 12);
        public static readonly SColor Button = Color.FromHSLA(220, .15f, .28f);
        public static readonly SColor ButtonClose = Color.FromHSLA(0, .90f, .15f);
        public static readonly SColor ButtonInnerGlow = Color.FromRGBA(255, 255, 255, 12);
        public static readonly SColor ButtonChecked = ControlBlue;
        public static readonly SColor ButtonHovered = Hover;
        public static readonly SColor ButtonText = Text;
        public static readonly SColor ButtonTextChecked = Color.FromRGBA(255, 255, 255);

        public static readonly SColor SwatchInnerGlow = Color.FromRGBA(255, 255, 255, 15);





    }
}
