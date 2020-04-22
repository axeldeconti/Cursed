using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Cursed.PostProcess
{
    public class ChromaticAberrationChanging : MonoBehaviour
    {
        [SerializeField] private float _lerpSpeed = 5f;

        private Volume _volume;
        private ChromaticAberration _chromaticAberration;

        private bool _launchLerp;
        private float _targetAberration;

        private void Awake()
        {
            _volume = GetComponent<Volume>();
        }

        private void Start()
        {
            ChromaticAberration chromaticAberration;

            if (_volume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
                _chromaticAberration = chromaticAberration;
        }

        private void Update()
        {
            if (_launchLerp)
                UpdateValue();
        }

        public void LerpToValue(float target)
        {
            _targetAberration = target;
            _launchLerp = true;
        }

        private void UpdateValue()
        {
            _chromaticAberration.intensity.value = Mathf.Lerp(_chromaticAberration.intensity.value, _targetAberration, _lerpSpeed * Time.deltaTime);

            if (_chromaticAberration.intensity.value == _targetAberration)
                _launchLerp = false;
        }
    }
}
