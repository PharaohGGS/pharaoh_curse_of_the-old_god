using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Pharaoh.Gameplay
{
    [System.Serializable]
    public enum PropertyType : int
    {
        Float = 0,
        Integer = 1,
        Color = 2,
        Vector2 = 3,
        Vector3 = 4,
        Vector4 = 5,
        Gradient = 6,
    }
    
    [CreateAssetMenu(fileName = "New Fade Property", menuName = "VFX/Property/Fade")]
    public class Fade : ScriptableObject
    {
        public string propertyName;
        public float speedIn;
        public float speedOut;
        
        public PropertyType type;
        [HideInInspector] public float maxFloat;
        [HideInInspector] public float minFloat;
        [HideInInspector] public int maxInteger;
        [HideInInspector] public int minInteger;
        [HideInInspector] public Color maxColor;
        [HideInInspector] public Color minColor;
        [HideInInspector] public Vector2 maxVector2;
        [HideInInspector] public Vector2 minVector2;
        [HideInInspector] public Vector3 maxVector3;
        [HideInInspector] public Vector3 minVector3;
        [HideInInspector] public Vector4 maxVector4;
        [HideInInspector] public Vector4 minVector4;
        [HideInInspector, GradientUsage(true)] public Gradient maxGradient;
        [HideInInspector, GradientUsage(true)] public Gradient minGradient;

        private void OnEnable()
        {
            propertyName = this.name.Replace("(Clone)", "");
        }

        public bool DoFading(VisualEffect vfx, FadeTransition transition)
        {
            float maxDelta = Time.deltaTime * (transition == FadeTransition.In ? speedIn : speedOut);
            bool ended = false;
            
            switch (type)
            {
                case PropertyType.Float:
                    var fCurrent = vfx.GetFloat(propertyName);
                    var fTarget = transition == FadeTransition.In ? maxFloat : minFloat;
                    ended = Mathf.Abs(fCurrent - fTarget) <= Mathf.Epsilon;
                    var fLerp = maxDelta <= Mathf.Epsilon ? fTarget : Mathf.MoveTowards(fCurrent, fTarget, maxDelta);
                    if (!ended) vfx.SetFloat(propertyName, fLerp);
                    break;
                case PropertyType.Integer:
                    var iCurrent = vfx.GetInt(propertyName);
                    var iTarget = transition == FadeTransition.In ? maxInteger : minInteger;
                    ended = Mathf.Abs(iCurrent - iTarget) <= Mathf.Epsilon;
                    var iLerp = maxDelta <= Mathf.Epsilon ? iTarget : Mathf.CeilToInt(Mathf.MoveTowards(iCurrent, iTarget, maxDelta));
                    if (!ended) vfx.SetInt(propertyName, iLerp);
                    break;
                case PropertyType.Vector2:
                    var v2Current = vfx.GetVector2(propertyName);
                    var v2Target = (transition == FadeTransition.In ? maxVector2 : minVector2);
                    ended = Mathf.Abs(v2Current.magnitude - v2Target.magnitude) <= Mathf.Epsilon;
                    var v2Lerp = maxDelta <= Mathf.Epsilon ? v2Target : Vector2.MoveTowards(v2Current, v2Target, maxDelta);
                    if (!ended) vfx.SetVector2(propertyName, v2Lerp);
                    break;
                case PropertyType.Vector3:
                    var v3Current = vfx.GetVector3(propertyName);
                    var v3Target = (transition == FadeTransition.In ? maxVector3 : minVector3);
                    ended = Mathf.Abs(v3Current.magnitude - v3Target.magnitude) <= Mathf.Epsilon;
                    var v3Lerp = maxDelta <= Mathf.Epsilon ? v3Target : Vector3.MoveTowards(v3Current, v3Target, maxDelta);
                    if (!ended) vfx.SetVector2(propertyName, v3Lerp);
                    break;
                case PropertyType.Vector4:
                    var v4Current = vfx.GetVector4(propertyName);
                    var v4Target = (transition == FadeTransition.In ? maxVector4 : minVector4);
                    ended = Mathf.Abs(v4Current.magnitude - v4Target.magnitude) <= Mathf.Epsilon;
                    var v4Lerp = maxDelta <= Mathf.Epsilon ? v4Target : Vector4.MoveTowards(v4Current, v4Target, maxDelta);
                    if (!ended) vfx.SetVector4(propertyName, v4Lerp);
                    break;
                case PropertyType.Color:
                    var cCurrent = vfx.GetVector4(propertyName);
                    var cTarget = (Vector4) (transition == FadeTransition.In ? maxColor : minColor);
                    ended = Mathf.Abs(cCurrent.magnitude - cTarget.magnitude) <= Mathf.Epsilon;
                    var cLerp = maxDelta <= Mathf.Epsilon ? cTarget : Vector4.MoveTowards(cCurrent, cTarget, maxDelta);
                    if (!ended) vfx.SetVector4(propertyName, cLerp);
                    break;
                case PropertyType.Gradient:
                    var gCurrent = vfx.GetGradient(propertyName);
                    var gTarget = (transition == FadeTransition.In ? maxGradient : minGradient);
                    if (gCurrent.colorKeys.Length != gTarget.colorKeys.Length || gCurrent.alphaKeys.Length != gTarget.alphaKeys.Length) break;
                    
                    bool[] endeds = new bool[gCurrent.colorKeys.Length];

                    /* https://forum.unity.com/threads/lerp-from-one-gradient-to-another.342561/ */
                    var keysTimes = new System.Collections.Generic.List<float>();
                    
                    // color keys
                    for (int i = 0; i < gCurrent.colorKeys.Length; i++) 
                    {
                        float k = gCurrent.colorKeys[i].time;
                        if (!keysTimes.Contains(k)) keysTimes.Add(k);
                    }
                    for (int i = 0; i < gTarget.colorKeys.Length; i++) 
                    {
                        float k = gTarget.colorKeys[i].time;
                        if (!keysTimes.Contains(k)) keysTimes.Add(k);
                    }
                    
                    // alpha keys
                    for (int i = 0; i < gCurrent.alphaKeys.Length; i++) 
                    {
                        float k = gCurrent.alphaKeys[i].time;
                        if (!keysTimes.Contains(k)) keysTimes.Add(k);
                    }

                    for (int i = 0; i < gTarget.alphaKeys.Length; i++) 
                    {
                        float k = gTarget.alphaKeys[i].time;
                        if (!keysTimes.Contains(k)) keysTimes.Add(k);
                    }
                    
                    GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
                    GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

                    for (int i = 0; i < keysTimes.Count; i++)
                    {
                        float key = keysTimes[i];
                        var current = (Vector4) gCurrent.Evaluate(key);
                        var target = (Vector4) gTarget.Evaluate(key);
                        endeds[i] = Mathf.Abs(current.magnitude - target.magnitude) <= Mathf.Epsilon;
                        Color lerp = maxDelta <= Mathf.Epsilon ? target : Vector4.MoveTowards(current, target, maxDelta);
                        clrs[i] = new GradientColorKey(lerp, key);
                        alphas[i] = new GradientAlphaKey(lerp.a, key);
                    }
                    
                    var g = new UnityEngine.Gradient();
                    g.SetKeys(clrs, alphas);
                    
                    ended = Array.TrueForAll(endeds, b => b == true);
                    if (!ended) vfx.SetGradient(propertyName, g);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ended;
        }
    }
}