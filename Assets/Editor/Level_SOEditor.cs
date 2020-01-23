using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cursed.LevelEditor
{
    [CustomEditor(typeof(Level_SO))]
    public class Level_SOEditor : Editor
    {
        private SerializedObject _object;
        private SerializedProperty _propCount;

        private void OnEnable()
        {
            _object = new SerializedObject(target);
            _propCount = _object.FindProperty("propLayers.Array.size");
        }

        public override void OnInspectorGUI()
        {
            _object.Update();

            var lvl = target as Level_SO;
            var propLayerProperty = _object.FindProperty("propLayers");

            //Level layer
            GUILayout.Label("Level layer", EditorStyles.boldLabel);
            lvl.levelLayer = (Texture2D)EditorGUILayout.ObjectField(lvl.levelLayer, typeof(Texture2D), allowSceneObjects: true, GUILayout.Height(100f), GUILayout.Width(300f));

            //Enemy layer
            GUILayout.Space(20f);
            GUILayout.Label("Enemy layer", EditorStyles.boldLabel);
            lvl.enemyLayer = (Texture2D)EditorGUILayout.ObjectField(lvl.enemyLayer, typeof(Texture2D), allowSceneObjects: true, GUILayout.Height(100f), GUILayout.Width(300f));

            //Prop layer
            GUILayout.Space(20f);
            GUILayout.Label("Prop layers", EditorStyles.boldLabel);
            GUILayout.BeginVertical();
            for (int i = 0; i < propLayerProperty.arraySize; i++)
            {
                GUILayout.BeginHorizontal();

                var element = propLayerProperty.GetArrayElementAtIndex(i);
                lvl.propLayers[i] = (Texture2D)EditorGUILayout.ObjectField(lvl.propLayers[i], typeof(Texture2D), allowSceneObjects: true, GUILayout.Height(100f), GUILayout.Width(300f));

                if (GUILayout.Button("x", GUILayout.Width(20f)))
                {
                    RemoveMappingAtIndex(i);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add layer"))
            {
                _propCount.intValue++;
            }
            GUILayout.EndVertical();

            _object.ApplyModifiedProperties();
        }

        private void RemoveMappingAtIndex(int index)
        {
            var lvl = target as Level_SO;
            List<Texture2D> textures = new List<Texture2D>(lvl.propLayers);

            textures.RemoveAt(index);

            if (GUI.changed)
            {
                Undo.RecordObject(lvl, "Property modification");
                lvl.propLayers = textures.ToArray();
                EditorUtility.SetDirty(lvl);
            }
        }
    }
}