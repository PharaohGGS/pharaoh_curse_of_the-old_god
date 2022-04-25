using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public abstract class CharacterMovement : MonoBehaviour
    {
        [SerializeField] protected Collider2D _collider, _ignoreCollider;

        protected virtual void Start()
        {
            Physics2D.IgnoreCollision(_collider, _ignoreCollider, true);
        }
    }
}