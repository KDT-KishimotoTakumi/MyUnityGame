using UnityEngine;

namespace MyUnityGame
{
    /// <summary>Presentation only: the world starts gray and regains colour with every collected item; the ball wears the latest colour.</summary>
    public sealed class WorldColorizer : MonoBehaviour
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [SerializeField] private GameManager game;
        [SerializeField] private Renderer floor;
        [SerializeField] private Renderer player;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private Camera view;
        [SerializeField] private Renderer[] items;

        [SerializeField] private Color floorGray = new Color(0.42f, 0.42f, 0.45f);
        [SerializeField] private Color floorVivid = new Color(0.98f, 0.93f, 0.80f);
        [SerializeField] private Color skyGray = new Color(0.12f, 0.12f, 0.14f);
        [SerializeField] private Color skyVivid = new Color(0.30f, 0.62f, 0.95f);
        [SerializeField] private Color playerGray = new Color(0.85f, 0.85f, 0.88f);

        private MaterialPropertyBlock block;

        private void Awake()
        {
            block = new MaterialPropertyBlock();
            for (int i = 0; i < items.Length; i++)
            {
                Paint(items[i], ColorProgress.ItemColor(i, items.Length));
            }
        }

        private void OnEnable()
        {
            game.StateChanged += Apply;
        }

        private void OnDisable()
        {
            game.StateChanged -= Apply;
        }

        private void Apply(int collected, int total, bool cleared)
        {
            Paint(floor, ColorProgress.Tint(floorGray, floorVivid, collected, total));
            view.backgroundColor = ColorProgress.Tint(skyGray, skyVivid, collected, total);

            Color ball = collected == 0 ? playerGray : ColorProgress.ItemColor(collected - 1, total);
            Paint(player, ball);
            trail.startColor = ball;
            trail.endColor = new Color(ball.r, ball.g, ball.b, 0f);
            if (collected == 0)
            {
                trail.Clear();
            }
        }

        private void Paint(Renderer target, Color color)
        {
            target.GetPropertyBlock(block);
            block.SetColor(ColorId, color);
            target.SetPropertyBlock(block);
        }
    }
}
