using UnityEngine;

namespace MyUnityGame
{
    /// <summary>Reports a player touch to the GameManager and hides itself once counted.</summary>
    public sealed class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private int itemId;
        [SerializeField] private GameManager gameManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && gameManager.TryCollect(itemId))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
