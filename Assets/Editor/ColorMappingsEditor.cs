using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cursed.LevelEditor
{
    [CustomEditor(typeof(ColorMappings))]
    public class ColorMappingsEditor : Editor
    {
        private SerializedObject _object;
        private SerializedProperty _mappingCount;

        private void OnEnable()
        {
            _object = new SerializedObject(target);
            _mappingCount = _object.FindProperty("mapping.Array.size");
        }

        public override void OnInspectorGUI()
        {
            _object.Update();

            GUILayout.Label("Mappings", EditorStyles.boldLabel);

            var arrayProperty = _object.FindProperty("mapping");

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                var element = arrayProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element, true);

                if(GUILayout.Button("x", GUILayout.Width(20f)))
                {
                    RemoveMappingAtIndex(i);
                }
                GUILayout.EndHorizontal();
            }

            if(GUILayout.Button("Add Mapping"))
            {
                _mappingCount.intValue++;
            }

            _object.ApplyModifiedProperties();
        }

        private void RemoveMappingAtIndex(int index)
        {
            var myObj = (ColorMappings)target;
            List<ColorToPrefab> mapping = new List<ColorToPrefab>(myObj.mapping);

            mapping.RemoveAt(index);

            if (GUI.changed)
            {
                Undo.RecordObject(myObj, "Property modification");
                myObj.mapping = mapping.ToArray();
                EditorUtility.SetDirty(myObj);
            }
        }
    }
}