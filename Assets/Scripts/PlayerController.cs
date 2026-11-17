using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUnityGame
{
    /// <summary>Moves the player ball with the Gameplay/Move action and restores its start state.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private float moveForce = 10f;
        [SerializeField] private Vector3 startPosition = new Vector3(0f, 0.5f, -7f);

        private Rigidbody rb;
        private InputAction moveAction;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            moveAction = inputActions.FindAction("Gameplay/Move", true);
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
        }

        private void FixedUpdate()
        {
            Vector2 move = moveAction.ReadValue<Vector2>();
            rb.AddForce(new Vector3(move.x, 0f, move.y) * moveForce);
        }

        /// <summary>Puts the ball back at the start position with no linear or angular velocity.</summary>
        public void ResetToStart()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = startPosition;
            rb.rotation = Quaternion.identity;
            transform.SetPositionAndRotation(startPosition, Quaternion.identity);
        }
    }
}
