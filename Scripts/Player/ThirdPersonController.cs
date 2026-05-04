using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace DomeCraft.Player
{
    /// <summary>
    /// Modular third-person character controller built on top of Unity's
    /// <see cref="CharacterController"/> component.
    ///
    /// Networked movement is delegated to a <see cref="NetworkTransform"/>
    /// component on the same GameObject, which handles position/rotation
    /// synchronisation across clients automatically.
    ///
    /// Only the owner (the player who controls this character) processes input
    /// and drives the <see cref="CharacterController"/>; all other clients
    /// receive state via <see cref="NetworkTransform"/>.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    public class ThirdPersonController : NetworkBehaviour
    {
        // ─── Inspector ──────────────────────────────────────────────────────────

        /// <summary>Movement speed in metres per second.</summary>
        [Tooltip("Movement speed in metres per second.")]
        [SerializeField] private float moveSpeed = 5f;

        /// <summary>Rotational smoothing speed (degrees per second).</summary>
        [Tooltip("Rotational smoothing speed in degrees per second.")]
        [SerializeField] private float rotationSpeed = 720f;

        /// <summary>Downward gravitational acceleration (m/s²).</summary>
        [Tooltip("Gravitational pull applied each frame.")]
        [SerializeField] private float gravity = -9.81f;

        /// <summary>
        /// Optional camera Transform used to align movement direction.
        /// If null, movement is calculated in world space.
        /// </summary>
        [Tooltip("Assign the player camera's Transform for camera-relative movement.")]
        [SerializeField] private Transform cameraTransform;

        // ─── Private Fields ──────────────────────────────────────────────────────

        private CharacterController _characterController;
        private Vector3 _verticalVelocity;

        // ─── Unity Lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Only the owning client processes input.
            if (!IsOwner) return;

            HandleMovement();
            ApplyGravity();
        }

        // ─── Movement ───────────────────────────────────────────────────────────

        /// <summary>
        /// Reads axis input, calculates a camera-relative direction, and moves
        /// the <see cref="CharacterController"/> accordingly.
        /// </summary>
        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical   = Input.GetAxis("Vertical");

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDirection.magnitude < 0.01f) return;

            // Translate input into camera-relative world space when a camera is assigned.
            Vector3 moveDirection = TransformInputDirection(inputDirection);

            // Rotate the character to face the movement direction.
            RotateTowards(moveDirection);

            // Move via the CharacterController (authoritative on owner; NetworkTransform replicates).
            _characterController.Move(moveDirection * (moveSpeed * Time.deltaTime));
        }

        /// <summary>
        /// Converts a raw input direction into a camera-relative world-space direction.
        /// Falls back to world space when no camera is assigned.
        /// </summary>
        /// <param name="inputDirection">Raw, normalised input direction in local space.</param>
        /// <returns>World-space direction aligned with the camera's forward axis.</returns>
        private Vector3 TransformInputDirection(Vector3 inputDirection)
        {
            if (cameraTransform == null) return inputDirection;

            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight   = cameraTransform.right;

            // Project onto the horizontal plane.
            cameraForward.y = 0f;
            cameraRight.y   = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            return (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
        }

        /// <summary>
        /// Smoothly rotates the character to face <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">World-space target direction.</param>
        private void RotateTowards(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        /// <summary>
        /// Accumulates downward velocity and applies gravity via the
        /// <see cref="CharacterController"/> each frame.
        /// Resets vertical velocity when the character is grounded.
        /// </summary>
        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity.y < 0f)
            {
                // Small negative value keeps the controller grounded on slopes.
                _verticalVelocity.y = -2f;
            }

            _verticalVelocity.y += gravity * Time.deltaTime;
            _characterController.Move(_verticalVelocity * Time.deltaTime);
        }

        // ─── Public API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Assigns (or clears) the camera Transform used for camera-relative movement.
        /// Call this after a camera is instantiated for this player.
        /// </summary>
        /// <param name="camera">The camera Transform to track, or null to use world space.</param>
        public void SetCameraTransform(Transform camera)
        {
            cameraTransform = camera;
        }
    }
}
