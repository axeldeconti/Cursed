using UnityEngine;

namespace Cursed.Traps
{
    public class EndLaserBeam : MonoBehaviour
    {
        public LaserBeam _laserBeam { get; private set; }

        private void Awake()
        {
            _laserBeam = GetComponentInParent<LaserBeam>();
        }
    }
}


