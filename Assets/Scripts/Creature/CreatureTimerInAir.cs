using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureTimerInAir : MonoBehaviour
    {
        [SerializeField] private FloatReference TimeBeforeComeBack;
        [SerializeField] private float _timer;

        private CreatureManager _creature;

        void Start()
        {
            _creature = GetComponent<CreatureManager>();
            _timer = TimeBeforeComeBack.Value;

        }
        void Update()
        {
            if (_creature.CurrentState == CreatureState.Moving  || _creature.CurrentState == CreatureState.OnWall)
                LaunchTimer();
            else
                ResetTimer();
        }

        private void LaunchTimer()
        {
            _timer -= Time.deltaTime;
            CheckTimer();
        }

        private void CheckTimer()
        {
            if (_timer <= 0f)
            {
                ResetTimer();
                _creature.CurrentState = CreatureState.OnComeBack;
            }
        }

        private void StopTimer()
        {
            float lastFrame = _timer;
            _timer = lastFrame;
        }

        private void ResetTimer()
        {
            _timer = TimeBeforeComeBack.Value;
        }
    }
}
