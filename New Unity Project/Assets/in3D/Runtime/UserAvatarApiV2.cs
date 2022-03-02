using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

using Alteracia.Web;
using in3D.AvatarsSDK.Configurations;

namespace in3D.AvatarsSDK
{
    /// <summary>
    /// Interface for access to users avatars data
    /// </summary>
    public interface IUserAvatarApi
    {
        /// <summary>
        /// All users and avatars in dictionary, fills after initial request.
        /// Always check for null.
        /// </summary>
        Dictionary<string, In3dUser> Users { get; }
        /// <summary>
        /// Get All ids of avatars for user. Server should be configured. User should be configured.
        /// </summary>
        /// <param name="configuration">user configuration</param>
        /// <returns>array of avatar ids for specific user</returns>
        Task<string[]> GetAvatarsIds(UserConfiguration configuration);
        /// <summary>
        /// Get all urls for user avatars. Server should be configured. User should be configured.
        /// </summary>
        /// <param name="configuration">user configuration</param>
        /// <param name="format">model format: fbx, dae, glb, vto</param>
        /// <returns>array of models urls for direct download</returns>
        Task<IModelUrls[]> GetAvatarsUrls(UserConfiguration configuration, ModelFormat format);
        /// <summary>
        /// Get specific urls of user avatar. Server should be configured. User should be configured.
        /// </summary>
        /// <param name="configuration">user configuration</param>
        /// <param name="format">model format: fbx, dae, glb, vto</param>
        /// <param name="avatarId">id of user avatar</param>
        /// <returns>models urls for direct download</returns>
        Task<IModelUrls> GetAvatarUrls(UserConfiguration configuration, ModelFormat format, string avatarId);
    }
    
    /// <summary>
    /// API for User Avatars. Version 2
    /// </summary>
    public class UserAvatarApiV2 : IUserAvatarApi
    {
        #region JsonScemes
            
        [Serializable]
        private class Metadata
        {
            public object tg3d;
            public object tg3d_upload;
            public string vendor_name;
        }
    
        [Serializable]
        private class ScanMetaData
        {
            public string id;
            public string vendor_id;
            public string status;
            public object eta_minutes;
            public string config_name;
            public string created_at; // DateTime - TODO FIX ALTWEB SOLUTION
            public string started_at; // TODO FIX ALTWEB SOLUTION
            public string processed_at; // TODO FIX ALTWEB SOLUTION
            public string callback_url;
            public string etag;
            public Metadata metadata;
        }
    
        [Serializable]
        private class AvatarMetaData
        {
            public string id;
            public string user_id;
            public string scan_id;
            public string device_type;
            public string device_id;
            public string source;
            public Metadata metadata;
            public object purpose;
        }
    
        [Serializable] 
        private class ScansAndAvatarsData
        {
            public ScanMetaData scan;
            public AvatarMetaData avatar;
        }
    
        [Serializable]
        private class AvatarsRequestBody
        {
            public string[] avatars_ids;
        }
            
        #endregion
        
        #region Urls

        [Serializable]
        private class AvatarData : ScansAndAvatarsData, IModelUrls
        {
            // Write from json
            [SerializeField] 
            public string url;
            
            // [] TODO Add attribute 
            // TODO make private
            public string _previewUrl;
            
            public AvatarData(string url, string preview, string parametric)
            {
                this.url = url;
                _previewUrl = preview;
                
                Urls.Add(Path.GetFileName(url), GetMainUrl());
                if (parametric != null) Urls.Add(Path.GetFileName(parametric.Split('?')[0]), parametric);
                if (preview != null) Urls.Add(Path.GetFileName(preview.Split('?')[0]), GetPreviewUrl());
            }

            public string GetMainUrl()
            {
                return url;
            }

            public string GetPreviewUrl()
            {
                return _previewUrl;
            }

            public string[] GetFromExtensionUrls(string extension) => Urls.GetUrlsWithExtension(extension);

            public string[] GetTextureUrls() => Urls.GetTextureUrls();

            public string[] GetModelUrls() => Urls.GetModelUrls();

