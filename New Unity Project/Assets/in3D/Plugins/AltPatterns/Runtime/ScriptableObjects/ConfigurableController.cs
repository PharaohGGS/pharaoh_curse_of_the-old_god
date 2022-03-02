using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Alteracia.Patterns.ScriptableObjects
{
   [Serializable]
   public abstract class ConfigurationReader : ScriptableObject
   {
      public abstract Task ReadConfigFile(ScriptableObject configurable);
   }

   [Serializable]
   public class ConfigEvent : UnityEvent<ScriptableObject> {}
   
   public abstract class ConfigurableController<T0, T1> : Controller<T0> where T0 : Controller<T0> where T1 : ScriptableObject
   {
      [SerializeField] 
      private bool initOnStart;
      [SerializeField]
      protected ConfigurationReader reader;
      [SerializeField]
      protected T1 configuration;
      public T1 Configuration => configuration;

      [Header("Config Ready Events")] 
      [SerializeField]
      private ObjectEvent<T1> scriptableEvent;
      [Space]
      [SerializeField]
      public ConfigEvent configurationReady = new ConfigEvent();

      private void Start()
      {
         if (initOnStart) ReadConfiguration();
      }

      public void SetConfiguration(T1 config, bool read = true)
      {
         this.configuration = config;
         if (read) ReadConfiguration();
      }
      
      public void RewriteConfiguration(string json)
      {
         SetConfiguration(json, true);
      }

      public void SetConfiguration(string json, bool rewrite = false, bool read = true)
      {
         if (!rewrite) this.configuration = Instantiate(this.configuration);
         JsonUtility.FromJsonOverwrite(json, configuration);
         if (read) ReadConfiguration();
      }

      public async void ReadConfiguration()
      {
         if (reader)
         {
            // Try load config
            await reader.ReadConfigFile(configuration);
         }
         
         if (!configuration) return;
         
         this.OnConfigurationRead();
         
         configurationReady?.Invoke(configuration);
         if (scriptableEvent) scriptableEvent.OnEvent?.Invoke(configuration);
      }

      protected abstract void OnConfigurationRead();

#if UNITY_EDITOR
      public void CreateConfiguration()
      {
         var path = UnityEditor.EditorUtility.SaveFilePanel(
            "Create configuration",
            "",
            typeof(T1) + ".asset",
            "asset");

         if (string.IsNullOrEmpty(path)) return;
         if (!path.StartsWith(Application.dataPath))
         {
            Debug.LogError("Can't save configuration outside project assets folder");
            return;
         }
         
         var so = ScriptableObject.CreateInstance<T1>();
         
         Undo.RecordObject(so, $"Create configuration {typeof(T1)}");
         AssetDatabase.CreateAsset(so, path.Substring(Application.dataPath.Length - 6));

         this.configuration = so;
         
         EditorUtility.SetDirty(so);
      }
      
      public void ReadConfigurationFromJson()
      {
         var path = UnityEditor.EditorUtility.OpenFilePanel(
            "Open configuration",
            "",
            "json");
         
         if (string.IsNullOrEmpty(path)) return;

         StreamReader freader = new StreamReader(path);
         string json = freader.ReadToEnd();
         
         JsonUtility.FromJsonOverwrite(json, configuration);
      }

      public void SaveConfigurationToJson()
      {
         var path = UnityEditor.EditorUtility.SaveFilePanel(
            "Save configuration as json",
            "",
            typeof(T1) + ".json",
            "json");

         if (string.IsNullOrEmpty(path)) return;
         
         var json = JsonUtility.ToJson(configuration);
         
         // Get only parameters, no reference to object
         Regex regex = new Regex(@"\s*""([^""]*?)""\s*:\s*\{([^\{\}]*?)\}(,|\s|)");
         json = regex.Replace(json, "");
         // Clear "," in the end
         regex = new Regex(@",\s*\}");
         json = regex.Replace(json, "}");

         if (string.IsNullOrEmpty(json)) return;

         StreamWriter writer = new StreamWriter(path, false);
         writer.WriteLine(json);
         writer.Close();
      }
      
#endif
   }
}
