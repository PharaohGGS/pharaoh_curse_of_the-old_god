using Pharaoh.Gameplay.Components;
using Pharaoh.Gameplay.Components.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Target Hit GameEvent", menuName = "GameEvents/Custom/Target Hit", order = 52)]
public class TargetHitGameEvent : AbstractGameEvent<HealthComponent> { }
