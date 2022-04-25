using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class TrapDetection : Detection<Collider2D>
    {
        public bool TryGetByIndex(int index, out GameObject obj)
        {
            obj = GetByIndex(index);
            return obj != null;
        }

        public GameObject GetByIndex(int index) => overlappedCount <= 0 ? null : _overlapColliders[index]?.gameObject;
    }
}