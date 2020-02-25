using UnityEditor;
using UnityEngine;

namespace Cursed.Traps
{
    [CustomEditor(typeof(LaserBeam))]
    public class LaserBeam_Editor : Editor
    {
        private LaserBeam _laser = null;

        private void OnEnable() => _laser = (LaserBeam)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update Collider"))
                _laser.UpdateCollider();

            base.OnInspectorGUI();

            //_laser.UpdateCollider();
        }


    }
}