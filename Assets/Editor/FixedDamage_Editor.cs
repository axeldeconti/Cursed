using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cursed.Combat
{
    [CustomEditor(typeof(FixedDamage))]
    public class FixedDamage_Editor : Editor
    {
        private FixedDamage _fixedDamage;

        private void OnEnable()
        {
            _fixedDamage = target as FixedDamage;
        }

        public override void OnInspectorGUI()
        {
            _fixedDamage.Name = EditorGUILayout.TextField("Name", _fixedDamage.Name);
            _fixedDamage.Damage = EditorGUILayout.IntField("Damage", _fixedDamage.Damage);
            EditorUtility.SetDirty(_fixedDamage);
        }
    }
}