namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    static public class Sprite_Ext
    {
        /// <summary> Returns the texture used by this sprite. </summary>
        static public Texture2D Texture(this Sprite t)
        => t.texture;
        /// <summary> Returns this sprite's texture's size in pixels. </summary>
        static public Vector2 TextureSizePixels(this Sprite t)
        => new Vector2(t.Texture().width, t.Texture().height);
        /// <summary> Returns this sprite's texture's size in units. </summary>
        static public Vector2 TextureSizeUnits(this Sprite t)
        => t.TextureSizePixels() / t.pixelsPerUnit;
        /// <summary> Returns this sprite's size in pixels. </summary>
        static public Vector2 SizePixels(this Sprite t)
        => new Vector2(t.rect.width, t.rect.height);
        /// <summary> Returns this sprite's size in units. </summary>
        static public Vector2 SizeUnits(this Sprite t)
        => t.SizePixels() / t.pixelsPerUnit;
        /// <summary> Returns this sprite's relative size on the texture. </summary>
        static public Vector2 Size01(this Sprite t)
        => t.SizePixels() / t.TextureSizePixels();
        /// <summary> Returns this sprite's offset in pixels. </summary>
        static public Vector2 OffsetPixels(this Sprite t)
        => new Vector2(t.rect.min.x, t.rect.min.y);
        /// <summary> Returns this sprite's offset in units. </summary>
        static public Vector2 OffsetUnits(this Sprite t)
        => t.OffsetPixels() / t.pixelsPerUnit;
        /// <summary> Returns this sprite's relative offset on the texture. </summary>
        static public Vector2 Offset01(this Sprite t)
        => t.OffsetPixels() / t.TextureSizePixels();
        /// <summary> Returns this sprite's size and offset in pixels. </summary>
        static public Vector4 SizeOffsetPixels(this Sprite t)
        => t.SizePixels().Append(t.OffsetPixels());
        /// <summary> Returns this sprite's size and offset in units. </summary>
        static public Vector4 SizeOffsetUnits(this Sprite t)
        => t.SizeUnits().Append(t.OffsetUnits());
        /// <summary> Returns this sprite's relative size and offset on the texture. </summary>
        static public Vector4 SizeOffset01(this Sprite t)
        => t.Size01().Append(t.Offset01());
        /// <summary> Returns this sprite's rect in pixels. </summary>
        static public Rect RectPixels(this Sprite t)
        => t.rect;
    }
}