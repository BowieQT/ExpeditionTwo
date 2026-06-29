
using System.Numerics;
using SColor = System.Drawing.Color;

namespace DieselExileTools.Common;

public static partial class DXTC {

    public class PaletteSwatch {
        public string Name { get; }
        public SColor Color { get; }  // System.Drawing.Color
        public uint Uint { get; }   // Precomputed for ImGui
        public Vector4 Vec4 { get; } // Precomputed normalized

        public PaletteSwatch(string name, SColor color) {
            Name = name;
            Color = color;
            Uint = color.ToImGui();
            Vec4 = color.ToVector4();
        }
    }

    public static class Palettes
    {
        public static class Material
        {
            public static class Red {
                public static readonly PaletteSwatch Red50 = new("Red 50", SColor.FromArgb(255, 235, 238));
                public static readonly PaletteSwatch Red100 = new("Red 100", SColor.FromArgb(255, 205, 210));
                public static readonly PaletteSwatch Red200 = new("Red 200", SColor.FromArgb(239, 154, 154));
                public static readonly PaletteSwatch Red300 = new("Red 300", SColor.FromArgb(229, 115, 115));
                public static readonly PaletteSwatch Red400 = new("Red 400", SColor.FromArgb(239, 83, 80));
                public static readonly PaletteSwatch Red500 = new("Red 500", SColor.FromArgb(244, 67, 54));
                public static readonly PaletteSwatch Red600 = new("Red 600", SColor.FromArgb(229, 57, 53));
                public static readonly PaletteSwatch Red700 = new("Red 700", SColor.FromArgb(211, 47, 47));
                public static readonly PaletteSwatch Red800 = new("Red 800", SColor.FromArgb(198, 40, 40));
                public static readonly PaletteSwatch Red900 = new("Red 900", SColor.FromArgb(183, 28, 28));
                public static readonly PaletteSwatch RedA100 = new("Red A100", SColor.FromArgb(255, 138, 128));
                public static readonly PaletteSwatch RedA200 = new("Red A200", SColor.FromArgb(255, 82, 82));
                public static readonly PaletteSwatch RedA400 = new("Red A400", SColor.FromArgb(255, 23, 68));
                public static readonly PaletteSwatch RedA700 = new("Red A700", SColor.FromArgb(213, 0, 0));
            }
            public static class Pink {
                public static readonly PaletteSwatch Pink50 = new("Pink 50", SColor.FromArgb(252, 228, 236));
                public static readonly PaletteSwatch Pink100 = new("Pink 100", SColor.FromArgb(248, 187, 208));
                public static readonly PaletteSwatch Pink200 = new("Pink 200", SColor.FromArgb(244, 143, 177));
                public static readonly PaletteSwatch Pink300 = new("Pink 300", SColor.FromArgb(240, 98, 146));
                public static readonly PaletteSwatch Pink400 = new("Pink 400", SColor.FromArgb(236, 64, 122));
                public static readonly PaletteSwatch Pink500 = new("Pink 500", SColor.FromArgb(233, 30, 99));
                public static readonly PaletteSwatch Pink600 = new("Pink 600", SColor.FromArgb(216, 27, 96));
                public static readonly PaletteSwatch Pink700 = new("Pink 700", SColor.FromArgb(194, 24, 91));
                public static readonly PaletteSwatch Pink800 = new("Pink 800", SColor.FromArgb(173, 20, 87));
                public static readonly PaletteSwatch Pink900 = new("Pink 900", SColor.FromArgb(136, 14, 79));
                public static readonly PaletteSwatch PinkA100 = new("Pink A100", SColor.FromArgb(255, 128, 171));
                public static readonly PaletteSwatch PinkA200 = new("Pink A200", SColor.FromArgb(255, 64, 129));
                public static readonly PaletteSwatch PinkA400 = new("Pink A400", SColor.FromArgb(245, 0, 87));
                public static readonly PaletteSwatch PinkA700 = new("Pink A700", SColor.FromArgb(197, 17, 98));
            }
            public static class Purple
            {
                public static readonly PaletteSwatch Purple50 = new("Purple 50", SColor.FromArgb(243, 229, 245));
                public static readonly PaletteSwatch Purple100 = new("Purple 100", SColor.FromArgb(225, 190, 231));
                public static readonly PaletteSwatch Purple200 = new("Purple 200", SColor.FromArgb(206, 147, 216));
                public static readonly PaletteSwatch Purple300 = new("Purple 300", SColor.FromArgb(186, 104, 200));
                public static readonly PaletteSwatch Purple400 = new("Purple 400", SColor.FromArgb(171, 71, 188));
                public static readonly PaletteSwatch Purple500 = new("Purple 500", SColor.FromArgb(156, 39, 176));
                public static readonly PaletteSwatch Purple600 = new("Purple 600", SColor.FromArgb(142, 36, 170));
                public static readonly PaletteSwatch Purple700 = new("Purple 700", SColor.FromArgb(123, 31, 162));
                public static readonly PaletteSwatch Purple800 = new("Purple 800", SColor.FromArgb(106, 27, 154));
                public static readonly PaletteSwatch Purple900 = new("Purple 900", SColor.FromArgb(74, 20, 140));
                public static readonly PaletteSwatch PurpleA100 = new("Purple A100", SColor.FromArgb(234, 128, 252));
                public static readonly PaletteSwatch PurpleA200 = new("Purple A200", SColor.FromArgb(224, 64, 251));
                public static readonly PaletteSwatch PurpleA400 = new("Purple A400", SColor.FromArgb(213, 0, 249));
                public static readonly PaletteSwatch PurpleA700 = new("Purple A700", SColor.FromArgb(170, 0, 255));
            }
            public static class DeepPurple
            {
                public static readonly PaletteSwatch DeepPurple50 = new("Deep Purple 50", SColor.FromArgb(237, 231, 246));
                public static readonly PaletteSwatch DeepPurple100 = new("Deep Purple 100", SColor.FromArgb(209, 196, 233));
                public static readonly PaletteSwatch DeepPurple200 = new("Deep Purple 200", SColor.FromArgb(179, 157, 219));
                public static readonly PaletteSwatch DeepPurple300 = new("Deep Purple 300", SColor.FromArgb(149, 117, 205));
                public static readonly PaletteSwatch DeepPurple400 = new("Deep Purple 400", SColor.FromArgb(126, 87, 194));
                public static readonly PaletteSwatch DeepPurple500 = new("Deep Purple 500", SColor.FromArgb(103, 58, 183));
                public static readonly PaletteSwatch DeepPurple600 = new("Deep Purple 600", SColor.FromArgb(94, 53, 177));
                public static readonly PaletteSwatch DeepPurple700 = new("Deep Purple 700", SColor.FromArgb(81, 45, 168));
                public static readonly PaletteSwatch DeepPurple800 = new("Deep Purple 800", SColor.FromArgb(69, 39, 160));
                public static readonly PaletteSwatch DeepPurple900 = new("Deep Purple 900", SColor.FromArgb(49, 27, 146));
                public static readonly PaletteSwatch DeepPurpleA100 = new("Deep Purple A100", SColor.FromArgb(179, 136, 255));
                public static readonly PaletteSwatch DeepPurpleA200 = new("Deep Purple A200", SColor.FromArgb(124, 77, 255));
                public static readonly PaletteSwatch DeepPurpleA400 = new("Deep Purple A400", SColor.FromArgb(101, 31, 255));
                public static readonly PaletteSwatch DeepPurpleA700 = new("Deep Purple A700", SColor.FromArgb(98, 0, 234));
            }
            public static class Indigo
            {
                public static readonly PaletteSwatch Indigo50 = new("Indigo 50", SColor.FromArgb(232, 234, 246));
                public static readonly PaletteSwatch Indigo100 = new("Indigo 100", SColor.FromArgb(197, 202, 233));
                public static readonly PaletteSwatch Indigo200 = new("Indigo 200", SColor.FromArgb(159, 168, 218));
                public static readonly PaletteSwatch Indigo300 = new("Indigo 300", SColor.FromArgb(121, 134, 203));
                public static readonly PaletteSwatch Indigo400 = new("Indigo 400", SColor.FromArgb(92, 107, 192));
                public static readonly PaletteSwatch Indigo500 = new("Indigo 500", SColor.FromArgb(63, 81, 181));
                public static readonly PaletteSwatch Indigo600 = new("Indigo 600", SColor.FromArgb(57, 73, 171));
                public static readonly PaletteSwatch Indigo700 = new("Indigo 700", SColor.FromArgb(48, 63, 159));
                public static readonly PaletteSwatch Indigo800 = new("Indigo 800", SColor.FromArgb(40, 53, 147));
                public static readonly PaletteSwatch Indigo900 = new("Indigo 900", SColor.FromArgb(26, 35, 126));
                public static readonly PaletteSwatch IndigoA100 = new("Indigo A100", SColor.FromArgb(140, 158, 255));
                public static readonly PaletteSwatch IndigoA200 = new("Indigo A200", SColor.FromArgb(83, 109, 254));
                public static readonly PaletteSwatch IndigoA400 = new("Indigo A400", SColor.FromArgb(61, 90, 254));
                public static readonly PaletteSwatch IndigoA700 = new("Indigo A700", SColor.FromArgb(48, 79, 254));
            }
            public static class Blue
            {
                public static readonly PaletteSwatch Blue50 = new("Blue 50", SColor.FromArgb(227, 242, 253));
                public static readonly PaletteSwatch Blue100 = new("Blue 100", SColor.FromArgb(187, 222, 251));
                public static readonly PaletteSwatch Blue200 = new("Blue 200", SColor.FromArgb(144, 202, 249));
                public static readonly PaletteSwatch Blue300 = new("Blue 300", SColor.FromArgb(100, 181, 246));
                public static readonly PaletteSwatch Blue400 = new("Blue 400", SColor.FromArgb(66, 165, 245));
                public static readonly PaletteSwatch Blue500 = new("Blue 500", SColor.FromArgb(33, 150, 243));
                public static readonly PaletteSwatch Blue600 = new("Blue 600", SColor.FromArgb(30, 136, 229));
                public static readonly PaletteSwatch Blue700 = new("Blue 700", SColor.FromArgb(25, 118, 210));
                public static readonly PaletteSwatch Blue800 = new("Blue 800", SColor.FromArgb(21, 101, 192));
                public static readonly PaletteSwatch Blue900 = new("Blue 900", SColor.FromArgb(13, 71, 161));
                public static readonly PaletteSwatch BlueA100 = new("Blue A100", SColor.FromArgb(130, 177, 255));
                public static readonly PaletteSwatch BlueA200 = new("Blue A200", SColor.FromArgb(68, 138, 255));
                public static readonly PaletteSwatch BlueA400 = new("Blue A400", SColor.FromArgb(41, 121, 255));
                public static readonly PaletteSwatch BlueA700 = new("Blue A700", SColor.FromArgb(41, 98, 255));
            }
            public static class LightBlue
            {
                public static readonly PaletteSwatch LightBlue50 = new("Light Blue 50", SColor.FromArgb(225, 245, 254));
                public static readonly PaletteSwatch LightBlue100 = new("Light Blue 100", SColor.FromArgb(179, 229, 252));
                public static readonly PaletteSwatch LightBlue200 = new("Light Blue 200", SColor.FromArgb(129, 212, 250));
                public static readonly PaletteSwatch LightBlue300 = new("Light Blue 300", SColor.FromArgb(79, 195, 247));
                public static readonly PaletteSwatch LightBlue400 = new("Light Blue 400", SColor.FromArgb(41, 182, 246));
                public static readonly PaletteSwatch LightBlue500 = new("Light Blue 500", SColor.FromArgb(3, 169, 244));
                public static readonly PaletteSwatch LightBlue600 = new("Light Blue 600", SColor.FromArgb(3, 155, 229));
                public static readonly PaletteSwatch LightBlue700 = new("Light Blue 700", SColor.FromArgb(2, 136, 209));
                public static readonly PaletteSwatch LightBlue800 = new("Light Blue 800", SColor.FromArgb(2, 119, 189));
                public static readonly PaletteSwatch LightBlue900 = new("Light Blue 900", SColor.FromArgb(1, 87, 155));
                public static readonly PaletteSwatch LightBlueA100 = new("Light Blue A100", SColor.FromArgb(128, 216, 255));
                public static readonly PaletteSwatch LightBlueA200 = new("Light Blue A200", SColor.FromArgb(64, 196, 255));
                public static readonly PaletteSwatch LightBlueA400 = new("Light Blue A400", SColor.FromArgb(0, 176, 255));
                public static readonly PaletteSwatch LightBlueA700 = new("Light Blue A700", SColor.FromArgb(0, 145, 234));
            }
            public static class Cyan
            {
                public static readonly PaletteSwatch Cyan50 = new("Cyan 50", SColor.FromArgb(224, 247, 250));
                public static readonly PaletteSwatch Cyan100 = new("Cyan 100", SColor.FromArgb(178, 235, 242));
                public static readonly PaletteSwatch Cyan200 = new("Cyan 200", SColor.FromArgb(128, 222, 234));
                public static readonly PaletteSwatch Cyan300 = new("Cyan 300", SColor.FromArgb(77, 208, 225));
                public static readonly PaletteSwatch Cyan400 = new("Cyan 400", SColor.FromArgb(38, 198, 218));
                public static readonly PaletteSwatch Cyan500 = new("Cyan 500", SColor.FromArgb(0, 188, 212));
                public static readonly PaletteSwatch Cyan600 = new("Cyan 600", SColor.FromArgb(0, 172, 193));
                public static readonly PaletteSwatch Cyan700 = new("Cyan 700", SColor.FromArgb(0, 151, 167));
                public static readonly PaletteSwatch Cyan800 = new("Cyan 800", SColor.FromArgb(0, 131, 143));
                public static readonly PaletteSwatch Cyan900 = new("Cyan 900", SColor.FromArgb(0, 96, 100));
                public static readonly PaletteSwatch CyanA100 = new("Cyan A100", SColor.FromArgb(132, 255, 255));
                public static readonly PaletteSwatch CyanA200 = new("Cyan A200", SColor.FromArgb(24, 255, 255));
                public static readonly PaletteSwatch CyanA400 = new("Cyan A400", SColor.FromArgb(0, 229, 255));
                public static readonly PaletteSwatch CyanA700 = new("Cyan A700", SColor.FromArgb(0, 184, 212));
            }
            public static class Teal
            {
                public static readonly PaletteSwatch Teal50 = new("Teal 50", SColor.FromArgb(224, 242, 241));
                public static readonly PaletteSwatch Teal100 = new("Teal 100", SColor.FromArgb(178, 223, 219));
                public static readonly PaletteSwatch Teal200 = new("Teal 200", SColor.FromArgb(128, 203, 196));
                public static readonly PaletteSwatch Teal300 = new("Teal 300", SColor.FromArgb(77, 182, 172));
                public static readonly PaletteSwatch Teal400 = new("Teal 400", SColor.FromArgb(38, 166, 154));
                public static readonly PaletteSwatch Teal500 = new("Teal 500", SColor.FromArgb(0, 150, 136));
                public static readonly PaletteSwatch Teal600 = new("Teal 600", SColor.FromArgb(0, 137, 123));
                public static readonly PaletteSwatch Teal700 = new("Teal 700", SColor.FromArgb(0, 121, 107));
                public static readonly PaletteSwatch Teal800 = new("Teal 800", SColor.FromArgb(0, 105, 92));
                public static readonly PaletteSwatch Teal900 = new("Teal 900", SColor.FromArgb(0, 77, 64));
                public static readonly PaletteSwatch TealA100 = new("Teal A100", SColor.FromArgb(167, 255, 235));
                public static readonly PaletteSwatch TealA200 = new("Teal A200", SColor.FromArgb(100, 255, 218));
                public static readonly PaletteSwatch TealA400 = new("Teal A400", SColor.FromArgb(29, 233, 182));
                public static readonly PaletteSwatch TealA700 = new("Teal A700", SColor.FromArgb(0, 191, 165));
            }
            public static class Green
            {
                public static readonly PaletteSwatch Green50 = new("Green 50", SColor.FromArgb(232, 245, 233));
                public static readonly PaletteSwatch Green100 = new("Green 100", SColor.FromArgb(200, 230, 201));
                public static readonly PaletteSwatch Green200 = new("Green 200", SColor.FromArgb(165, 214, 167));
                public static readonly PaletteSwatch Green300 = new("Green 300", SColor.FromArgb(129, 199, 132));
                public static readonly PaletteSwatch Green400 = new("Green 400", SColor.FromArgb(102, 187, 106));
                public static readonly PaletteSwatch Green500 = new("Green 500", SColor.FromArgb(76, 175, 80));
                public static readonly PaletteSwatch Green600 = new("Green 600", SColor.FromArgb(67, 160, 71));
                public static readonly PaletteSwatch Green700 = new("Green 700", SColor.FromArgb(56, 142, 60));
                public static readonly PaletteSwatch Green800 = new("Green 800", SColor.FromArgb(46, 125, 50));
                public static readonly PaletteSwatch Green900 = new("Green 900", SColor.FromArgb(27, 94, 32));
                public static readonly PaletteSwatch GreenA100 = new("Green A100", SColor.FromArgb(185, 246, 202));
                public static readonly PaletteSwatch GreenA200 = new("Green A200", SColor.FromArgb(105, 240, 174));
                public static readonly PaletteSwatch GreenA400 = new("Green A400", SColor.FromArgb(0, 230, 118));
                public static readonly PaletteSwatch GreenA700 = new("Green A700", SColor.FromArgb(0, 200, 83));
            }
            public static class LightGreen
            {
                public static readonly PaletteSwatch LightGreen50 = new("Light Green 50", SColor.FromArgb(241, 248, 233));
                public static readonly PaletteSwatch LightGreen100 = new("Light Green 100", SColor.FromArgb(220, 237, 200));
                public static readonly PaletteSwatch LightGreen200 = new("Light Green 200", SColor.FromArgb(197, 225, 165));
                public static readonly PaletteSwatch LightGreen300 = new("Light Green 300", SColor.FromArgb(174, 213, 129));
                public static readonly PaletteSwatch LightGreen400 = new("Light Green 400", SColor.FromArgb(156, 204, 101));
                public static readonly PaletteSwatch LightGreen500 = new("Light Green 500", SColor.FromArgb(139, 195, 74));
                public static readonly PaletteSwatch LightGreen600 = new("Light Green 600", SColor.FromArgb(124, 179, 66));
                public static readonly PaletteSwatch LightGreen700 = new("Light Green 700", SColor.FromArgb(104, 159, 56));
                public static readonly PaletteSwatch LightGreen800 = new("Light Green 800", SColor.FromArgb(85, 139, 47));
                public static readonly PaletteSwatch LightGreen900 = new("Light Green 900", SColor.FromArgb(51, 105, 30));
                public static readonly PaletteSwatch LightGreenA100 = new("Light Green A100", SColor.FromArgb(204, 255, 144));
                public static readonly PaletteSwatch LightGreenA200 = new("Light Green A200", SColor.FromArgb(178, 255, 89));
                public static readonly PaletteSwatch LightGreenA400 = new("Light Green A400", SColor.FromArgb(118, 255, 3));
                public static readonly PaletteSwatch LightGreenA700 = new("Light Green A700", SColor.FromArgb(100, 221, 23));
            }
            public static class Lime
            {
                public static readonly PaletteSwatch Lime50 = new("Lime 50", SColor.FromArgb(240, 249, 235));
                public static readonly PaletteSwatch Lime100 = new("Lime 100", SColor.FromArgb(230, 244, 208));
                public static readonly PaletteSwatch Lime200 = new("Lime 200", SColor.FromArgb(220, 238, 179));
                public static readonly PaletteSwatch Lime300 = new("Lime 300", SColor.FromArgb(212, 225, 147));
                public static readonly PaletteSwatch Lime400 = new("Lime 400", SColor.FromArgb(197, 220, 121));
                public static readonly PaletteSwatch Lime500 = new("Lime 500", SColor.FromArgb(175, 222, 71));
                public static readonly PaletteSwatch Lime600 = new("Lime 600", SColor.FromArgb(165, 214, 58));
                public static readonly PaletteSwatch Lime700 = new("Lime 700", SColor.FromArgb(145, 198, 36));
                public static readonly PaletteSwatch Lime800 = new("Lime 800", SColor.FromArgb(124, 179, 27));
                public static readonly PaletteSwatch Lime900 = new("Lime 900", SColor.FromArgb(104, 159, 15));
                public static readonly PaletteSwatch LimeA100 = new("Lime A100", SColor.FromArgb(244, 255, 129));
                public static readonly PaletteSwatch LimeA200 = new("Lime A200", SColor.FromArgb(230, 255, 0));
                public static readonly PaletteSwatch LimeA400 = new("Lime A400", SColor.FromArgb(198, 255, 0));
                public static readonly PaletteSwatch LimeA700 = new("Lime A700", SColor.FromArgb(174, 234, 0));
            }
            public static class Yellow
            {
                public static readonly PaletteSwatch Yellow50 = new("Yellow 50", SColor.FromArgb(255, 253, 231));
                public static readonly PaletteSwatch Yellow100 = new("Yellow 100", SColor.FromArgb(255, 249, 196));
                public static readonly PaletteSwatch Yellow200 = new("Yellow 200", SColor.FromArgb(255, 245, 157));
                public static readonly PaletteSwatch Yellow300 = new("Yellow 300", SColor.FromArgb(255, 241, 118));
                public static readonly PaletteSwatch Yellow400 = new("Yellow 400", SColor.FromArgb(255, 238, 88));
                public static readonly PaletteSwatch Yellow500 = new("Yellow 500", SColor.FromArgb(255, 235, 59));
                public static readonly PaletteSwatch Yellow600 = new("Yellow 600", SColor.FromArgb(253, 216, 53));
                public static readonly PaletteSwatch Yellow700 = new("Yellow 700", SColor.FromArgb(251, 192, 45));
                public static readonly PaletteSwatch Yellow800 = new("Yellow 800", SColor.FromArgb(249, 168, 37));
                public static readonly PaletteSwatch Yellow900 = new("Yellow 900", SColor.FromArgb(245, 127, 23));
                public static readonly PaletteSwatch YellowA100 = new("Yellow A100", SColor.FromArgb(255, 255, 141));
                public static readonly PaletteSwatch YellowA200 = new("Yellow A200", SColor.FromArgb(255, 255, 0));
                public static readonly PaletteSwatch YellowA400 = new("Yellow A400", SColor.FromArgb(255, 234, 0));
                public static readonly PaletteSwatch YellowA700 = new("Yellow A700", SColor.FromArgb(255, 214, 0));
            }
            public static class Amber
            {
                public static readonly PaletteSwatch Amber50 = new("Amber 50", SColor.FromArgb(255, 248, 225));
                public static readonly PaletteSwatch Amber100 = new("Amber 100", SColor.FromArgb(255, 236, 179));
                public static readonly PaletteSwatch Amber200 = new("Amber 200", SColor.FromArgb(255, 224, 130));
                public static readonly PaletteSwatch Amber300 = new("Amber 300", SColor.FromArgb(255, 213, 79));
                public static readonly PaletteSwatch Amber400 = new("Amber 400", SColor.FromArgb(255, 202, 40));
                public static readonly PaletteSwatch Amber500 = new("Amber 500", SColor.FromArgb(255, 193, 7));
                public static readonly PaletteSwatch Amber600 = new("Amber 600", SColor.FromArgb(255, 179, 0));
                public static readonly PaletteSwatch Amber700 = new("Amber 700", SColor.FromArgb(255, 160, 0));
                public static readonly PaletteSwatch Amber800 = new("Amber 800", SColor.FromArgb(255, 143, 0));
                public static readonly PaletteSwatch Amber900 = new("Amber 900", SColor.FromArgb(255, 111, 0));
                public static readonly PaletteSwatch AmberA100 = new("Amber A100", SColor.FromArgb(255, 255, 209));
                public static readonly PaletteSwatch AmberA200 = new("Amber A200", SColor.FromArgb(255, 255, 171));
                public static readonly PaletteSwatch AmberA400 = new("Amber A400", SColor.FromArgb(255, 255, 145));
                public static readonly PaletteSwatch AmberA700 = new("Amber A700", SColor.FromArgb(255, 255, 109));
            }
            public static class Orange
            {
                public static readonly PaletteSwatch Orange50 = new("Orange 50", SColor.FromArgb(255, 243, 224));
                public static readonly PaletteSwatch Orange100 = new("Orange 100", SColor.FromArgb(255, 224, 178));
                public static readonly PaletteSwatch Orange200 = new("Orange 200", SColor.FromArgb(255, 204, 128));
                public static readonly PaletteSwatch Orange300 = new("Orange 300", SColor.FromArgb(255, 183, 77));
                public static readonly PaletteSwatch Orange400 = new("Orange 400", SColor.FromArgb(255, 167, 38));
                public static readonly PaletteSwatch Orange500 = new("Orange 500", SColor.FromArgb(255, 152, 0));
                public static readonly PaletteSwatch Orange600 = new("Orange 600", SColor.FromArgb(251, 140, 0));
                public static readonly PaletteSwatch Orange700 = new("Orange 700", SColor.FromArgb(245, 124, 0));
                public static readonly PaletteSwatch Orange800 = new("Orange 800", SColor.FromArgb(239, 108, 0));
                public static readonly PaletteSwatch Orange900 = new("Orange 900", SColor.FromArgb(230, 81, 0));
                public static readonly PaletteSwatch OrangeA100 = new("Orange A100", SColor.FromArgb(255, 209, 128));
                public static readonly PaletteSwatch OrangeA200 = new("Orange A200", SColor.FromArgb(255, 171, 64));
                public static readonly PaletteSwatch OrangeA400 = new("Orange A400", SColor.FromArgb(255, 145, 0));
                public static readonly PaletteSwatch OrangeA700 = new("Orange A700", SColor.FromArgb(255, 109, 0));
            }
            public static class DeepOrange
            {
                public static readonly PaletteSwatch DeepOrange50 = new("Deep Orange 50", SColor.FromArgb(251, 233, 231));
                public static readonly PaletteSwatch DeepOrange100 = new("Deep Orange 100", SColor.FromArgb(255, 204, 188));
                public static readonly PaletteSwatch DeepOrange200 = new("Deep Orange 200", SColor.FromArgb(255, 171, 145));
                public static readonly PaletteSwatch DeepOrange300 = new("Deep Orange 300", SColor.FromArgb(255, 138, 101));
                public static readonly PaletteSwatch DeepOrange400 = new("Deep Orange 400", SColor.FromArgb(255, 112, 67));
                public static readonly PaletteSwatch DeepOrange500 = new("Deep Orange 500", SColor.FromArgb(255, 87, 34));
                public static readonly PaletteSwatch DeepOrange600 = new("Deep Orange 600", SColor.FromArgb(244, 81, 30));
                public static readonly PaletteSwatch DeepOrange700 = new("Deep Orange 700", SColor.FromArgb(230, 74, 25));
                public static readonly PaletteSwatch DeepOrange800 = new("Deep Orange 800", SColor.FromArgb(216, 67, 21));
                public static readonly PaletteSwatch DeepOrange900 = new("Deep Orange 900", SColor.FromArgb(191, 54, 12));
                public static readonly PaletteSwatch DeepOrangeA100 = new("Deep Orange A100", SColor.FromArgb(255, 158, 128));
                public static readonly PaletteSwatch DeepOrangeA200 = new("Deep Orange A200", SColor.FromArgb(255, 110, 64));
                public static readonly PaletteSwatch DeepOrangeA400 = new("Deep Orange A400", SColor.FromArgb(255, 61, 0));
                public static readonly PaletteSwatch DeepOrangeA700 = new("Deep Orange A700", SColor.FromArgb(221, 44, 0));
            }
            public static class Brown
            {
                public static readonly PaletteSwatch Brown50 = new("Brown 50", SColor.FromArgb(239, 235, 233));
                public static readonly PaletteSwatch Brown100 = new("Brown 100", SColor.FromArgb(215, 204, 200));
                public static readonly PaletteSwatch Brown200 = new("Brown 200", SColor.FromArgb(188, 170, 164));
                public static readonly PaletteSwatch Brown300 = new("Brown 300", SColor.FromArgb(161, 136, 127));
                public static readonly PaletteSwatch Brown400 = new("Brown 400", SColor.FromArgb(141, 110, 99));
                public static readonly PaletteSwatch Brown500 = new("Brown 500", SColor.FromArgb(121, 85, 72));
                public static readonly PaletteSwatch Brown600 = new("Brown 600", SColor.FromArgb(109, 76, 65));
                public static readonly PaletteSwatch Brown700 = new("Brown 700", SColor.FromArgb(93, 64, 55));
                public static readonly PaletteSwatch Brown800 = new("Brown 800", SColor.FromArgb(78, 52, 46));
                public static readonly PaletteSwatch Brown900 = new("Brown 900", SColor.FromArgb(62, 39, 35));
            }
            public static class Grey
            {
                public static readonly PaletteSwatch Grey50 = new("Grey 50", SColor.FromArgb(250, 250, 250));
                public static readonly PaletteSwatch Grey100 = new("Grey 100", SColor.FromArgb(245, 245, 245));
                public static readonly PaletteSwatch Grey200 = new("Grey 200", SColor.FromArgb(238, 238, 238));
                public static readonly PaletteSwatch Grey300 = new("Grey 300", SColor.FromArgb(224, 224, 224));
                public static readonly PaletteSwatch Grey400 = new("Grey 400", SColor.FromArgb(189, 189, 189));
                public static readonly PaletteSwatch Grey500 = new("Grey 500", SColor.FromArgb(158, 158, 158));
                public static readonly PaletteSwatch Grey600 = new("Grey 600", SColor.FromArgb(117, 117, 117));
                public static readonly PaletteSwatch Grey700 = new("Grey 700", SColor.FromArgb(97, 97, 97));
                public static readonly PaletteSwatch Grey800 = new("Grey 800", SColor.FromArgb(66, 66, 66));
                public static readonly PaletteSwatch Grey900 = new("Grey 900", SColor.FromArgb(33, 33, 33));
            }
            public static class BlueGrey
            {
                public static readonly PaletteSwatch BlueGrey50 = new("Blue Grey 50", SColor.FromArgb(236, 239, 241));
                public static readonly PaletteSwatch BlueGrey100 = new("Blue Grey 100", SColor.FromArgb(207, 216, 220));
                public static readonly PaletteSwatch BlueGrey200 = new("Blue Grey 200", SColor.FromArgb(176, 190, 197));
                public static readonly PaletteSwatch BlueGrey300 = new("Blue Grey 300", SColor.FromArgb(144, 164, 174));
                public static readonly PaletteSwatch BlueGrey400 = new("Blue Grey 400", SColor.FromArgb(120, 144, 156));
                public static readonly PaletteSwatch BlueGrey500 = new("Blue Grey 500", SColor.FromArgb(96, 125, 139));
                public static readonly PaletteSwatch BlueGrey600 = new("Blue Grey 600", SColor.FromArgb(84, 110, 122));
                public static readonly PaletteSwatch BlueGrey700 = new("Blue Grey 700", SColor.FromArgb(69, 90, 100));
                public static readonly PaletteSwatch BlueGrey800 = new("Blue Grey 800", SColor.FromArgb(55, 71, 79));
                public static readonly PaletteSwatch BlueGrey900 = new("Blue Grey 900", SColor.FromArgb(38, 50, 56));
            }

