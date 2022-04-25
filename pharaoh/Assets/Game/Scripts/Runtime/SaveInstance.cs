using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.VFX;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

public class SaveInstance : MonoBehaviour
{
    public enum Type
    {
        None,
        Enemy,
        MovingBlock,
        CanopicJar,
        CanopicFire
    };

    private readonly int ENEMY_PARSE_INDEX = 6;
    private readonly int MOVING_BLOCK_PARSE_INDEX = 12;

    [HideInInspector] public ulong instanceID;
    public Type type = Type.None;
    public PlayerSkills playerSkills;

    private void Awake()
    {
        // Prevents error while debugging
        if (SaveDataManager.Instance == null)
            return;

        if (type == Type.Enemy && gameObject.name.StartsWith("Enemy_"))
            instanceID = ulong.Parse(gameObject.name.Substring(ENEMY_PARSE_INDEX));
        else if (type == Type.MovingBlock && gameObject.name.StartsWith("MovingBlock_"))
            instanceID = ulong.Parse(gameObject.name.Substring(MOVING_BLOCK_PARSE_INDEX));
        else if (type == Type.CanopicFire)
            playerSkills.onChange += OnSkillUnlocked;

        Load();
    }

    private void OnDestroy()
    {
        // Prevents error while debugging
        if (SaveDataManager.Instance == null)
            return;

        // Saves the instance when unloaded
        Save();
    }

    private void Load()
    {
        // Load this object instance from save data depending on data type
        switch (type)
        {
            case Type.Enemy:
                // Load this enemy state
                bool state;
                SaveDataManager.Instance.LoadEnemyState(instanceID, out state);
                gameObject.SetActive(state);
                break;

            case Type.MovingBlock:
                // Load this block position if previously saved, otherwise keep its original position
                Vector3 position;
                if (SaveDataManager.Instance.LoadBlockPosition(instanceID, out position))
                    gameObject.transform.position = position;
                break;

            case Type.CanopicJar:
                CanopicJarPickable pickable = GetComponent<CanopicJarPickable>();
                CanopicJarPickable.CanopicJar type = pickable.jar;
                switch (type)
                {
                    case CanopicJarPickable.CanopicJar.Monkey:
                        if (playerSkills.HasDash) pickable.Open();
                        break;

                    case CanopicJarPickable.CanopicJar.Bird:
                        if (playerSkills.HasGrapplingHook) pickable.Open();
                        break;

                    case CanopicJarPickable.CanopicJar.Dog:
                        if (playerSkills.HasSwarmDash) pickable.Open();
                        break;

                    case CanopicJarPickable.CanopicJar.Human:
                        if (playerSkills.HasSandSoldier) pickable.Open();
                        break;

                    case CanopicJarPickable.CanopicJar.Crocodile:
                        if (playerSkills.HasHeart) pickable.Open();
                        break;

                    default:
                        break;
                }
                break;

            case Type.CanopicFire:
                if (name.EndsWith(" - Dash") && playerSkills.HasDash
                    || name.EndsWith(" - GrapplingHook") && playerSkills.HasGrapplingHook
                    || name.EndsWith(" - SwarmDash") && playerSkills.HasSwarmDash
                    || name.EndsWith(" - SandSoldier") && playerSkills.HasSandSoldier)
                {
                    GetComponent<VisualEffect>().enabled = true;
                    //GetComponent<Light>().enabled = false; // currently no lights on the object
                }
                break;

            case Type.None:
            default:
                break;
        }
    }

    public void Save()
    {
        // Save this object instance to save data depending on data type
        switch (type)
        {
            case Type.Enemy:
                // Save this enemy state
                SaveDataManager.Instance.SaveEnemyState(instanceID, !GetComponent<HealthComponent>().isDead);
                break;

            case Type.MovingBlock:
                // Save this block position
                SaveDataManager.Instance.SaveBlockPosition(instanceID, transform.position);
                break;

            case Type.CanopicJar:
                SaveDataManager.Instance.SaveSkills();
                break;

            case Type.None:
            default:
                break;
        }
    }

    private void OnSkillUnlocked()
    {
        if (name.EndsWith(" - Dash") && playerSkills.HasDash
                    || name.EndsWith(" - GrapplingHook") && playerSkills.HasGrapplingHook
                    || name.EndsWith(" - SwarmDash") && playerSkills.HasSwarmDash
                    || name.EndsWith(" - SandSoldier") && playerSkills.HasSandSoldier)
        {
            GetComponent<VisualEffect>().enabled = true;
            //GetComponent<Light>().enabled = false; // currently no lights on the object
        }

        if (name.EndsWith(" - Dash") && playerSkills.HasDash)
            GameObject.Find("PauseMenu_v2").GetComponent<PauseMenu>().OnPauseMenu();
    }

}
