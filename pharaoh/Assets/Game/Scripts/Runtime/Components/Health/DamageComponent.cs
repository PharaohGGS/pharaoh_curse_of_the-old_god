using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class DamageComponent : MonoBehaviour
    {
        [field: SerializeField, Range(0.25f, 20f)] public float attackRate { get; private set; } = 0.5f;
        [field: SerializeField, Range(1, 100)] public float value { get; private set; } = 10;
    }
}
