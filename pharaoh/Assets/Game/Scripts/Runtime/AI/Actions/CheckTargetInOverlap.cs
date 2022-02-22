using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInOverlap : ActionNode
    {
        [SerializeField] private Collider[] _detectedColliders;

        private Collider _collider;
        private DetectionComponent _detection;

        protected override void OnStart()
        {
            if (_detection) return;

            if (!agent.TryGetComponent(out _detection))
            {
                LogHandler.SendMessage($"No detection possible with this agent.", MessageType.Warning);
            }

            if (_collider) return;

            _detectedColliders = new Collider[8];

            if (!agent.TryGetComponent(out _collider))
            {
                LogHandler.SendMessage($"No collider on this agent.", MessageType.Warning);
            }

            LogHandler.SendMessage($"{_collider.name}", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            int size = 0;
            switch (_collider)
            {
                case BoxCollider box:
                    size = Physics.OverlapBoxNonAlloc(
                        box.center, box.size / 2, _detectedColliders, box.transform.rotation,
                        _detection.detectionLayer);
                    
                    Debug.DrawLine(box.center, box.center + box.size / 2, Color.red);
                    Debug.DrawLine(box.center, box.center - box.size / 2, Color.red);
                    break;
                case SphereCollider sphere:
                    size = Physics.OverlapSphereNonAlloc(
                        sphere.center, sphere.radius, _detectedColliders,
                        _detection.detectionLayer);
                    break;
                case CapsuleCollider capsule:
                    ///* https://roundwide.com/physics-overlap-capsule/ *///
                    var direction = new Vector3 { [capsule.direction] = 1 };
                    var offset = capsule.height / 2 - capsule.radius;
                    var point0 = capsule.transform.TransformPoint(capsule.center - direction * offset);
                    var point1 = capsule.transform.TransformPoint(capsule.center + direction * offset);
                    var r = capsule.transform.TransformVector(capsule.radius, capsule.radius, capsule.radius);
                    var radius = Enumerable.Range(0, 3).Select(xyz => xyz == capsule.direction ? 0 : r[xyz])
                        .Select(Mathf.Abs).Max();
                    size = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, 
                        _detectedColliders, _detection.detectionLayer);
                    break;
                default:
                    break;
            }

            state = size > 0 ? NodeState.Success : NodeState.Failure;
            return state;
        }
    }
}