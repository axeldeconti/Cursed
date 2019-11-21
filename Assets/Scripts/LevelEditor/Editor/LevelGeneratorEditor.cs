using UnityEditor;
using UnityEngine;

namespace Cursed.LevelEditor
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGeneratorEditor : Editor
    {
        private LevelGenerator _gen;

        private void OnEnable()
        {
            _gen = target as LevelGenerator;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Level"))
            {
                _gen.GenerateLevel();
            }

            base.OnInspectorGUI();
        }
    }
}