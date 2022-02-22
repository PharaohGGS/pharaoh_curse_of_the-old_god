using System.Linq;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInOverlap : ActionNode
    {
        [SerializeField] private Collider[] colliders;

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

            colliders = new Collider[8];

            if (!agent.TryGetComponent(out _collider))
            {
                LogHandler.SendMessage($"No collider on this agent.", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            int size = 0;
            Vector3 center = Vector3.zero;
            switch (_collider)
            {
                case BoxCollider box:
                    center = box.transform.TransformPoint(box.center);
                    size = Physics.OverlapBoxNonAlloc(
                        center, box.size / 2, colliders, box.transform.rotation,
                        _detection.detectionLayer);
                    break;
                case SphereCollider sphere:
                    center = sphere.transform.TransformPoint(sphere.center);
                    size = Physics.OverlapSphereNonAlloc(
                        center, sphere.radius, colliders,
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
                        colliders, _detection.detectionLayer);
                    break;
                default:
                    break;
            }
            
            int index = 0;
            if (colliders[0] && colliders[0].transform == agent.transform)
            {
                index++;
            }

            if (size <= index)
            {
                blackboard.ClearData("target");
                state = NodeState.Running;
                return state;
            }
            
            blackboard.SetData("target", colliders[index].transform);
            state = NodeState.Success;
            return state;
        }
    }
}