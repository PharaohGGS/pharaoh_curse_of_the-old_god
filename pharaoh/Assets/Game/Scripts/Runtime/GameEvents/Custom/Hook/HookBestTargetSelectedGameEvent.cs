using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook Best Target Selected GameEvent", menuName = "GameEvents/Custom/Hook/BestTargetSelect",
        order = 52)]
    public class HookBestTargetSelectedGameEvent : AbstractGameEvent<Pharaoh.Gameplay.HookTargeting, GameObject>
    {
    }
}