using UnityEngine;

namespace Cursed.LevelEditor
{
    public class ColorMappings : ScriptableObject
    {
        public ColorToPrefab[] mapping;

        public GameObject GetPrefabFromColor(Color color)
        {
            foreach (ColorToPrefab m in mapping)
            {
                if (m.color.Equals(color))
                    return m.prefab;
            }
            
            Debug.LogError("Color not found");
            return null;
        }
    }
}