using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        private CharacterController _controller;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        public float PlayerSpeed = 4f;

        [Networked] private bool IsFlipped { get; set; }
        [Networked] private float Speed { get; set; }
        [Networked] private bool IsDead { get; set; }

        private Vector2 _moveInput;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
            {
                return;
            }

            Vector3 move = new Vector3(_moveInput.x, _moveInput.y, 0) * Runner.DeltaTime * PlayerSpeed;
            _controller.Move(move);

            Speed = move.magnitude;

            if (move.x != 0)
            {
                bool newFlip = move.x < 0;
                if (IsFlipped != newFlip)
                {
                    IsFlipped = newFlip;
                    UpdateFlip();
                }
            }

            if (_moveInput != Vector2.zero)
            {
                gameObject.transform.up = Vector3.up;
            }
        }

        private void UpdateFlip()
        {
            _spriteRenderer.flipX = IsFlipped;
        }

        public override void Render()
        {
            _animator.SetFloat("Speed", Speed);
            _animator.SetBool("Dead", IsDead);
            UpdateFlip();
        }

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }
    }
}