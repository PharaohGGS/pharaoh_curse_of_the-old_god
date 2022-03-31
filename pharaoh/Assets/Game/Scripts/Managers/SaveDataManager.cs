using UnityEngine;
using DesignPatterns;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Pharaoh.Managers
{
    public class SaveDataManager : PersistantMonoSingleton<SaveDataManager>
    {

        [System.Serializable]
        private class SaveData
        {
            public static readonly float DEFLOAT = -666f;

            public float[] lastCheckpoint;
            public bool[] skills; //[Swarm Dash, Sand Soldier, Grappling Hook]
            public bool[] enemiesStates;
            public float[] blocksPositions_x;
            public float[] blocksPositions_y;
            public float[] blocksPositions_z;

            public SaveData(uint movingBlockCount)
            {
                lastCheckpoint = new float[] { 0f, 0f, 0f };
                skills = new bool[] { false, false, false };
                enemiesStates = new bool[0];
                blocksPositions_x = new float[movingBlockCount];
                blocksPositions_y = new float[movingBlockCount];
                blocksPositions_z = new float[movingBlockCount];
                Array.Fill<float>(blocksPositions_x, DEFLOAT);
                Array.Fill<float>(blocksPositions_y, DEFLOAT);
                Array.Fill<float>(blocksPositions_z, DEFLOAT);
            }
        }

        private readonly string SAVEFILE = "/save.dat";
        [HideInInspector] public uint MOVINGBLOCKSCOUNT;

        private SaveData _saveData;
        private GameObject _player;
        private PlayerRespawn _playerRespawn;

        public PlayerSkills playerSkills;

        [Header("BUTTONS")]

        public bool save;
        public bool load;

        private void Start()
        {
            _saveData = new SaveData(MOVINGBLOCKSCOUNT);
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerRespawn = FindObjectOfType<PlayerRespawn>();
        }

        // Saves the current state of the game - last checkpoint triggered, enemies states and blocks positions to a save data object
        public void Save()
        {
            SaveLastCheckpoint(_playerRespawn.respawnPoint);
            SaveSkills();
            //SaveEnemiesStates();
            SaveBlocksPositions();

            SaveToJSON();
        }

        // Writes the save data to a save file
        private void SaveToJSON()
        {
            File.WriteAllText(Application.persistentDataPath + SAVEFILE, JsonUtility.ToJson(_saveData));
            Debug.Log("Saved on file : " + Application.persistentDataPath + SAVEFILE);
        }

        // Loads the saved state of the game - last checkpoint triggered, enemies states and blocks positions from a save data object
        public void Load()
        {
            LoadFromJSON();

            LoadLastCheckpoint();
            LoadSkills();
            //LoadEnemiesStates();
            LoadBlocksPositions();
        }

        // Loads the save data from the save file
        private void LoadFromJSON()
        {
            _saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(Application.persistentDataPath + SAVEFILE));
            Debug.Log("Loaded from file : " + Application.persistentDataPath + SAVEFILE);
        }


        // Saves the last checkpoint triggered to the save data object
        private void SaveLastCheckpoint(Vector3 checkpoint)
        {
            _saveData.lastCheckpoint = new float[] { checkpoint.x, checkpoint.y, checkpoint.z };
        }

        // Loads the last checkpoint triggered from the save data object
        private void LoadLastCheckpoint()
        {
            _playerRespawn.respawnPoint = new Vector3(_saveData.lastCheckpoint[0], _saveData.lastCheckpoint[1], _saveData.lastCheckpoint[2]);
            _player.transform.position = _playerRespawn.respawnPoint;
        }

        // Saves the states of the player skills to the save data object
        private void SaveSkills()
        {
            _saveData.skills = new bool[] { playerSkills.hasSwarmDash, playerSkills.hasSandSoldier, playerSkills.hasGrapplingHook };
        }

        // Loads the states of skills from the save data object
        private void LoadSkills()
        {
            playerSkills.hasSwarmDash = _saveData.skills[0];
            playerSkills.hasSandSoldier = _saveData.skills[1];
            playerSkills.hasGrapplingHook = _saveData.skills[2];
        }

        // Saves the enemies states into the save data object
        private void SaveEnemiesStates()
        {
            // Finds all GameObjects with tag "Enemy" and sort them by position
            /*IOrderedEnumerable<GameObject> sortedEnemies =
                GameObject.FindGameObjectsWithTag("Enemy").ToList().OrderBy(enemy => enemy.GetInstanceID());
            _saveData.enemiesStates = new bool[sortedEnemies.Count()];

            int i = 0;
            foreach (GameObject enemy in sortedEnemies)
            {
                _saveData.enemiesStates[i++] = enemy.GetComponent<Enemy>().IsAlive;
            }*/
        }

        // Loads the enemies states from the save data object
        private void LoadEnemiesStates()
        {
            // Finds all GameObjects with tag "MovingBlock" and sort them by position
            /*IOrderedEnumerable<GameObject> sortedEnemies =
                GameObject.FindGameObjectsWithTag("Enemy").ToList().OrderBy(enemy => enemy.GetInstanceID());

            // Loops through each blocks and load their position
            int i = 0;
            foreach (GameObject enemy in sortedEnemies)
            {
                enemy.GetComponent<Enemy>().IsAlive = _saveData.enemiesStates[i++];
            }*/
        }

        // Saves the blocks positions into the save data object
        private void SaveBlocksPositions()
        {
            // Loops through each MovingBlock and save its position in the save data
            foreach (GameObject block in GameObject.FindGameObjectsWithTag("MovingBlock"))
            {
                _saveData.blocksPositions_x[block.GetComponent<MovingBlock>().instanceID] = block.transform.position.x;
                _saveData.blocksPositions_y[block.GetComponent<MovingBlock>().instanceID] = block.transform.position.y;
                _saveData.blocksPositions_z[block.GetComponent<MovingBlock>().instanceID] = block.transform.position.z;
            }
        }

        // Loads the blocks positions from the save data object
        // DEPRECATED
        private void LoadBlocksPositions()
        {
            // Finds all GameObjects with tag "MovingBlock" and sort them by position
            IOrderedEnumerable<GameObject> sortedBlocks =
                GameObject.FindGameObjectsWithTag("MovingBlock").ToList().OrderBy(block => block.GetInstanceID());

            // Loops through each blocks and load their position
            int i = 0;
            foreach (GameObject block in sortedBlocks)
            {
                block.transform.position = new Vector3(_saveData.blocksPositions_x[i], _saveData.blocksPositions_y[i], _saveData.blocksPositions_z[i]);
                i++;
            }
        }

        // Loads a block position from the save data object
        // Returns true if the block ahs previously been saved, false otherwise
        public bool LoadBlockPosition(ulong instanceID, out Vector3 position)
        {
            position = new Vector3(_saveData.blocksPositions_x[instanceID], _saveData.blocksPositions_y[instanceID], _saveData.blocksPositions_z[instanceID]);
            return !(position.x == SaveData.DEFLOAT && position.y == SaveData.DEFLOAT && position.z == SaveData.DEFLOAT);
        }

        // Used to save/load from the inspector script
        private void OnValidate()
        {
            if (save)
            {
                Save();
                save = false;
            }
            if (load)
            {
                Load();
                load = false;
            }
        }

    }
}

