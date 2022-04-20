using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkillsData", menuName = "Data/PlayerSkills")]
public class PlayerSkills : ScriptableObject
{
    public bool hasDash;
    public bool hasGrapplingHook;
    public bool hasSwarmDash;
    public bool hasSandSoldier;
    public bool hasHeart;
}
