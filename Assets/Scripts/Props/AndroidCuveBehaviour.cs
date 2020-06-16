﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Cursed.Managers;

namespace Cursed.Props
{
    public class AndroidCuveBehaviour : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController _normalCuve;
        [SerializeField] private RuntimeAnimatorController _brokenCuve;
        [SerializeField] private VoidEvent _cuveBroken;

        private Animator _animator;

        private ControlerManager _controlerManager;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _controlerManager = ControlerManager.Instance;
        }

        private void Update()
        {
            if (_controlerManager._ControlerType == ControlerManager.ControlerType.XBOX || _controlerManager._ControlerType == ControlerManager.ControlerType.None)
            {
                if (Input.GetButtonDown("Attack_1"))
                    CheckScene();
            }
            else if(_controlerManager._ControlerType == ControlerManager.ControlerType.PS4)
            {
                if (Input.GetButtonDown("Attack_1_PS4"))
                    CheckScene();
            }
        }

        public void CheckScene()
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                UpdateAnimator(_normalCuve);
            }
            else if (SceneManager.GetActiveScene().name == "Tuto" || SceneManager.GetActiveScene().name == "Intro")
            {
                UpdateAnimator(_brokenCuve);
                _cuveBroken?.Raise();
            }
        }

        private void UpdateAnimator(RuntimeAnimatorController newAnimator)
        {
            _animator.runtimeAnimatorController = newAnimator;
        }
    }
}
