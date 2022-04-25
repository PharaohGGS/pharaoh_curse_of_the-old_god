using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSkillsData", menuName = "Data/PlayerSkills")]
public class PlayerSkills : ScriptableObject
{
    public UnityAction onChange;

    public bool hasDash;
    public bool hasGrapplingHook;
    public bool hasSwarmDash;
    public bool hasSandSoldier;
    public bool hasHeart;

    public bool HasDash
    { get => hasDash; set { hasDash = value; onChange?.Invoke(); } }
    public bool HasGrapplingHook
    { get => hasGrapplingHook; set { hasGrapplingHook = value; onChange?.Invoke(); } }
    public bool HasSwarmDash
    { get => hasSwarmDash; set { hasSwarmDash = value; onChange?.Invoke(); } }
    public bool HasSandSoldier
    { get => hasSandSoldier; set { hasSandSoldier = value; onChange?.Invoke(); } }
    public bool HasHeart
    { get => hasHeart; set { hasHeart = value; onChange?.Invoke(); } }

    public void Reset()
    {
        hasDash = false;
        hasGrapplingHook = false;
        hasSwarmDash = false;
        hasSandSoldier = false;
        hasHeart = false;
    }
}
