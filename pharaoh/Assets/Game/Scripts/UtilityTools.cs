#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class UtilityTools
{

    private static readonly string SCENE_PATH = "Assets/Game/Scenes/Blocking/Scenes by room/";
    private static readonly string PLAYERSKILLSDEBUG_PATH = "Assets/Game/ScriptableObjects/SaveFile/PlayerSkillsData_DEBUG.asset";

    // Loads the SceneMerged scene as well as all the game scenes
    [MenuItem("Utility/Load Game Scenes")]
    private static void LoadGameScenes()
    {
        var allScenes = new DirectoryInfo(SCENE_PATH).GetFiles("*.unity", SearchOption.AllDirectories);

        EditorSceneManager.OpenScene(SCENE_PATH + "/SceneMerged/SceneMerged.unity", OpenSceneMode.Single);

        foreach (var file in allScenes)
        {
            if (Regex.IsMatch(file.Name, @"^\d+"))
                EditorSceneManager.OpenScene(file.FullName, OpenSceneMode.Additive);
        }
    }

    // Loads the SceneMerged scene as well as the first 2 game scenes
    [MenuItem("Utility/Load 2 First Game Scenes")]
    private static void LoadTwoFirstGameScenes()
    {
        FileInfo[] firstScene = new DirectoryInfo(SCENE_PATH).GetFiles("0-0 - SCENE.unity", SearchOption.AllDirectories);
        FileInfo[] secondScene = new DirectoryInfo(SCENE_PATH).GetFiles("1-0 - SCENE.unity", SearchOption.AllDirectories);

        EditorSceneManager.OpenScene(SCENE_PATH + "/SceneMerged/SceneMerged.unity", OpenSceneMode.Single);

        if (firstScene.Length != 1)
        {
            Debug.LogWarning("First scene \'0-0 - SCENE\' either not found or found multiple.");
            return;
        }
        EditorSceneManager.OpenScene(firstScene[0].FullName, OpenSceneMode.Additive);

        if (secondScene.Length != 1)
        {
            Debug.LogWarning("First scene \'1-0 - SCENE\' either not found or found multiple.");
            return;
        }
        EditorSceneManager.OpenScene(secondScene[0].FullName, OpenSceneMode.Additive);
    }

    [MenuItem("Utility/Reset Player Skills DEBUG")]
    private static void ResetPlayerSkillsDEBUG()
    {
        PlayerSkills playerSkills = (PlayerSkills)AssetDatabase.LoadAssetAtPath(PLAYERSKILLSDEBUG_PATH, typeof(PlayerSkills));

        if (playerSkills == null)
        {
            Debug.LogWarning("PlayerSkills_DEBUG at path " + PLAYERSKILLSDEBUG_PATH + " not found.");
            return;
        }

        playerSkills.hasDash = true;
        playerSkills.hasGrapplingHook = true;
        playerSkills.hasSwarmDash = true;
        playerSkills.hasSandSoldier = true;
        playerSkills.hasHeart = true;

        Debug.Log("Player Skills DEBUG Reset.");
    }

    private static void SetAllBlockingMeshRenderers(bool enabled)
    {
        List<Scene> loadedScenes = new List<Scene>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i));
        }

        foreach (Scene scene in loadedScenes)
        {
            GameObject[] gos = scene.GetRootGameObjects();
            foreach (GameObject go in gos)
            {
                if (go.name.EndsWith(" - BLOCKING"))
                {
                    MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in mrs)
                    {
                        mr.enabled = enabled;
                    }
                }
            }
        }
    }

    // Disables all mesh renderers in BLOCKING objects in the loaded scenes
    [MenuItem("Utility/Disable All Blocking Mesh Renderers")]
    private static void DisableAllBlockingMeshes()
    {
        SetAllBlockingMeshRenderers(false);

        Debug.Log("All blocking mesh renderers disabled.");
    }

    // Enables all mesh renderers in BLOCKING objects in the loaded scenes
    [MenuItem("Utility/Enable All Blocking Mesh Renderers")]
    private static void EnableAllBlockingMeshes()
    {
        SetAllBlockingMeshRenderers(true);

        Debug.Log("All blocking mesh renderers enabled.");
    }

}
#endif
