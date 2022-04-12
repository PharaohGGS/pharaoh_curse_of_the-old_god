using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Playground.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed = 6;
        [SerializeField] private float turnSpeed = 2;

        [SerializeField] private Animator animator;
        private Rigidbody _rigidbody;
        private PlayerInputAsset _playerInputAsset;
        private Vector2 _moveInput, _turnInput;

        private void Awake()
        {
            _playerInputAsset = new PlayerInputAsset();
            if (!TryGetComponent(out _rigidbody))
            {
                Debug.Log($"{gameObject.name}: Failed to find Rigidbody");
            }
        }

        private void OnEnable()
        {
            _playerInputAsset.Player.Enable();
        }

        void FixedUpdate()
        {
            _moveInput = _playerInputAsset.Player.Move.ReadValue<Vector2>();

            Transform playerTransform = transform;
            _rigidbody.velocity = (playerTransform.forward * _moveInput.y + playerTransform.right * _moveInput.x).normalized * speed;
            
            
            float forward = _moveInput.y;
            float right = _moveInput.x;
            
            //transform.Translate(new Vector3(right, 0, forward) * speed * Time.deltaTime);

            _turnInput = _playerInputAsset.Player.Look.ReadValue<Vector2>();
            float turn = _turnInput.x;
            
            transform.rotation *= Quaternion.Slerp(Quaternion.identity,
                Quaternion.LookRotation(turn < 0 ? Vector3.left : Vector3.right),
                Mathf.Abs(turn) * turnSpeed * Time.deltaTime);
            
            animator.SetFloat("Forward", forward);
            animator.SetFloat("Right", right);
            
            
        }
    }
}