            /// <summary>
            /// Always check for null
            /// </summary>
            public Dictionary<string, string> Urls { get; } = new Dictionary<string, string>();
        }
        
        #endregion 

        public Dictionary<string, In3dUser> Users { get; } = new Dictionary<string, In3dUser>();
        
        private Dictionary<string, string> _previews = new Dictionary<string, string>();

        private AvatarsServer _server;
        
        public UserAvatarApiV2(AvatarsServer server)
        {
            _server = server;
        }

        #region  Interface Methods

        
        public async Task<string[]> GetAvatarsIds(UserConfiguration configuration)
        {
            if (!await CheckAvatarsIds(configuration)) return null;

            return Users[configuration.UserId].Avatars.Select(a => a.Key).ToArray();
        }
        
        public async Task<IModelUrls[]> GetAvatarsUrls(UserConfiguration configuration, ModelFormat format)
        {
            if (!await CheckAvatarsIds(configuration) || !await CheckAvatarsLoaders(configuration, format)) return null;
            
            return Users[configuration.UserId].Avatars
                .Select(a => a.Value.Urls[(int)format])
                .Where(l => l != null).ToArray();
        }

        public async Task<IModelUrls> GetAvatarUrls(UserConfiguration configuration, ModelFormat format, string avatarId)
        {
            if (!await CheckUserAvatar(configuration, avatarId)) return null;
            
            if (Users[configuration.UserId].Avatars[avatarId].Urls[(int)format] == null
                && !await AddAvatarLoader(configuration, format, avatarId)) return null;
            
            return Users[configuration.UserId].Avatars[avatarId].Urls[(int)format];
        }
        

        #endregion
        
        #region Private Methods
        
        private async Task<bool> CheckUserAvatar(UserConfiguration configuration, string avatarId)
        {
            if (Users.Count != 0 && Users.ContainsKey(configuration.UserId) &&
                Users[configuration.UserId].Avatars.Count != 0 &&
                Users[configuration.UserId].Avatars.ContainsKey(avatarId)) return true;
            
            var user = GetUser(configuration);
            
            using (var req = await Requests.Post(
                $"{_server.Url}/v2/user_avatars/get/{avatarId}?{configuration.UserAppend}",
                configuration.AuthorizationHeader))
            {
                if (!req.Success()) return false;
                
                ScanMetaData scan = JsonUtility.FromJson<ScanMetaData>(
                    req.downloadHandler.text.FormatJsonText(typeof(ScanMetaData)));
                
                user.Avatars[avatarId] = new In3dAvatar(scan.id);

                return true;
            }
        }
        
        private async Task<bool> CheckAvatarsIds(UserConfiguration configuration)
        {
            if (Users.Count == 0 || 
                !Users.ContainsKey(configuration.UserId) || 
                Users[configuration.UserId].Avatars.Count == 0) 
                await GetAvatarList(configuration);
            
            return Users.Count != 0;
        }

        private async Task GetAvatarList(UserConfiguration configuration)
        {
            using (var req = await Requests.Post(
                $"{_server.Url}/v2/user_avatars/scans/list?{configuration.UserAppend}", configuration.AuthorizationHeader))
            {
                if (!req.Success()) return;

                JsonArray<ScansAndAvatarsData> avatars = JsonUtility.FromJson<JsonArray<ScansAndAvatarsData>>(
                    req.downloadHandler.text.FormatJsonText(typeof(JsonArray<ScansAndAvatarsData>)));

                In3dUser user = GetUser(configuration);

                if (avatars == null) return;

                foreach (var avatarData in avatars.list
                    .Where(a => string.IsNullOrEmpty(_server.Vendor)
                    || a.scan.metadata.vendor_name == _server.Vendor))
                {
                    user.Avatars[avatarData.avatar.id] = new In3dAvatar(avatarData.scan.id);
                }
            }
        }
        
        private In3dUser GetUser(UserConfiguration configuration)
        {
            In3dUser user = new In3dUser();
            if (Users.ContainsKey(configuration.UserId)) user = Users[configuration.UserId];
            else Users[configuration.UserId] = user;
            return user;
        }
        
