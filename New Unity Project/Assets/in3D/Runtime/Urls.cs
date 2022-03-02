using System;
using System.Collections.Generic;
using System.Linq;

namespace in3D.AvatarsSDK
{
    /// <summary>
    /// files format and special format of avatars
    /// </summary>
    public enum ModelFormat
    {
        /// <summary>
        /// Digital Asset Exchange file format, rigged, packed in zip archive with textures and animations
        /// </summary>
        Dae,
        /// <summary>
        /// FBX (Filmbox) file format, textured, rigged
        /// </summary>
        Fbx,
        /// <summary>
        /// GLTF (GL Transmission Format) binary file format, textured, rigged
        /// </summary>
        Glb,
        /// <summary>
        /// GLTF (GL Transmission Format) binary file format, in static pose for virtual fitting room, textured (obsolete)
        /// </summary>
        Vto,
        
        //Torso Obsolete
        
        /// <summary>
        /// Undressed version of avatar in glb file format, textured, rigged.
        /// Null if scan of user already stripped or scan created without undress config
        /// </summary>
        UndressedGlb,
        /// <summary>
        /// Obsolete.
        /// Undressed version of avatar in in static pose for virtual fitting room, textured.
        /// Null if scan of user already stripped or scan created without undress config
        /// </summary>
        UndressedVto
    }

    /// <summary>
    /// Interface for access to direct urls for model, model parts, preview image and special data
    /// </summary>
    public interface IModelUrls
    {
        /// <summary>
        /// Main url of model
        /// </summary>
        /// <returns>Full url as string</returns>
        string GetMainUrl();
        /// <summary>
        /// Preview url of model
        /// </summary>
        /// <returns></returns>
        string GetPreviewUrl();
        /// <summary>
        /// All urls with specified extension
        /// <param name="extension">"zip", "fbx", "glb", "gltf", "bin", "dat", "json"</param>
        /// </summary>
        /// <returns>Null if no parts with such extension provided</returns>
        string[] GetFromExtensionUrls(string extension);
        /// <summary>
        /// All urls of textures if is eny
        /// </summary>
        /// <returns>Null if no textures provided</returns>
        string[] GetTextureUrls();
        /// <summary>
        /// Separate parts of model - scene (gltf), mesh (bin), indexes (idx) if is any
        /// </summary>
        /// <returns>Null if no parts provided</returns>
        string[] GetModelUrls();
        /// <summary>
        /// All urls in dictionary, can be empty and fills after initiated Get Methods
        /// </summary>
        Dictionary<string, string> Urls { get; }
    }
    
    /// <summary>
    /// Extension for IModelUrls classes
    /// </summary>
    internal static class ModelsUrls
    {
        private static readonly string[] TexturesExtension = { "jpg", "png", "ktx2" };
        private static readonly string[] ModelsExtension = { "zip", "fbx", "glb", "gltf", "bin", "dat", "json" };
        
        public static string[] GetTextureUrls(this Dictionary<string, string> urls)
        {
            return (from pair in urls 
                where TexturesExtension.Any(extension => pair.Key.EndsWith("." + extension)) 
                select pair.Value).ToArray();
        }
        
        public static string[] GetModelUrls(this Dictionary<string, string> urls)
        {
            return (from pair in urls 
                where ModelsExtension.Any(extension => pair.Key.EndsWith("." + extension)) 
                select pair.Value).ToArray();
        }

        public static string[] GetUrlsWithExtension(this Dictionary<string, string> urls, string extension)
        {
            return urls.Where(pair => pair.Key.EndsWith($".{extension}")).Select(pair => pair.Value).ToArray();
        }
    }

    [Serializable]
    internal class GltfUrls
    {
        [Alteracia.Web.JsonProperties("channels.")]
        public string channels;
			
        [Alteracia.Web.JsonProperties("diffuse.")]
        public string diffuse;

        [Alteracia.Web.JsonProperties("normals.")]
        public string normals;

        [Alteracia.Web.JsonProperties("model.gltf")]
        public string scene;
			
        [Alteracia.Web.JsonProperties("model.bin")]
        public string mesh;
            
        [Alteracia.Web.JsonProperties("mask.")]
        public string mask;

        [Alteracia.Web.JsonProperties("idx.dat")]
        public string indexes;
        
        [Alteracia.Web.JsonProperties("offsets.dat")]
        public string offsets;
        
        [Alteracia.Web.JsonProperties("annotation.json")]
        public string annotation;
        
        [Alteracia.Web.JsonProperties("support_displacements.json")]
        public string displacements;
    }
    
    [Serializable]
    internal class UrlParts
    {
        public string origin;
        public string avatar_path;
        public string avatar_clothes_path;
        public string clothes_path;
    }
}
