using Pharaoh.Gameplay.Components;
using Pharaoh.Gameplay.Components.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trigger Hit GameEvent", menuName = "GameEvents/Custom/Trigger Hit", order = 52)]
public class TriggerHitGameEvent : AbstractGameEvent<Damager> { }
