using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Cursed.LevelEditor
{
    [CustomPropertyDrawer(typeof(ColorToPrefab))]
    public class MappingDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element
            var container = new VisualElement();

            // Create property fields
            var colorField = new PropertyField(property.FindPropertyRelative("color"));
            var prefabField = new PropertyField(property.FindPropertyRelative("prefab"));

            //Add fields to the container
            container.Add(colorField);
            container.Add(prefabField);

            return container;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var colorRect = new Rect(position.x, position.y, 100, position.height);
            var prefabRect = new Rect(position.x + 120, position.y, 180, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(colorRect, property.FindPropertyRelative("color"), GUIContent.none);
            EditorGUI.PropertyField(prefabRect, property.FindPropertyRelative("prefab"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}