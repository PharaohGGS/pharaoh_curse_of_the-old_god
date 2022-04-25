using UnityEngine;
using System.IO;
using System;
using Pharaoh.Gameplay.Components;

namespace Pharaoh.Managers
{
    public class SaveDataManager : MonoBehaviour
    {

        public static SaveDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _prefsData = new PrefsData();
        }

        [System.Serializable]
        private class PrefsData
        {
            public int windowMode = 0;
            public int resolutionValue = 4;
        }

        [System.Serializable]
        private class SaveData
        {
            public static readonly float DEFLOAT = -666f;

            public float[] lastCheckpoint;
            public string lastScene;
            public bool[] skills; //[Swarm Dash, Sand Soldier, Grappling Hook]
            public bool[] enemiesStates;
            public float[] blocksPositions_x;
            public float[] blocksPositions_y;
            public float[] blocksPositions_z;

            public SaveData(uint enemiesCount, uint movingBlockCount)
            {
                lastCheckpoint = new float[] { 0f, 0f, 0f };
                lastScene = "0-0 - SCENE";
                skills = new bool[] { false, false, false, false, false };
                enemiesStates = new bool[enemiesCount];
                blocksPositions_x = new float[movingBlockCount];
                blocksPositions_y = new float[movingBlockCount];
                blocksPositions_z = new float[movingBlockCount];
                Array.Fill(enemiesStates, true);
                Array.Fill(blocksPositions_x, DEFLOAT);
                Array.Fill(blocksPositions_y, DEFLOAT);
                Array.Fill(blocksPositions_z, DEFLOAT);
            }
        }

        private readonly string SAVEFILE = "/save.dat";
        private readonly string PREFSFILE = "/prefs.dat";
        public uint ENEMIES_COUNT;
        public uint MOVING_BLOCKS_COUNT;

        private SaveData _saveData;
        private PrefsData _prefsData;

        public LastCheckpoint lastCheckpoint;
        public PlayerSkills playerSkills;

        // Creates a new save file
        public void NewSave()
        {
            _saveData = new SaveData(ENEMIES_COUNT, MOVING_BLOCKS_COUNT);
            playerSkills.Reset();
            lastCheckpoint.position = new Vector3(-2.5f, 2f, 1f); //need because ScriptableObjects are saved in editor
            Save();
            LevelManager.Instance.ChangeRoom(_saveData.lastScene);
        }

        // Loads the save file
        public void LoadSave()
        {
            _saveData = new SaveData(ENEMIES_COUNT, MOVING_BLOCKS_COUNT);
            if (SaveFileExists())
                Load();
            else
                Debug.LogWarning("Trying to load but no save file found.");
        }

        // Erases the save file
        public void EraseSave()
        {
            Debug.Log("Erasing save");
            if (SaveFileExists())
                File.Delete(Application.persistentDataPath + SAVEFILE);
            else
                Debug.Log("No save file exists.");
        }

        // Saves the current state of the game - last checkpoint triggered, enemies states and blocks positions to a save data object
        public void Save()
        {
            SaveLastCheckpoint();
            SaveSkills();
            SaveEnemiesStates(); //saving currently displayed enemies
            SaveBlocksPositions(); //saving currently displayed moving blocks

            SaveToJSON();
        }

        // Loads the saved state of the game - last checkpoint triggered, enemies states and blocks positions from a save data object
        public void Load()
        {
            LoadFromJSON();

            LoadLastCheckpoint();
            LoadSkills();
            // Enemy states are loaded during runtime
            // Blocks positions are loaded during runtime
        }

        // Returns whether there is a save file present or not
        public bool SaveFileExists()
        {
            return File.Exists(Application.persistentDataPath + SAVEFILE);
        }

        // Returns whether there is a prefs file present or not
        public bool PrefsFileExists()
        {
            return File.Exists(Application.persistentDataPath + PREFSFILE);
        }

        // Writes the save data to a save file
        private void SaveToJSON()
        {
            File.WriteAllText(Application.persistentDataPath + SAVEFILE, JsonUtility.ToJson(_saveData));
            Debug.Log("Saved on file : " + Application.persistentDataPath + SAVEFILE);
        }

