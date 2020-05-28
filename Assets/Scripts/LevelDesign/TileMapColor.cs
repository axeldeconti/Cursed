using UnityEngine;

namespace Cursed.LevelDesign
{
    public class TileMapColor : MonoBehaviour
    {
        [SerializeField] private Color _tileColor = new Color(.3f,.3f,.3f,1f);
        private SpriteRenderer[] _tilesSprite;

        public void UpdateTilesColor()
        {
            _tilesSprite = new SpriteRenderer[transform.childCount];
            for(int i = 0; i < _tilesSprite.Length; i++)
            {
                _tilesSprite[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
                _tilesSprite[i].color = _tileColor;
            }
        }
    }
}