using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Cursed.UI
{
    public class TestRoomIntro : MonoBehaviour
    {
        [Header("Post Process")]
        [SerializeField] private Volume _globalVolume = null;
        private DepthOfField _depthOfField;

        private void Start()
        {
            // SET BLUR EFFECT 
            if (_globalVolume != null)
            {
                DepthOfField depthOfField;
                if (_globalVolume.profile.TryGet<DepthOfField>(out depthOfField))
                    _depthOfField = depthOfField;
            }
        }

        public void SetDOF()
        {
            _depthOfField.mode.value = DepthOfFieldMode.Gaussian;
        }

        public void UnsetDOF()
        {
            _depthOfField.mode.value = DepthOfFieldMode.Off;
        }
    }
}
