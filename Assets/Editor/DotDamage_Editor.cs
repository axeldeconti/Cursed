using UnityEditor;

namespace Cursed.Combat
{
    [CustomEditor(typeof(DotDamage))]
    public class DotDamage_Editor : Editor
    {
        private DotDamage _dotDamage;

        private void OnEnable()
        {
            _dotDamage = target as DotDamage;
        }

        public override void OnInspectorGUI()
        {
            _dotDamage.Name = EditorGUILayout.TextField("Name", _dotDamage.Name);
            _dotDamage.DamagePerSecond = EditorGUILayout.FloatField("Damage Per Second", _dotDamage.DamagePerSecond);
            _dotDamage.Duration = EditorGUILayout.FloatField("Duration", _dotDamage.Duration);
        }
    }
}