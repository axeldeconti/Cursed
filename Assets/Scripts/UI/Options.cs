using UnityEngine;
using UnityEngine.UI;
using Cursed.Managers;

namespace Cursed.UI
{
    public class Options : MonoBehaviour
    {
        [Header("Vibrations")]
        [SerializeField] private Image _vibrationImg;
        [SerializeField] private Sprite _checkSquare;
        [SerializeField] private Sprite _uncheckSquare;

        [Header("Volume")]
        [SerializeField] private Slider _mainVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        private void Start()
        {
            if(OptionsManager.Instance.VibrationActive)
                _vibrationImg.sprite = _checkSquare;
            else
                _vibrationImg.sprite = _uncheckSquare;

            _mainVolumeSlider.value = OptionsManager.Instance.MainVolume;
            _musicVolumeSlider.value = OptionsManager.Instance.MusicVolume;
            _sfxVolumeSlider.value = OptionsManager.Instance.SFXVolume;
        }

        public void ToggleVibrations()
        {
            OptionsManager.Instance.SetVibration(!OptionsManager.Instance.VibrationActive);

            if (OptionsManager.Instance.VibrationActive)
                _vibrationImg.sprite = _checkSquare;
            else
                _vibrationImg.sprite = _uncheckSquare;
        }

        public void UpdateMainVolume(float volume)
        {
            OptionsManager.Instance.SetMainVolumeTo(volume);
        }
        public void UpdateMusicVolume(float volume)
        {
            OptionsManager.Instance.SetMusicVolumeTo(volume);
        }
        public void UpdateSFXVolume(float volume)
        {
            OptionsManager.Instance.SetSFXVolumeTo(volume);
        }
    }
}
