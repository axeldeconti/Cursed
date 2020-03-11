using UnityEditor;
using UnityEngine;
using Cursed.Item;

namespace Cursed.Combat
{
    [CustomEditor(typeof(Weapon))]
    public class Weapon_Editor : Editor
    {
        private Weapon _weapon;
        private SerializedObject _object;
        private SerializedProperty _damageType;
        private SerializedProperty _vfxTouchImpact;

        private void OnEnable()
        {
            _weapon = target as Weapon;
            _object = new SerializedObject(target);
            _damageType = _object.FindProperty("_damageType");
            _vfxTouchImpact = _object.FindProperty("_vfxTouchImpact");
        }

        public override void OnInspectorGUI()
        {
            _object.Update();

            //Pick up
            GUILayout.Label("Pick Up", EditorStyles.boldLabel);
            _weapon.Name = EditorGUILayout.TextField("Name", _weapon.Name);
            _weapon.Type = (PickUpType)EditorGUILayout.EnumPopup("Type", _weapon.Type);
            _weapon.SpawnChanceWeight = EditorGUILayout.IntField("Spawn Chance Weight", _weapon.SpawnChanceWeight);

            //Attack definition
            GUILayout.Space(10);
            GUILayout.Label("Data", EditorStyles.boldLabel);
            _weapon.WeaponType = (WeaponType)EditorGUILayout.EnumPopup("Type", _weapon.WeaponType);
            _weapon.Cooldown = EditorGUILayout.FloatField("Cooldown", _weapon.Cooldown);

            GUILayout.Space(10);
            GUILayout.Label("Damage", EditorStyles.boldLabel);
            _weapon.DamageType = (DamageType_SO)EditorGUILayout.ObjectField(_weapon.DamageType, typeof(DamageType_SO));
            if (_weapon.DamageType)
            {
                if (_weapon.DamageType as FixedDamage)
                    Editor.CreateEditor(_weapon.DamageType, typeof(FixedDamage_Editor)).OnInspectorGUI();
                else if (_weapon.DamageType as ForkDamage)
                    Editor.CreateEditor(_weapon.DamageType, typeof(ForkDamage_Editor)).OnInspectorGUI();
                else if (_weapon.DamageType as DotDamage)
                    Editor.CreateEditor(_weapon.DamageType, typeof(DotDamage_Editor)).OnInspectorGUI();
            }

            GUILayout.Space(10);
            GUILayout.Label("Critic", EditorStyles.boldLabel);
            _weapon.CriticalMultiplier = EditorGUILayout.FloatField("Critical Multiplier", _weapon.CriticalMultiplier);
            _weapon.CriticalChance = EditorGUILayout.FloatField("Critical Chance", _weapon.CriticalChance);

            //Vfx
            GUILayout.Space(10);
            GUILayout.Label("Vfx", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_vfxTouchImpact, true);

            EditorUtility.SetDirty(_weapon);
            _object.ApplyModifiedProperties();
        }
    }
}