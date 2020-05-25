using Cursed.VisualEffect;
using UnityEngine;

namespace Cursed.Managers
{
    public class OptionsManager : Singleton<OptionsManager>
    {
        private float _mainVolume;
        private float _musicVolume;
        private float _sfxVolume;

        private bool _vibrationsActive;

        #region INIT
        private void Start()
        {
            SetMainVolumeTo(1f);
            SetMusicVolumeTo(1f);
            SetSFXVolumeTo(1f);
            SetVibration(true);
        }

        #endregion

        public float SetMainVolumeTo(float value)
        {
            _mainVolume = value;
            Debug.Log("Main volume set to " + value);

            AkSoundEngine.SetRTPCValue("Main_Slider", value);
            return _mainVolume;
        }

        public float SetMusicVolumeTo(float value)
        {
            _musicVolume = value;
            Debug.Log("Music volume set to " + value);

            AkSoundEngine.SetRTPCValue("Music_Slider", value);

            return _musicVolume;
        }

        public float SetSFXVolumeTo(float value)
        {
            _sfxVolume = value;
            Debug.Log("SFX volume set to " + value);

            AkSoundEngine.SetRTPCValue("SFX_Slider", value);

            return _sfxVolume;
        }

        public bool SetVibration(bool active)
        {
            _vibrationsActive = active;
            ControllerVibration.Instance.vibrationActive = active;
            Debug.Log("Vibrations set to " + active);
            return _vibrationsActive;
        }


        #region GETTERS & SETTERS
        public float MainVolume => _mainVolume;
        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;
        public bool VibrationActive
        {
            get => _vibrationsActive;
            set => _vibrationsActive = value;
        }

        #endregion
    }
}
