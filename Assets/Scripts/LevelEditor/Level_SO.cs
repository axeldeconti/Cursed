using UnityEngine;

namespace Cursed.LevelEditor
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Level Editor/New Level")]
    public class Level_SO : ScriptableObject
    {
        public Texture2D levelLayer = null;
        public Texture2D[] propLayers = null;
        public Texture2D enemyLayer = null;        
    }
}