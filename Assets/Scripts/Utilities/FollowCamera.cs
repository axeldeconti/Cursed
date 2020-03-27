using UnityEngine;
using Cinemachine;

namespace Cursed.Utilities
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        private void Update()
        {
            transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0);
        }
    }
}
