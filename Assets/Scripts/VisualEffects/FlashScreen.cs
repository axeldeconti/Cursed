using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.VisualEffect
{
    public class FlashScreen : MonoBehaviour
    {
        public CanvasGroup _uiElement;

        private void Start()
        {
            _uiElement.alpha = 0;
        }

        public void FlashScreenFadeOut(float lerpTime)
        {
            _uiElement.alpha = 1;
            StartCoroutine(FadeCanvasGroup(_uiElement, _uiElement.alpha, 0f, lerpTime));
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime)
        {
            float _timeStartedLerping = Time.time;
            float _timeSinceStarted = Time.time - _timeStartedLerping;
            float _percentageComplete = _timeSinceStarted / lerpTime;

            while (true)
            {
                _timeSinceStarted = Time.time - _timeStartedLerping;
                _percentageComplete = _timeSinceStarted / lerpTime;

                float currentValue = Mathf.Lerp(start, end, _percentageComplete);

                cg.alpha = currentValue;

                if (_percentageComplete >= 1) break;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}