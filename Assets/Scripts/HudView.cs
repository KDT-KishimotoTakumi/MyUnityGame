using UnityEngine;
using UnityEngine.UI;

namespace MyUnityGame
{
    /// <summary>Shows the collected count as "n/total" and toggles the CLEAR label.</summary>
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Text collectedText;
        [SerializeField] private GameObject clearText;

        public void Refresh(int collected, int total, bool cleared)
        {
            collectedText.text = $"{collected}/{total}";
            clearText.SetActive(cleared);
        }
    }
}
