using UnityEngine;

namespace Cursed.Traps
{
    public class LaserBeam : Trap
    {
        [Header("Data")]
        [SerializeField] private Transform _start = null;
        [SerializeField] private Transform _end = null;

        [SerializeField] private FloatReference _colliderSize = null;

        private BoxCollider2D _coll = null;

        private void Start() => _coll = GetComponent<BoxCollider2D>();

        public void UpdateCollider()
        {
            _coll.offset = new Vector2(0, (_end.localPosition.y + _start.localPosition.y) / 2);
            _coll.size = new Vector2(_colliderSize, _end.localPosition.y - _start.localPosition.y);
        }

        public float ColliderSize => _colliderSize;
    }
}