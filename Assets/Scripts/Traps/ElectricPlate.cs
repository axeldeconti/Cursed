using Cursed.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Traps
{
    public class ElectricPlate : Trap
    {
        [Header("Data")]
        [SerializeField] private FloatReference _timBetweenDamage = null;
        [SerializeField] private FloatReference _activationTime = null;
        [SerializeField] private FloatReference _deactivationTime = null;
        [SerializeField] private bool _isActive = false;
        [SerializeField] private bool _isActivating = false;

        private Dictionary<IAttackable, float> _currentAttackables = null;

        private void Start()
        {
            _currentAttackables = new Dictionary<IAttackable, float>();
        }

        private void Update()
        {
            //Don't update if no one is in the trap
            if (_currentAttackables.Count == 0)
                return;

            if (!_isActive)
                return;

            foreach (IAttackable a in _currentAttackables.Keys)
            {
                _currentAttackables[a] -= Time.deltaTime;

                if (_currentAttackables[a] <= 0)
                {
                    _currentAttackables[a] = _timBetweenDamage;
                    a.OnAttack(gameObject, _attack.CreateAttack());
                }
            }
        }

        protected override void InflinctDamage(Component[] attackables)
        {
            foreach (IAttackable a in attackables)
            {
                if (!_currentAttackables.ContainsKey(a))
                {
                    _currentAttackables.Add(a, _timBetweenDamage);
                    a.OnAttack(gameObject, _attack.CreateAttack());
                }
            }

            if (!_isActivating && !_isActive)
                StartCoroutine(Activation());
        }

        private IEnumerator Activation()
        {
            _isActivating = true;
            yield return new WaitForSeconds(_activationTime);
            _isActivating = false;
            _isActive = true;

            //Inflict first damage
            foreach (IAttackable a in _currentAttackables.Keys)
            {
                a.OnAttack(gameObject, _attack.CreateAttack());
            }
        }

        private IEnumerator Deactivation()
        {
            bool still = true;
            float timer = _deactivationTime;
            bool hasEnded = true;

            while (still)
            {
                if (timer <= 0)
                    break;

                if (_currentAttackables.Count != 0)
                {
                    hasEnded = false;
                    break;
                }

                timer -= Time.deltaTime;
                yield return null;
            }

            if (hasEnded)
                _isActive = false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Component[] attackables = collision.GetComponentsInChildren(typeof(IAttackable));

            foreach (IAttackable a in attackables)
            {
                if (_currentAttackables.ContainsKey(a))
                    _currentAttackables.Remove(a);
            }

            if (_currentAttackables.Count == 0)
                StartCoroutine(Deactivation());
        }

        public bool IsActive => _isActive;
        public bool IsActivating => _isActivating;
    }
}