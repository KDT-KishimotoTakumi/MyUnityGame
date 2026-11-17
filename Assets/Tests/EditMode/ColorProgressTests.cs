using NUnit.Framework;
using UnityEngine;

namespace MyUnityGame.Tests
{
    public class ColorProgressTests
    {
        [TestCase(0, 5, 0f)]
        [TestCase(1, 5, 0.2f)]
        [TestCase(5, 5, 1f)]
        [TestCase(7, 5, 1f)]
        [TestCase(-1, 5, 0f)]
        [TestCase(3, 0, 0f)]
        public void Ratio_IsClampedShareOfCollected(int collected, int total, float expected)
        {
            Assert.AreEqual(expected, ColorProgress.Ratio(collected, total), 1e-5f);
        }

        [Test]
        public void ItemColors_AreFiveDistinctVividHues()
        {
            for (int i = 0; i < 5; i++)
            {
                Color.RGBToHSV(ColorProgress.ItemColor(i, 5), out float h, out float s, out float v);
                Assert.AreEqual(i / 5f, h, 0.01f, $"hue of item {i}");
                Assert.Greater(s, 0.6f, "saturated");
                Assert.Greater(v, 0.9f, "bright");
            }
        }

        [Test]
        public void Tint_GoesFromGrayToVivid_WithProgress()
        {
            Color gray = Color.gray;
            Color vivid = Color.cyan;
            Assert.AreEqual(gray, ColorProgress.Tint(gray, vivid, 0, 5));
            Assert.AreEqual(vivid, ColorProgress.Tint(gray, vivid, 5, 5));
            Assert.AreEqual(Color.Lerp(gray, vivid, 0.4f), ColorProgress.Tint(gray, vivid, 2, 5));
        }
    }
}
