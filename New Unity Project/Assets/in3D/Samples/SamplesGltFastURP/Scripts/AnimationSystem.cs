using Alteracia.Animations;
using UnityEngine;

namespace in3D.Animations
{
    public class AnimationSystem : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController controller;
        [SerializeField] private AltAnimationGroup returnRootAnimation;
        
        private Animator _animator;
        private AltAnimator _returnRootAnimator;
  
        public void PrepareModel(Transform model)
        {
            var root = model.transform.Find("Scene"); // TODO make naming independent
            if (!root) return;
            root = root.Find("Armature"); // TODO make naming independent
            if (!root) return;
            Destroy(root.GetComponent<Animation>());
            _animator = root.GetComponent<Animator>();
            _animator.runtimeAnimatorController = controller;
            _animator.applyRootMotion = true;
            var skin = root.GetComponentInChildren<SkinnedMeshRenderer>();
            var hips = root.Find("mixamorig:Hips");
            skin.rootBone = hips;
            // When Animations applied 90 degrees model rotating TODO Fix on avatars
            model.transform.Rotate(Vector3.right * -90f);

            _returnRootAnimator = root.gameObject.AddComponent<AltAnimator>();
            _returnRootAnimator.Add(returnRootAnimation);
        }

        public void PlayAnimation(int index)
        {
            if (!_animator) return;
            
            if (_animator.GetInteger("index") != -1)
            {
                _returnRootAnimator.Play("return");
            }
            
            _animator.SetInteger("index", index);
            _animator.SetTrigger("play");
        }
    }
}
