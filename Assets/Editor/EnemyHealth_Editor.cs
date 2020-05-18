using UnityEngine;
using UnityEditor;

namespace Cursed.Character
{
    [CustomEditor(typeof(EnemyHealth))]
    public class EnemyHealth_Editor : Editor
    {
        private EnemyHealth _enemyHealth;

        private void OnEnable() => _enemyHealth = (EnemyHealth)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Kill Enemy"))
                _enemyHealth.Die();

            base.OnInspectorGUI();
        }
    }
}
