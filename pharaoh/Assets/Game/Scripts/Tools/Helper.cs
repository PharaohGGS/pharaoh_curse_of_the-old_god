using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pharaoh.Tools
{
    public static class Helper
    {
        public static bool IsSharingSameInstance(this UnityEngine.MonoBehaviour objectToCompare,
            UnityEngine.GameObject comparisonObject)
        {
            return objectToCompare.gameObject.GetInstanceID() == comparisonObject.GetInstanceID();
        }

        public static void LookAt2D(this Transform transform, Transform target)
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            Vector2 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        public static bool IsInLayerMask(this GameObject go, LayerMask mask) => (mask.value & (1 << go.layer)) > 0;

        public static int OverlapNonAlloc(this Collider collider, ref Collider[] colliders, LayerMask layerMask)
        {
            int size = 0;
            Vector3 center = Vector3.zero;
            switch (collider)
            {
                case BoxCollider box:
                    center = box.transform.TransformPoint(box.center);
                    size = Physics.OverlapBoxNonAlloc(
                        center, box.size / 2, colliders, box.transform.rotation,
                        layerMask);
                    break;
                case SphereCollider sphere:
                    center = sphere.transform.TransformPoint(sphere.center);
                    size = Physics.OverlapSphereNonAlloc(
                        center, sphere.radius, colliders,
                        layerMask);
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
                    size = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, colliders, layerMask);
                    break;
                default:
                    throw new NotImplementedException("Not implemented overlap non alloc method for this collider");
            }

            return size;
        }

        public static int OverlapNonAlloc(this Collider2D collider, ref Collider2D[] colliders, LayerMask layerMask)
        {
            int size = 0;
            Vector2 center = Vector2.zero;
            switch (collider)
            {
                case BoxCollider2D box:
                    center = box.transform.TransformPoint(box.offset);
                    size = Physics2D.OverlapBoxNonAlloc(
                        center, box.size / 2, box.transform.rotation.x, colliders,
                        layerMask);
                    break;
                case CircleCollider2D sphere:
                    center = sphere.transform.TransformPoint(sphere.offset);
                    size = Physics2D.OverlapCircleNonAlloc(
                        center, sphere.radius, colliders,
                        layerMask);
                    break;
                case CapsuleCollider2D capsule:
                    var point = capsule.transform.TransformPoint(capsule.offset);
                    size = Physics2D.OverlapCapsuleNonAlloc(point, capsule.size,
                        capsule.direction, capsule.transform.rotation.x,
                        colliders, layerMask);
                    break;
                default:
                    throw new NotImplementedException("Not implemented overlap non alloc method for this collider");
            }

            return size;
        }
    }
}