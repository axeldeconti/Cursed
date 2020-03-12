using UnityEngine;

namespace Cursed.Traps
{
    public class LaserBeam : Trap
    {
        private enum _lasertype { laser, multiLaserVertical, multilaserHorizontal }
        [SerializeField] private _lasertype _laserType;

        [Header("Data")]
        [SerializeField] private Transform _start = null;
        [SerializeField] private Transform _end = null;

        [SerializeField] private FloatReference _colliderSize = null;

        private BoxCollider2D _coll = null;


        public void UpdateCollider()
        {
            if (!_coll)
                _coll = GetComponent<BoxCollider2D>();

            if(_laserType == _lasertype.multiLaserVertical || _laserType == _lasertype.laser)
            {
                _coll.offset = new Vector2(0, (_end.localPosition.y + _start.localPosition.y) / 2);
                _coll.size = new Vector2(_colliderSize, _end.localPosition.y - _start.localPosition.y);
            }
            if (_laserType == _lasertype.multilaserHorizontal)
            {
                _coll.offset = new Vector2((_end.localPosition.x + _start.localPosition.x) / 2, 0);
                _coll.size = new Vector2(_end.localPosition.x - _start.localPosition.x, _colliderSize);
            }

        }

        public float ColliderSize => _colliderSize;

    }
}