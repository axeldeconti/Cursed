using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cursed.Combat
{
    [CustomEditor(typeof(ForkDamage))]
    public class ForkDamage_Editor : Editor
    {
        private ForkDamage _forkDamage;

        private void OnEnable()
        {
            _forkDamage = target as ForkDamage;
        }

        public override void OnInspectorGUI()
        {
            _forkDamage.Name = EditorGUILayout.TextField("Name", _forkDamage.Name);
            _forkDamage.MinDamage = EditorGUILayout.IntField("Min Damage", _forkDamage.MinDamage);
            _forkDamage.MaxDamage = EditorGUILayout.IntField("Max Damage", _forkDamage.MaxDamage);
            EditorUtility.SetDirty(_forkDamage);
        }
    }
}