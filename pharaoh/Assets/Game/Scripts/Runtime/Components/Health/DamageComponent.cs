using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class DamageComponent : MonoBehaviour
    {
        [field: SerializeField] public float Damage { get; private set; }
    }
}
