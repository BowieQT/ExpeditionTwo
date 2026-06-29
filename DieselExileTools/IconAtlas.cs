using ImGuiNET;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SVector2 = System.Numerics.Vector2;
using RectangleF = ExileCore2.Shared.RectangleF;
using ExileCore2;
using Graphics = ExileCore2.Graphics;
using Image = SixLabors.ImageSharp.Image;

namespace DieselExileTools.ExileCore2;
public static partial class DXT
{
    public class IconAtlas {
        public string Name { get; }
        public string FilePath { get; }
        public nint TextureId { get; }
        public SVector2 IconSize { get; }
        public SVector2 AtlasSize { get; }
        public int IconsPerRow { get; }
        public int IconsPerColumn { get; }
        public int TotalIcons { get; }
        public IconAtlas(Graphics graphics, string name, string filePath, SVector2 iconSize) {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));

            if (File.Exists(filePath)) {

                using (var image = Image.Load<Rgba32>(filePath)) {
                    AtlasSize = new(image.Width, image.Height);
                }

                if (graphics.InitImage(name, filePath)) {
                    TextureId = graphics.GetTextureId(name);
                    DBug.Log($"IconAtlas: {name}, Size: {AtlasSize}, Loaded from: {filePath}", false);
                }
                else {
                    throw new Exception("Failed to initialize image.");
                }
            }
            else {
                throw new FileNotFoundException($"Texture file not found: {filePath}");
            }

            Name = name;
            FilePath = filePath;
            IconSize = iconSize;
            IconsPerRow = (int)(AtlasSize.X / iconSize.X);
            IconsPerColumn = (int)(AtlasSize.Y / IconSize.Y);
            TotalIcons = IconsPerRow * IconsPerColumn;
        }
        public RectangleF GetIconUV(int iconIndex) {
            float x = iconIndex % IconsPerRow * IconSize.X / AtlasSize.X;
            float y = iconIndex / IconsPerRow * IconSize.Y / AtlasSize.Y;
            return new RectangleF(x, y, IconSize.X / AtlasSize.X, IconSize.Y / AtlasSize.Y);
        }
        public (SVector2 uv0, SVector2 uv1) GetIconUVs(int iconIndex) {
            int x = iconIndex % IconsPerRow;
            int y = iconIndex / IconsPerRow;

            float u0 = x * IconSize.X / AtlasSize.X;
            float v0 = y * IconSize.Y / AtlasSize.Y;
            float u1 = (x + 1) * IconSize.X / AtlasSize.X;
            float v1 = (y + 1) * IconSize.Y / AtlasSize.Y;

            SVector2 uv0 = new SVector2(u0, v0);
            SVector2 uv1 = new SVector2(u1, v1);
            return (uv0, uv1);
        }

    }
}
