using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class SpikeTrapBehaviour : TrapBehaviour<MeleeTrapData>
    {
        [SerializeField] private GameObject mesh;
        [SerializeField] private Transform showingTransform;
        [SerializeField] private Transform hidingTransform;

        private Collider2D _col;
        private Rigidbody2D _rb;
        
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private bool _isFirstTime = false;

        protected void Awake()
        {
            // at start hide gear
            if (TryGetComponent(out _col)) _col.enabled = false;
            if (TryGetComponent(out _rb) && hidingTransform) Respawn();
        }

        public override void Activate(GameObject target)
        {
            bool isSameTarget = _currentTarget == target;
            if (!isSameTarget) _currentTarget = target;
            bool addDelay = _isFirstTime || !isSameTarget || !data.oneTimeDelay;
            if (_isFirstTime) _isFirstTime = false;
            StartCoroutine(Action(addDelay));
        }

        public override void Respawn()
        {
            if (data.oneTimeDelay)
            {
                _isFirstTime = true;
                return;
            }

            StartCoroutine(Move(data.hidingSpeed * 100f, _rb.position, hidingTransform.position));
            if (_col) _col.enabled = false;
            mesh?.SetActive(false);
        }

        private IEnumerator Action(bool addDelay)
        {
            var delay = new WaitForSeconds(data.delay);
            var lifeTime = new WaitForSeconds(data.lifeTime);
            var timeOut = new WaitForSeconds(data.timeOut);

            var show = Move(data.showingSpeed, hidingTransform.position, showingTransform.position);
            var hide = Move(data.hidingSpeed, showingTransform.position, hidingTransform.position);

            isStarted = true;

            // wait a delay before activate the trap
            if (addDelay) yield return delay;
            
            // when showing, activate collider
            if (_col) _col.enabled = true;
            mesh?.SetActive(true);
            yield return StartCoroutine(show);

            // after showing wait some lifeTime
            yield return lifeTime; 

            // when hiding, disable collider after finish hiding
            yield return StartCoroutine(hide);
            mesh?.SetActive(false);
            if (_col) _col.enabled = false; 

            // timeOut after hiding
            yield return timeOut;
            isStarted = false;
        }

        private IEnumerator Move(float speed, Vector2 start, Vector2 end)
        {
            if (!_rb) yield break;

            float currentTime = 0f;
            var curve = data.curve;
            
            float maxDistance = Vector2.Distance(start, end);
            float duration = maxDistance / speed;
            Vector2 nextPosition = start;

            // movement
            while (currentTime < duration)
            {
                nextPosition = Vector2.Lerp(start, end, curve.Evaluate(currentTime / duration));
                currentTime = Mathf.MoveTowards(currentTime, duration, Time.fixedDeltaTime * speed);
                _rb.MovePosition(nextPosition);
                yield return _waitForFixedUpdate;
            }

            nextPosition = Vector2.Lerp(start, end, curve.Evaluate(currentTime / duration)); 
            _rb.MovePosition(nextPosition);
        }
    }
}