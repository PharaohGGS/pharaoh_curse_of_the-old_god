using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSkillsData", menuName = "Data/PlayerSkills")]
public class PlayerSkills : ScriptableObject
{
    public UnityAction<bool> onChange;

    public bool hasDash;
    public bool hasGrapplingHook;
    public bool hasSwarmDash;
    public bool hasSandSoldier;
    public bool hasHeart;

    public bool HasDash
    { get => hasDash; set { hasDash = value; onChange?.Invoke(true); } }
    public bool HasGrapplingHook
    { get => hasGrapplingHook; set { hasGrapplingHook = value; onChange?.Invoke(false); } }
    public bool HasSwarmDash
    { get => hasSwarmDash; set { hasSwarmDash = value; onChange?.Invoke(false); } }
    public bool HasSandSoldier
    { get => hasSandSoldier; set { hasSandSoldier = value; onChange?.Invoke(false); } }
    public bool HasHeart
    { get => hasHeart; set { hasHeart = value; onChange?.Invoke(false); } }

    public void Reset()
    {
        hasDash = false;
        hasGrapplingHook = false;
        hasSwarmDash = false;
        hasSandSoldier = false;
        hasHeart = false;
    }
}