        private async Task<bool> CheckAvatarsLoaders(UserConfiguration configuration, ModelFormat format)
        {
            if (Users[configuration.UserId].Avatars.Any(i => i.Value.Urls[(int)format] != null)) 
                return true;
            
            if (_previews.Count != Users[configuration.UserId].Avatars.Count)
            {
                var body = JsonUtility.ToJson(new AvatarsRequestBody()
                    { avatars_ids = Users[configuration.UserId].Avatars
                        .Select(a => a.Key).ToArray() });

                var req = await Requests.PostJson(
                    $"{_server.Url}/v2/user_avatars/preview_many?fmt={In3dAvatar.Format(format)}", body,
                    configuration.AuthorizationHeader);

                if (req.Success())
                {
                    string[] parts = req.downloadHandler.text.
                        Replace("\"", "").
                        Replace("{", "").
                        Replace("}", "").
                        Split(',');
                    
                    foreach (var pair in parts)
                    {
                        string[] part1 = pair.Split(':');
                        _previews.Add(part1[0], pair.Remove(0, part1[0].Length + 1));
                    }
                }
            }
            
            foreach (var avatar in Users[configuration.UserId].Avatars.Where(avatar => avatar.Value.Urls[(int)format] == null))
            {
                await AddAvatarLoader(configuration, format, avatar.Key);
            }
            
            return Users[configuration.UserId].Avatars.Any(i => i.Value.Urls[(int)format] != null);
        }
        
        private async Task<bool> AddAvatarLoader(UserConfiguration configuration, ModelFormat format, string avatarId)
        {
            using (var req = await Requests.Post(
                $"{_server.Url}/v2/user_avatars/model/{avatarId}?format={In3dAvatar.Format(format)}",
                configuration.AuthorizationHeader))
            {
                string json = "";
                
                if (req.responseCode == 401) // TODO in POST
                {
                    await configuration.Refresh(_server);
                    using (var repeat = await Requests.Post(
                        $"{_server.Url}/v2/user_avatars/model/{avatarId}?format={In3dAvatar.Format(format)}",
                        configuration.AuthorizationHeader))
                    {
                        if (!repeat.Success()) return false;
                        json = repeat.downloadHandler.text;
                    }
                }
                else if (!req.Success()) return false;
                else json = req.downloadHandler.text;

                if (string.IsNullOrEmpty(json)) return false;
                
                AvatarData avatarModelDto = JsonUtility.FromJson<AvatarData>(json.FormatJsonText(typeof(AvatarData)));
                
                // Get parametric data
                string parametric = null;
                string datFormat = In3dAvatar.Format(format).EndsWith("undressed") ? "dat_undressed" : "dat";
                using(var req1 = await Requests.Post(
                    $"{_server.Url}/v2/user_avatars/model/{avatarId}?format={datFormat}",
                    configuration.AuthorizationHeader))
                {
                    if (req1.Success())
                    {
                        parametric = JsonUtility.FromJson<AvatarData>( // TODO Create string to object attribute
                            req1.downloadHandler.text.FormatJsonText(typeof(AvatarData))).url;
                    }
                }

                // Get Preview
                string previewLink = null;
                if (_previews.ContainsKey(avatarId)) previewLink = _previews[avatarId];
                else
                {
                    using(var req1 = await Requests.Post(
                        $"{_server.Url}/v2/user_avatars/preview/{avatarId}?fmt={In3dAvatar.Format(format)}",
                        configuration.AuthorizationHeader))
                    {
                        if (req1.Success())
                        {
                            previewLink = JsonUtility.FromJson<string>( // TODO Create string to object attribute
                                req1.downloadHandler.text.FormatJsonText(typeof(string)));
                        }
                    }
                }

                Users[configuration.UserId].Avatars[avatarId].Urls[(int)format] =
                    new AvatarData(avatarModelDto.url, previewLink, parametric);
            }
            return true;
        }

        #endregion
    }
}
