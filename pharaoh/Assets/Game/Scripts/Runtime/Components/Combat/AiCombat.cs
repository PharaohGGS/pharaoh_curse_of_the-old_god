using System;
using System.Collections.Generic;
using System.Linq;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;
using AudioManager = Pharaoh.Managers.AudioManager;

namespace Pharaoh.Gameplay.Components
{
    public class AiCombat : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Gear[] gears;

        private int _weaponIndex = -1;
        private readonly List<Gear> _weapons = new List<Gear>();
        
        private Transform _currentTarget;
        private HealthComponent _targetHealth;

        public Gear activeWeapon => _weapons.Count >= 1 ? _weapons[_weaponIndex] : null;
        public bool hasTarget => _currentTarget != null;
        
        private void Awake()
        {
            if (gears.Length <= 0)
            {
                gears = GetComponentsInChildren<Gear>();
            }

            foreach (var gear in gears) if (gear is IWeapon) _weapons.Add(gear);
            if (_weapons.Count <= 0) return;
            _weaponIndex = 0;

            if (!animator)
            {
                LogHandler.SendMessage("No animator on this ai", MessageType.Warning);
            }
        }

        public void ChangeWeapon()
        {
            if (_weapons.Count <= 1) return;
            _weaponIndex = (_weaponIndex + 1) % _weapons.Count;
        } 

        public bool Attack(Transform target)
        {
            if (!activeWeapon || !target) return false;

            if (_currentTarget != target)
            { 
                _currentTarget = target;
                SubscribeToTargetHealth();
            }

            if (activeWeapon is not IWeapon weapon) return false;

            switch (activeWeapon)
            {
                case DistanceGear distanceGear:
                    return DistanceAttack(distanceGear);
                case MeleeGear meleeGear:
                    return MeleeAttack(meleeGear);
                default:
                    throw new ArgumentOutOfRangeException(nameof(activeWeapon));
            }
        }

        private void SubscribeToTargetHealth()
        {
            if (!_currentTarget.TryGetComponent(out HealthComponent health) || health == _targetHealth)
            {
                return;
            }   
            
            _targetHealth?.onDeath?.RemoveListener(OnTargetDeath);
            _targetHealth = health;
            _targetHealth?.onDeath?.AddListener(OnTargetDeath);
        }

        private void OnTargetDeath(HealthComponent targetHealth)
        {
            _targetHealth?.onDeath?.RemoveListener(OnTargetDeath);
            _currentTarget = null;
            _targetHealth = null;
        }

        private bool MeleeAttack(MeleeGear gear)
        {
            if (!_currentTarget)
            {
                LogHandler.SendMessage($"{name} can't attack nothing", MessageType.Warning);
                return false;
            }

            var data = gear.GetData();
            var distance = Mathf.Abs(_currentTarget.position.x - transform.position.x);

            var canThrow = data.throwable;
            var isThrowable = distance <= data.throwableRange;
            var isStabbable = distance <= data.range;
            
            switch (canThrow)
            {
                case false when isStabbable:
                    // do stabbing animation
                    animator.ResetTrigger("Attacking");
                    animator.SetTrigger("Attacking");
                    LogHandler.SendMessage($"{name} stabbing {_currentTarget}", MessageType.Log);
                    AudioManager.Instance?.Play("ClawSwing");
                    return true;
                case true when isStabbable && isThrowable:
                    // do stabbing animation
                    animator.ResetTrigger("Attacking");
                    animator.SetTrigger("Attacking");
                    LogHandler.SendMessage($"{name} stabbing {_currentTarget}", MessageType.Log);
                    Debug.Log("----Play harpoon swing");
                    return true;
                case true when !isStabbable && isThrowable:
                    // do throwing animation
                    animator.ResetTrigger("Shooting");
                    animator.SetTrigger("Shooting");
                    LogHandler.SendMessage($"{name} shooting at {_currentTarget}", MessageType.Log);
                    Debug.Log("----Play harpoon throw");
                    return true;
                default:
                    LogHandler.SendMessage($"{name} is too far from {_currentTarget.name}", MessageType.Warning);
                    return false;
            }
        }
        
        private bool DistanceAttack(DistanceGear gear)
        {
            if (!_currentTarget)
            {
                LogHandler.SendMessage($"{name} can't attack nothing", MessageType.Warning);
                return false;
            }

            return true;
            // do something related to distance attack
        }

        public void Throw()
        {
            if (!_currentTarget) return;
            if (activeWeapon is not MeleeGear melee || !melee.GetData().throwable) return;
            melee.Throw(_currentTarget);
        }

        public void Shoot()
        {
            if (!_currentTarget) return;
            if (activeWeapon is not DistanceGear distance) return;
            distance.Shoot(_currentTarget);
        }
    }
}