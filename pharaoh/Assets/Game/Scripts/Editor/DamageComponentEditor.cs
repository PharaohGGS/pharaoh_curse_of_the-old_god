using System;
using UnityEditor;

namespace Pharaoh.Gameplay.Component
{
    [CustomEditor(typeof(DamageComponent))]
    public class DamageComponentEditor : Editor
    {
        private SerializedProperty damageDealingTypeProp;

        private SerializedProperty fireRateProp;
        private SerializedProperty damagePerRateProp;

        private SerializedProperty damagePerHitProp;

        private void OnEnable()
        {
            damageDealingTypeProp = serializedObject.FindProperty("damageDealingType");
            damagePerRateProp = serializedObject.FindProperty("damagePerRate");
            fireRateProp = serializedObject.FindProperty("fireRate");
            damagePerHitProp = serializedObject.FindProperty("damagePerHit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DamageComponent script = (DamageComponent) target;

            EditorGUILayout.PropertyField(damageDealingTypeProp);
            var damageDealingOption = (DamageDealingType) damageDealingTypeProp.enumValueIndex;

            switch (damageDealingOption)
            {
                case DamageDealingType.FireRate:
                    // show firerate variables
                    EditorGUILayout.PropertyField(fireRateProp);
                    EditorGUILayout.PropertyField(damagePerRateProp);
                    break;
                case DamageDealingType.OneHit:
                    // show onehit variables
                    EditorGUILayout.PropertyField(damagePerHitProp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}