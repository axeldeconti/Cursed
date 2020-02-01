using UnityEngine;

namespace Cursed.LevelEditor
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Level Editor/New Level")]
    public class Level_SO : ScriptableObject
    {
        public ColorMappings levelMapping = null;
        public Texture2D levelLayer = null;
        public ColorMappings propMapping = null;
        public Texture2D[] propLayers = null;
        public ColorMappings ennemyMapping = null;
        public Texture2D enemyLayer = null;
        public ColorMappings backgroundMapping = null;
        public Texture2D backgroundLayer = null;
    }
}