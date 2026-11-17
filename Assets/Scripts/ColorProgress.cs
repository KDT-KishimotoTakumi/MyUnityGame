using UnityEngine;

namespace MyUnityGame
{
    /// <summary>Colour rules of the "bring the colour back" presentation: each item owns a hue, the world tints with progress.</summary>
    public static class ColorProgress
    {
        /// <summary>Share of items collected, clamped to 0..1 (0 when there are no items).</summary>
        public static float Ratio(int collected, int total)
        {
            return total <= 0 ? 0f : Mathf.Clamp01((float)collected / total);
        }

        /// <summary>A vivid hue per item, evenly spaced around the colour wheel.</summary>
        public static Color ItemColor(int index, int total)
        {
            return Color.HSVToRGB((float)index / total, 0.8f, 1f);
        }

        public static Color Tint(Color gray, Color vivid, int collected, int total)
        {
            return Color.Lerp(gray, vivid, Ratio(collected, total));
        }
    }
}
