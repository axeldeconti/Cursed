using UnityEngine;

namespace Cursed.Traps
{
    public enum LaserType { laser, multiLaserVertical, multilaserHorizontal }

    public class LaserBeam : Trap
    {
        public LaserType _laserType;

        [Header("Data")]
        [SerializeField] private Transform _start = null;
        [SerializeField] private Transform _end = null;

        [SerializeField] private FloatReference _colliderSize = null;

        private BoxCollider2D _coll = null;


        public void UpdateCollider()
        {
            if (!_coll)
                _coll = GetComponent<BoxCollider2D>();

            if (_laserType == LaserType.multiLaserVertical || _laserType == LaserType.laser)
            {
                _coll.offset = new Vector2(0, (_end.localPosition.y + _start.localPosition.y) / 2);
                _coll.size = new Vector2(_colliderSize, _end.localPosition.y - _start.localPosition.y);
            }
            if (_laserType == LaserType.multilaserHorizontal)
            {
                _coll.offset = new Vector2((_end.localPosition.x + _start.localPosition.x) / 2, 0);
                _coll.size = new Vector2(_end.localPosition.x - _start.localPosition.x, _colliderSize);
            }

        }

        public float ColliderSize => _colliderSize;

    }
}