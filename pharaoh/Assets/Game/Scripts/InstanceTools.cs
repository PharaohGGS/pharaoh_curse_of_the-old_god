#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;
using System.Collections.Generic;
using System.IO;

public class InstanceTools
{
    private const int MAINMENU_INDEX = 0;
    private const string PLAYERSKILLS_PATH = "Assets/Game/ScriptableObjects/SaveFile/PlayerSkillsData.asset";
    private static readonly string SAVEFILE_PATH = "/save.dat";
    private static readonly string PREFSFILE_PATH = "/prefs.dat";

    // Returns all the build scenes as their paths
    private static List<string> GetAllBuildScenes()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
        }
        return scenes;
    }

    // Returns whether all build scenes are loaded or not
    static private bool AreAllBuildScenesLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (!SceneManager.GetSceneByBuildIndex(i).isLoaded)
                return false;
        }

        return true;
    }

    [MenuItem("Instances/Load Build Scenes")]
    private static void LoadBuildScenes()
    {
        List<string> scenes = GetAllBuildScenes();
        foreach (string s in scenes)
        {
            if (!SceneManager.GetSceneByPath(s).isLoaded)
                EditorSceneManager.OpenScene(s, OpenSceneMode.Additive);
        }
    }

    [MenuItem("Instances/Generate Enemies Instance IDs")]
    private static void GenerateEnemiesInstanceIDs()
    {
        if (!AreAllBuildScenesLoaded())
        {
            Debug.LogWarning("Not all build scenes loaded.");
            return;
        }

        uint id = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            go.name = "Enemy_" + id++;
        }

        foreach (string s in GetAllBuildScenes())
        {
            EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(s));
        }

        Debug.Log("Counted " + id + " enemies.");

        GameObject[] gos = SceneManager.GetSceneByBuildIndex(MAINMENU_INDEX).GetRootGameObjects();
        foreach (GameObject go in gos)
        {
            if (go.name == "SaveDataManager")
            {
                go.GetComponent<SaveDataManager>().ENEMIES_COUNT = id;
                EditorSceneManager.SaveScene(SceneManager.GetSceneByBuildIndex(MAINMENU_INDEX));
                Debug.Log("SaveDataManager's enemies count updated.");
                return;
            }
        }

        Debug.LogWarning("No SaveDataManager found.");
    }

    [MenuItem("Instances/Generate Blocks Instance IDs")]
    private static void GenerateBlocksInstanceIDs()
    {
        if (!AreAllBuildScenesLoaded())
        {
            Debug.LogWarning("Not all build scenes loaded.");
            return;
        }

        uint id = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MovingBlock"))
        {
            go.name = "MovingBlock_" + id++;
        }

        foreach (string s in GetAllBuildScenes())
        {
            EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(s));
        }

        Debug.Log("Counted " + id + " moving blocks.");

        GameObject[] gos = SceneManager.GetSceneByBuildIndex(MAINMENU_INDEX).GetRootGameObjects();
        foreach (GameObject go in gos)
        {
            if (go.name == "SaveDataManager")
            {
                go.GetComponent<SaveDataManager>().MOVING_BLOCKS_COUNT = id;
                EditorSceneManager.SaveScene(SceneManager.GetSceneByBuildIndex(MAINMENU_INDEX));
                Debug.Log("SaveDataManager's moving blocks count updated.");
                return;
            }
        }

        Debug.LogWarning("No SaveDataManager found.");
    }

    [MenuItem("Instances/Reset Player Skills")]
    private static void ResetPlayerSkills()
    {
        PlayerSkills playerSkills = (PlayerSkills)AssetDatabase.LoadAssetAtPath(PLAYERSKILLS_PATH, typeof(PlayerSkills));

        if (playerSkills == null)
        {
            Debug.LogWarning("PlayerSkills at path " + PLAYERSKILLS_PATH + " not found.");
            return;
        }

        playerSkills.Reset();

        Debug.Log("Player Skills Reset.");
    }

    [MenuItem("Instances/Delete Save File")]
    private static void EraseSaveFile()
    {
        string saveFile = Application.persistentDataPath + SAVEFILE_PATH;

        if (File.Exists(saveFile))
        {
            File.Delete(saveFile);
            Debug.Log("Save file deleted.");
        }
        else
            Debug.Log("No save file found.");
    }

    [MenuItem("Instances/Delete Prefs File")]
    private static void ErasePrefsFile()
    {
        string prefsFile = Application.persistentDataPath + PREFSFILE_PATH;

        if (File.Exists(prefsFile))
        {
            File.Delete(prefsFile);
            Debug.Log("Prefs file deleted.");
        }
        else
            Debug.Log("No prefs file found.");
    }

    [MenuItem("Instances/Unload Build Scenes")]
    private static void UnloadBuildScenes()
    {
        List<string> scenes = GetAllBuildScenes();
        foreach (string s in scenes)
        {
            Scene scene = SceneManager.GetSceneByPath(s);
            if (scene.isLoaded && scene != SceneManager.GetActiveScene())
                EditorSceneManager.CloseScene(scene, false);
        }
    }
}
#endif
