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
        private Animator _animator;

        private List<ElectricPlateAttackable> _currentAttackables = null;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _currentAttackables = new List<ElectricPlateAttackable>();
        }

        private void Update()
        {
            //Don't update if no one is in the trap
            if (_currentAttackables.Count == 0)
                return;

            if (!_isActive)
                return;

            foreach (ElectricPlateAttackable a in _currentAttackables)
            {
                a.time -= Time.deltaTime;

                if (a.time <= 0)
                {
                    a.time = _timBetweenDamage;
                    a.attackable.OnAttack(gameObject, _attack.CreateAttack());
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Component[] attackables = collision.GetComponentsInChildren(typeof(IAttackable));

            foreach (IAttackable a in attackables)
            {
                if (!HasAttackable(a))
                {
                    _currentAttackables.Add(new ElectricPlateAttackable(a, _timBetweenDamage));

                    if (_isActive)
                        a.OnAttack(gameObject, _attack.CreateAttack());
                }
            }

            if (!_isActivating && !_isActive)
                StartCoroutine(Activation());
        }

        private bool HasAttackable(IAttackable attackable)
        {
            foreach (ElectricPlateAttackable a in _currentAttackables)
            {
                if (a.attackable == attackable)
                    return true;
            }

            return false;
        }

        private IEnumerator Activation()
        {
            _isActivating = true;

            //Launch animation
            _animator.SetBool("Enter", true);
            _animator.SetBool("Exit", false);

            yield return new WaitForSeconds(_activationTime);
            _isActivating = false;
            _isActive = true;

            //Inflict first damage
            foreach (ElectricPlateAttackable a in _currentAttackables)
            {
                a.attackable.OnAttack(gameObject, _attack.CreateAttack());
            }
        }

        private IEnumerator Deactivation()
        {
            bool still = true;
            float timer = _deactivationTime;
            bool hasEnded = true;

            //Launch animation
            _animator.SetBool("Enter", false);
            _animator.SetBool("Exit", true);
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
                if (HasAttackable(a))
                    _currentAttackables.Remove(GetAttackable(a));
            }

            if (_currentAttackables.Count == 0)
                StartCoroutine(Deactivation());
        }

        private ElectricPlateAttackable GetAttackable(IAttackable attackable)
        {
            foreach (ElectricPlateAttackable a in _currentAttackables)
            {
                if (a.attackable == attackable)
                    return a;
            }

            return null;
        }

        public bool IsActive => _isActive;
        public bool IsActivating => _isActivating;

        private class ElectricPlateAttackable
        {
            public IAttackable attackable = null;
            public float time = 0;

            public ElectricPlateAttackable(IAttackable attackable, float time)
            {
                this.attackable = attackable;
                this.time = time;
            }
        }
    }
}