            public static readonly Dictionary<string, List<PaletteSwatch>> All = new(){
                { "Red", new List<PaletteSwatch> { Red.Red50, Red.Red100, Red.Red200, Red.Red300, Red.Red400, Red.Red500, Red.Red600, Red.Red700, Red.Red800, Red.Red900, Red.RedA100, Red.RedA200,Red.RedA400, Red.RedA700 }},
                { "Pink", new List<PaletteSwatch> { Pink.Pink50, Pink.Pink100, Pink.Pink200, Pink.Pink300, Pink.Pink400, Pink.Pink500, Pink.Pink600, Pink.Pink700, Pink.Pink800, Pink.Pink900, Pink.PinkA100, Pink.PinkA200, Pink.PinkA400, Pink.PinkA700 }},
                { "Purple", new List<PaletteSwatch> { Purple.Purple50, Purple.Purple100, Purple.Purple200, Purple.Purple300, Purple.Purple400, Purple.Purple500, Purple.Purple600, Purple.Purple700, Purple.Purple800, Purple.Purple900, Purple.PurpleA100, Purple.PurpleA200, Purple.PurpleA400, Purple.PurpleA700 }},
                { "Deep Purple", new List<PaletteSwatch> { DeepPurple.DeepPurple50, DeepPurple.DeepPurple100, DeepPurple.DeepPurple200, DeepPurple.DeepPurple300, DeepPurple.DeepPurple400, DeepPurple.DeepPurple500, DeepPurple.DeepPurple600, DeepPurple.DeepPurple700, DeepPurple.DeepPurple800, DeepPurple.DeepPurple900, DeepPurple.DeepPurpleA100, DeepPurple.DeepPurpleA200, DeepPurple.DeepPurpleA400, DeepPurple.DeepPurpleA700 }},
                { "Indigo", new List<PaletteSwatch> { Indigo.Indigo50, Indigo.Indigo100, Indigo.Indigo200, Indigo.Indigo300, Indigo.Indigo400, Indigo.Indigo500, Indigo.Indigo600, Indigo.Indigo700, Indigo.Indigo800, Indigo.Indigo900, Indigo.IndigoA100, Indigo.IndigoA200, Indigo.IndigoA400, Indigo.IndigoA700 }},
                { "Blue", new List<PaletteSwatch> { Blue.Blue50, Blue.Blue100, Blue.Blue200, Blue.Blue300, Blue.Blue400, Blue.Blue500, Blue.Blue600, Blue.Blue700, Blue.Blue800, Blue.Blue900, Blue.BlueA100, Blue.BlueA200, Blue.BlueA400, Blue.BlueA700 }},
                { "Light Blue", new List<PaletteSwatch> { LightBlue.LightBlue50, LightBlue.LightBlue100, LightBlue.LightBlue200, LightBlue.LightBlue300, LightBlue.LightBlue400, LightBlue.LightBlue500, LightBlue.LightBlue600, LightBlue.LightBlue700, LightBlue.LightBlue800, LightBlue.LightBlue900, LightBlue.LightBlueA100, LightBlue.LightBlueA200, LightBlue.LightBlueA400, LightBlue.LightBlueA700 }},
                { "Cyan", new List<PaletteSwatch> { Cyan.Cyan50, Cyan.Cyan100, Cyan.Cyan200, Cyan.Cyan300, Cyan.Cyan400, Cyan.Cyan500, Cyan.Cyan600, Cyan.Cyan700, Cyan.Cyan800, Cyan.Cyan900, Cyan.CyanA100, Cyan.CyanA200, Cyan.CyanA400, Cyan.CyanA700 }},
                { "Teal", new List<PaletteSwatch> { Teal.Teal50, Teal.Teal100, Teal.Teal200, Teal.Teal300, Teal.Teal400, Teal.Teal500, Teal.Teal600, Teal.Teal700, Teal.Teal800, Teal.Teal900, Teal.TealA100, Teal.TealA200, Teal.TealA400, Teal.TealA700 }},
                { "Green", new List<PaletteSwatch> { Green.Green50, Green.Green100, Green.Green200, Green.Green300, Green.Green400, Green.Green500, Green.Green600, Green.Green700, Green.Green800, Green.Green900, Green.GreenA100, Green.GreenA200, Green.GreenA400, Green.GreenA700 }},
                { "Light Green", new List<PaletteSwatch> { LightGreen.LightGreen50, LightGreen.LightGreen100, LightGreen.LightGreen200, LightGreen.LightGreen300, LightGreen.LightGreen400, LightGreen.LightGreen500, LightGreen.LightGreen600, LightGreen.LightGreen700, LightGreen.LightGreen800, LightGreen.LightGreen900, LightGreen.LightGreenA100, LightGreen.LightGreenA200, LightGreen.LightGreenA400, LightGreen.LightGreenA700 }},
                { "Lime", new List<PaletteSwatch> { Lime.Lime50, Lime.Lime100, Lime.Lime200, Lime.Lime300, Lime.Lime400, Lime.Lime500, Lime.Lime600, Lime.Lime700, Lime.Lime800, Lime.Lime900, Lime.LimeA100, Lime.LimeA200, Lime.LimeA400, Lime.LimeA700 }},
                { "Yellow", new List<PaletteSwatch> { Yellow.Yellow50, Yellow.Yellow100, Yellow.Yellow200, Yellow.Yellow300, Yellow.Yellow400, Yellow.Yellow500, Yellow.Yellow600, Yellow.Yellow700, Yellow.Yellow800, Yellow.Yellow900, Yellow.YellowA100, Yellow.YellowA200, Yellow.YellowA400, Yellow.YellowA700 }},
                { "Amber", new List<PaletteSwatch> { Amber.Amber50, Amber.Amber100, Amber.Amber200, Amber.Amber300, Amber.Amber400, Amber.Amber500, Amber.Amber600, Amber.Amber700, Amber.Amber800, Amber.Amber900, Amber.AmberA100, Amber.AmberA200, Amber.AmberA400, Amber.AmberA700 }},
                { "Orange", new List<PaletteSwatch> { Orange.Orange50, Orange.Orange100, Orange.Orange200, Orange.Orange300, Orange.Orange400, Orange.Orange500, Orange.Orange600, Orange.Orange700, Orange.Orange800, Orange.Orange900, Orange.OrangeA100, Orange.OrangeA200, Orange.OrangeA400, Orange.OrangeA700 }},
                { "Deep Orange", new List<PaletteSwatch> { DeepOrange.DeepOrange50, DeepOrange.DeepOrange100, DeepOrange.DeepOrange200, DeepOrange.DeepOrange300, DeepOrange.DeepOrange400, DeepOrange.DeepOrange500, DeepOrange.DeepOrange600, DeepOrange.DeepOrange700, DeepOrange.DeepOrange800, DeepOrange.DeepOrange900, DeepOrange.DeepOrangeA100, DeepOrange.DeepOrangeA200, DeepOrange.DeepOrangeA400, DeepOrange.DeepOrangeA700 }},
                { "Brown", new List<PaletteSwatch> { Brown.Brown50, Brown.Brown100, Brown.Brown200, Brown.Brown300, Brown.Brown400, Brown.Brown500, Brown.Brown600, Brown.Brown700, Brown.Brown800, Brown.Brown900 }},
                { "Grey", new List<PaletteSwatch> { Grey.Grey50, Grey.Grey100, Grey.Grey200, Grey.Grey300, Grey.Grey400, Grey.Grey500, Grey.Grey600, Grey.Grey700, Grey.Grey800, Grey.Grey900 }},
                { "Blue Grey", new List<PaletteSwatch> { BlueGrey.BlueGrey50, BlueGrey.BlueGrey100, BlueGrey.BlueGrey200, BlueGrey.BlueGrey300, BlueGrey.BlueGrey400, BlueGrey.BlueGrey500, BlueGrey.BlueGrey600, BlueGrey.BlueGrey700, BlueGrey.BlueGrey800, BlueGrey.BlueGrey900 }},
            };

        }
    }
}


