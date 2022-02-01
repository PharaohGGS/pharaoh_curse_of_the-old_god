using System;
using UnityEditor;

namespace Pharaoh.Gameplay.Component
{
    [CustomEditor(typeof(DamageComponent))]
    public class DamageComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DamageComponent script = (DamageComponent) target;

            script.damageDealingType =
                (DamageDealingType) EditorGUILayout.EnumPopup("DamageDealingType", script.damageDealingType);

            switch (script.damageDealingType)
            {
                case DamageDealingType.FireRate:
                    // show firerate variables
                    script.damagePerRate = EditorGUILayout.FloatField("DamagePerRate", script.damagePerRate);
                    script.fireRate = EditorGUILayout.FloatField("FireRate", script.fireRate);
                    break;
                case DamageDealingType.OneHit:
                    // show onehit variables
                    script.damagePerHit = EditorGUILayout.FloatField("DamagePerHit", script.damagePerHit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}