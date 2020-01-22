using UnityEngine;

namespace Cursed.Creature
{
    public class CreaturePartOnWall : MonoBehaviour
    {
        private SpriteMask _mask;
        private Transform _parent;

        private void Awake()
        {
            _parent = transform.parent;
            if (_parent.GetComponent<SpriteMask>() == null)
                _mask = _parent.gameObject.AddComponent(typeof(SpriteMask)) as SpriteMask;
            else
                _mask = _parent.GetComponent<SpriteMask>();

            _mask.sprite = _parent.GetComponent<SpriteRenderer>().sprite;
        }
    }
}
