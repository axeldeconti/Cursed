using UnityEngine;

namespace Cursed.LevelEditor
{
    public class LevelGenerator : MonoBehaviour
    {
        private Texture2D map;// a remplacer

        [Header("Mapping")]
        [SerializeField] private ColorMappings _mapping = null;

        [Header("Maps")]
        [SerializeField] private Level_SO[] _levelsOne = null;
        [SerializeField] private Level_SO[] _levelsTwo = null;
        [SerializeField] private Level_SO[] _levelsThree = null;

        public void GenerateLevel()
        {
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    GenerateTile(x, y);
                }
            }
        }

        private void GenerateTile(int x, int y)
        {
            Color pixelColor = map.GetPixel(x, y);

            //If pixel is transparent, ignore it
            if (pixelColor.a == 0)
                return;

            GameObject prefab = _mapping.GetPrefabFromColor(pixelColor);
            Vector2 position = new Vector2(x, y);
            Instantiate(prefab, position, Quaternion.identity, transform);
        }
    }
}