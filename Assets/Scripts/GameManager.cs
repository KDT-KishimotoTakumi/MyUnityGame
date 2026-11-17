using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUnityGame
{
    /// <summary>Owns the BallCollect rules: collection, clear state and the HUD. Mirrors state into serialized fields for CLI observation.</summary>
    public sealed class GameManager : MonoBehaviour
    {
        private const int TotalItems = 5;

        [SerializeField] private PlayerController player;
        [SerializeField] private CollectibleItem[] items;
        [SerializeField] private HudView hud;
        [SerializeField] private InputActionAsset inputActions;

        [Header("Observed state (read-only, written by the game)")]
        [SerializeField] private int collectedCount;
        [SerializeField] private bool isCleared;

        private readonly CollectionState state = new CollectionState(TotalItems);

        public int CollectedCount => collectedCount;

        public bool IsCleared => isCleared;

        /// <summary>Raised whenever the observed state is refreshed: (collected, total, cleared).</summary>
        public event System.Action<int, int, bool> StateChanged;

        [SerializeField] private float fallResetHeight = -5f;

        private InputAction restartAction;

        private void OnEnable()
        {
            restartAction = inputActions.FindAction("Gameplay/Restart", true);
            restartAction.Enable();
        }

        private void OnDisable()
        {
            restartAction?.Disable();
        }

        private void Start()
        {
            SyncState();
        }

        private void Update()
        {
            if (restartAction.WasPressedThisFrame() || player.transform.position.y < fallResetHeight)
            {
                ResetGame();
            }
        }

        /// <summary>Returns everything to the start state: ball position and velocity, count, all items, and the HUD.</summary>
        public void ResetGame()
        {
            player.ResetToStart();
            state.Reset();
            foreach (CollectibleItem item in items)
            {
                item.gameObject.SetActive(true);
            }

            SyncState();
        }

        /// <summary>Counts the item once. Returns false for an unknown or already collected id.</summary>
        public bool TryCollect(int itemId)
        {
            if (!state.TryCollect(itemId))
            {
                return false;
            }

            SyncState();
            return true;
        }

        private void SyncState()
        {
            collectedCount = state.CollectedCount;
            isCleared = state.IsCleared;
            hud.Refresh(collectedCount, state.TotalCount, isCleared);
            StateChanged?.Invoke(collectedCount, state.TotalCount, isCleared);
        }
    }
}
