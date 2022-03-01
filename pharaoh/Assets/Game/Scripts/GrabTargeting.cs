using System;
using UnityEngine.InputSystem;

namespace Pharaoh.Gameplay
{
    public class GrabTargeting : Targeting
    {
        private PlayerInput _playerInput;

        protected override void Awake()
        {
            base.Awake();
            _playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.CharacterActions.Grab.performed += OnGrab;
        }

        private void OnDisable()
        {
            _playerInput.CharacterActions.Grab.performed -= OnGrab;
            _playerInput.Disable();
        }
        private void OnGrab(InputAction.CallbackContext obj)
        {
            // unhook the current grabbed object if there is one
            if (_currentTarget) UnGrab();
            // hook the nearest grabbable objects if there is one
            if (_bestTargetLeft || _bestTargetRight) Grab();
        }

        private void UnGrab()
        {
            throw new NotImplementedException();
        }

        private void Grab()
        {
            throw new NotImplementedException();
        }
    }
}