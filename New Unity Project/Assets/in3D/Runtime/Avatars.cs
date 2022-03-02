using System.Collections.Generic;
using System.Linq;

namespace in3D.AvatarsSDK
{
    public class In3dAvatar
    {
        private static readonly string[] Formats = { "dae", "fbx", "glb", "vto", "T_undressed", "vto_undressed" };
        public static string Format(ModelFormat format) => Formats[(int)format];
        public static ModelFormat Format(string format) => (ModelFormat)Formats.ToList().IndexOf(format);

        public string ScanId { get; private set; }
            
        public IModelUrls[] Urls { get; } = new IModelUrls[Formats.Length];
        public List<string> Items { get; } = new List<string>();
        

        public In3dAvatar(string scanId)
        {
            ScanId = scanId;
        }
    }

    // TODO more data for user avatar
    /*
    public class In3dUserAvatar : In3dAvatar
    {
        private ScansAndAvatarsData _properties;
        public ScansAndAvatarsData Properties => _properties;
        
        public In3dUserAvatar(ScansAndAvatarsData properties)
        {
            _properties = properties;
        }
    }
    */
}
