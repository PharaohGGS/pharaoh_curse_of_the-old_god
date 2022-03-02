using System;
using System.Threading.Tasks;
using BKUnity;
using UnityEngine;
using UnityEngine.Events;
#if GLTFAST_UNITY
using GLTFast;
using GLTFast.Loading;
#endif
namespace in3D.Avatars
{
#if GLTFAST_UNITY
    public class GltfHumanAsset : GltfAsset
    {
        public RuntimeAnimatorController controller;
        
        [Serializable]
        public class AnimatorReady : UnityEvent<Animator> {}

        public AnimatorReady OnAnimatorReady = new AnimatorReady();
        
        public override async Task<bool> Load(
            string url,
            IDownloadProvider downloadProvider=null,
            IDeferAgent deferAgent=null,
            IMaterialGenerator materialGenerator=null,
            ICodeLogger logger = null
        )
        {
            bool success = await base.Load(url, downloadProvider, deferAgent, materialGenerator, logger);

            if (!success) return success;
            // Look for root node
            var root = this.transform.Find("Scene");
            if (!root) return success;
            // When Animations applied 90 degrees model rotating TODO Fix on avatars
            root.Rotate(Vector3.right * -90f);
            
            root = root.Find("Armature");
            if (!root) return success;
            
            // Remove GltFast Animation
            Destroy(root.GetComponent<Animation>());
            // Add Animator
            var animator = root.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = true;
            // Set up humanoid
            root.gameObject.AddComponent<HumanoidAvatarBuilder>();
            
            OnAnimatorReady.Invoke(animator);

            return success;
        }
    }
#endif
}