        // Loads the save data from the save file
        private void LoadFromJSON()
        {
            _saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + SAVEFILE));
            Debug.Log("Loaded from file : " + Application.persistentDataPath + SAVEFILE);
        }


        // Saves the last checkpoint triggered to the save data object
        private void SaveLastCheckpoint()
        {
            _saveData.lastCheckpoint = new float[] { lastCheckpoint.position.x, lastCheckpoint.position.y, lastCheckpoint.position.z };
            _saveData.lastScene = LevelManager.Instance.currentRoom;
        }

        // Loads the last checkpoint triggered from the save data object
        private void LoadLastCheckpoint()
        {
            lastCheckpoint.position = new Vector3(_saveData.lastCheckpoint[0], _saveData.lastCheckpoint[1], _saveData.lastCheckpoint[2]);
            LevelManager.Instance.currentRoom = _saveData.lastScene;
        }

        // Saves the states of the player skills to the save data object
        public void SaveSkills()
        {
            _saveData.skills = new bool[] { playerSkills.HasDash, playerSkills.HasGrapplingHook, playerSkills.HasSwarmDash, playerSkills.HasSandSoldier, playerSkills.HasHeart };
        }

        // Loads the states of skills from the save data object
        private void LoadSkills()
        {
            playerSkills.hasDash = _saveData.skills[0];
            playerSkills.hasGrapplingHook = _saveData.skills[1];
            playerSkills.hasSwarmDash = _saveData.skills[2];
            playerSkills.hasSandSoldier = _saveData.skills[3];
            playerSkills.hasHeart = _saveData.skills[4];
        }

        // Saves all the currently loaded enemies states
        private void SaveEnemiesStates()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                _saveData.enemiesStates[go.GetComponent<SaveInstance>().instanceID] = !go.GetComponent<HealthComponent>().isDead;
            }
        }

        // Saves one enemy defined by its intance ID to a given state
        public void SaveEnemyState(ulong instanceID, bool state)
        {
            _saveData.enemiesStates[instanceID] = state;
        }

        // Loads an enemy state from the save data object
        public void LoadEnemyState(ulong instanceID, out bool state)
        {
            state = _saveData.enemiesStates[instanceID];
        }

        // Saves all the currently loaded blocks positions
        private void SaveBlocksPositions()
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("MovingBlock"))
            {
                _saveData.blocksPositions_x[go.GetComponent<SaveInstance>().instanceID] = go.transform.position.x;
                _saveData.blocksPositions_y[go.GetComponent<SaveInstance>().instanceID] = go.transform.position.y;
                _saveData.blocksPositions_z[go.GetComponent<SaveInstance>().instanceID] = go.transform.position.z;
            }
        }

        // Saves one block defined by its intance ID to a given position
        public void SaveBlockPosition(ulong instanceID, Vector3 position)
        {
            _saveData.blocksPositions_x[instanceID] = position.x;
            _saveData.blocksPositions_y[instanceID] = position.y;
            _saveData.blocksPositions_z[instanceID] = position.z;
        }

        // Loads a block position from the save data object
        // Returns true if the block ahs previously been saved, false otherwise
        public bool LoadBlockPosition(ulong instanceID, out Vector3 position)
        {
            position = new Vector3(_saveData.blocksPositions_x[instanceID], _saveData.blocksPositions_y[instanceID], _saveData.blocksPositions_z[instanceID]);
            return !(position.x == SaveData.DEFLOAT && position.y == SaveData.DEFLOAT && position.z == SaveData.DEFLOAT);
        }

        public void SavePrefs(int windowMode, int resolutionValue)
        {
            _prefsData.windowMode = windowMode;
            _prefsData.resolutionValue = resolutionValue;

            SavePrefsToJSON();
        }

        public int LoadWindowMode()
        {
            return _prefsData.windowMode;
        }

        public int LoadResolutionValue()
        {
            return _prefsData.resolutionValue;
        }

        private void SavePrefsToJSON()
        {
            File.WriteAllText(Application.persistentDataPath + PREFSFILE, JsonUtility.ToJson(_prefsData));
            Debug.Log("Saved on file : " + Application.persistentDataPath + PREFSFILE);
        }

        public void LoadPrefsFromJSON()
        {
            _prefsData = JsonUtility.FromJson<PrefsData>(File.ReadAllText(Application.persistentDataPath + PREFSFILE));
            Debug.Log("Loaded from file : " + Application.persistentDataPath + PREFSFILE);
        }

    }
}

