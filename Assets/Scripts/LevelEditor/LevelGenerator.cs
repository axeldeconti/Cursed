using UnityEngine;

namespace Cursed.LevelEditor
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private int _levelLayerDepth;
        [SerializeField] private int _bgLayerDepth;

        [Header("Maps")]
        [SerializeField] private Level_SO[] _levelsOne = null;
        [SerializeField] private Level_SO[] _levelsTwo = null;
        [SerializeField] private Level_SO[] _levelsThree = null;

        public void GenerateLevel(int levelNumber)
        {
            Level_SO currentLevel = null;

            //Pick a random level
            switch (levelNumber)
            {
                case 1:
                    currentLevel = _levelsOne[Random.Range(0, _levelsOne.Length)];
                    break;
                case 2:
                    currentLevel = _levelsTwo[Random.Range(0, _levelsTwo.Length)];
                    break;
                case 3:
                    currentLevel = _levelsThree[Random.Range(0, _levelsThree.Length)];
                    break;
                default:
                    Debug.LogError("Level layer wrong, must be 1, 2 or 3");
                    break;
            }

            //Generate background
            GenerateLayer(currentLevel.backgroundLayer, currentLevel.backgroundMapping, Vector2.zero, _bgLayerDepth);

            //Generate level
            GenerateLayer(currentLevel.levelLayer, currentLevel.levelMapping, Vector3.zero, _levelLayerDepth);

            //Generate props
            int i = Random.Range(0, currentLevel.propLayers.Length);
            GenerateLayer(currentLevel.propLayers[i], currentLevel.propMapping, Vector3.zero, _levelLayerDepth);

            //Generate ennemy
            GenerateLayer(currentLevel.enemyLayer, currentLevel.ennemyMapping, Vector3.zero, _levelLayerDepth);
        }

        /// <summary>
        /// Generate one layer for the level
        /// </summary>
        /// <param name="map">Map of the level to generate</param>
        /// <param name="mapping">Mapping associated to the level</param>
        /// <param name="center">Center of the level</param>
        /// <param name="z">Depth of the layer</param>
        private void GenerateLayer(Texture2D map, ColorMappings mapping, Vector2 center, int z)
        {
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    Color pixelColor = map.GetPixel(x, y);

                    //If pixel is transparent, ignore it
                    if (pixelColor.a != 0)
                        GenerateTile((int)center.x + x, (int)center.y + y, z, pixelColor, mapping);
                }
            }
        }

        /// <summary>
        /// Spawn a tile at the correct position
        /// </summary>
        /// <param name="x">Postition on x</param>
        /// <param name="y">Position on y</param>
        /// <param name="z">Postion on z</param>
        /// <param name="pixelColor">Pixel color of the tile to spawn</param>
        /// <param name="mapping">Mapping to know wich prefab to spawn</param>
        private void GenerateTile(int x, int y, int z, Color pixelColor, ColorMappings mapping)
        {
            GameObject prefab = mapping.GetPrefabFromColor(pixelColor);
            Vector2 position = new Vector3(x, y, z);
            Instantiate(prefab, position, Quaternion.identity, transform);
        }
    }
}