using UnityEditor;
using UnityEngine;

namespace Cursed.LevelDesign
{
    [CustomEditor(typeof(TileMapColor))]
    public class TileMapColor_Editor : Editor
    {
        private TileMapColor _tileMapColor = null;

        private void OnEnable() => _tileMapColor = (TileMapColor)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update Tiles Color"))
                _tileMapColor.UpdateTilesColor();

            base.OnInspectorGUI();
        }
    }
}
