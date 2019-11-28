using UnityEngine;
using UnityEditor;

namespace Cursed.Combat
{
    [CustomEditor(typeof(AttackDefinition))]
    public class AttackDefinitionEditor : Editor
    {
        DamageTypeDefinition lastType = DamageTypeDefinition.Fixed;
        public override void OnInspectorGUI()
        {
            var a = target as AttackDefinition;

            GUILayout.Label("Data", EditorStyles.boldLabel);
            a.cooldown = EditorGUILayout.FloatField("Cooldown", a.cooldown);
            GUILayout.Space(10);

            GUILayout.Label("Damage", EditorStyles.boldLabel);
            a.damageType = (DamageTypeDefinition)EditorGUILayout.EnumPopup(a.damageType);
            if(a.damageType != lastType)
            {
                lastType = a.damageType;
                a.ResetDamages();
            }
            switch (a.damageType)
            {
                case DamageTypeDefinition.Fixed:
                    a.fixedDamage = EditorGUILayout.FloatField("Damages", a.fixedDamage);
                    break;
                case DamageTypeDefinition.Fork:
                    a.minDamage = EditorGUILayout.FloatField("Min", a.minDamage);
                    a.maxDamage = EditorGUILayout.FloatField("Max", a.maxDamage);
                    break;
                case DamageTypeDefinition.Dot:
                    a.dotDamage = EditorGUILayout.FloatField("Damages", a.dotDamage);
                    a.dotDuration = EditorGUILayout.FloatField("Duration", a.dotDuration);
                    break;
                default:
                    break;
            }
            GUILayout.Space(10);

            GUILayout.Label("Critic", EditorStyles.boldLabel);
            a.criticalMultiplier = EditorGUILayout.FloatField("Critical Multiplier", a.criticalMultiplier);
            a.criticalChance = EditorGUILayout.FloatField("Critical Multiplier", a.criticalChance);
        }
    }
}