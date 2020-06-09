using Cursed.VisualEffect;
using Cursed.Utilities;
using UnityEngine;

namespace Cursed.Managers
{
    public class OptionsManager : Singleton<OptionsManager>
    {
        [SerializeField] private VibrationData_SO _activeVibration_SO;

        private float _mainVolume;
        private float _musicVolume;
        private float _sfxVolume;
        private float _ambianceVolume;

        private bool _vibrationsActive;

        #region INIT
        private void Start()
        {
            SetMainVolumeTo(1f);
            SetMusicVolumeTo(1f);
            SetSFXVolumeTo(1f);
            SetAmbianceVolumeTo(1f);

            // Vibration INIT
            _vibrationsActive = true;
            ControllerVibration.Instance.vibrationActive = true;
            Debug.Log("Vibrations set to " + true);
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

        public float SetAmbianceVolumeTo(float value)
        {
            _ambianceVolume = value;
            Debug.Log("Ambiance volume set to " + value);

            AkSoundEngine.SetRTPCValue("Ambiance_Slider", value);

            return _ambianceVolume;
        }

        public bool SetVibration(bool active)
        {
            _vibrationsActive = active;
            ControllerVibration.Instance.vibrationActive = active;
            LaunchVibration();
            Debug.Log("Vibrations set to " + active);
            return _vibrationsActive;
        }

        private void LaunchVibration()
        {
            ControllerVibration.Instance.StartVibration(_activeVibration_SO);
        }


        #region GETTERS & SETTERS
        public float MainVolume => _mainVolume;
        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;
        public float AmbianceVolume => _ambianceVolume;
        public bool VibrationActive
        {
            get => _vibrationsActive;
            set => _vibrationsActive = value;
        }

        #endregion
    }
}
