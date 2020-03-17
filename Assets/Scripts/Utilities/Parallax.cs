using UnityEngine;
using Cinemachine;

namespace Cursed.Utilities
{
    public class Parallax : MonoBehaviour
    {
        private float _lengthX, _startPosX;
        private float _lengthY, _startPosY;
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private float _parallaxEffect;

        private void Start()
        {
            _startPosX = transform.position.x;
            _startPosY = transform.position.y;
            _lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
            _lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
        }

        private void FixedUpdate()
        {
            float tempX = (_camera.transform.position.x * (1 - _parallaxEffect));
            float distX = (_camera.transform.position.x * _parallaxEffect);
            float tempY = (_camera.transform.position.y * (1 - _parallaxEffect));
            float distY = (_camera.transform.position.y * _parallaxEffect);

            transform.position = new Vector3(_startPosX + distX, _startPosY + distY, transform.position.z);

            if (tempX > _startPosX + _lengthX)
                _startPosX += _lengthX;
            else if (tempX < _startPosX - _lengthX)
                _startPosX -= _lengthX;

            if (tempY > _startPosY + _lengthY)
                _startPosY += _lengthY;
            else if (tempY < _startPosY - _lengthY)
                _startPosY -= _lengthY;
        }
    }
}
