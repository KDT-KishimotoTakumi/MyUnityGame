using UnityEngine;

namespace MyUnityGame
{
    /// <summary>Presentation only: items gently breathe so they read as living sparks of colour. Position never changes.</summary>
    public sealed class ItemPulse : MonoBehaviour
    {
        [SerializeField] private float amount = 0.18f;
        [SerializeField] private float speed = 3f;

        private Vector3 baseScale;
        private float phase;

        private void Awake()
        {
            baseScale = transform.localScale;
            phase = transform.position.z;
        }

        private void Update()
        {
            transform.localScale = baseScale * (1f + amount * Mathf.Sin(Time.time * speed + phase));
        }
    }
}
