using Cursed.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Traps
{
    public class ElectricPlate : Trap
    {
        [Header("Data")]
        [SerializeField] private FloatReference _timBetweenDamage = null;

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

            foreach (IAttackable a in _currentAttackables.Keys)
            {
                _currentAttackables[a] -= Time.deltaTime;

                if(_currentAttackables[a] <= 0)
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
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Component[] attackables = collision.GetComponentsInChildren(typeof(IAttackable));

            foreach (IAttackable a in attackables)
            {
                if (_currentAttackables.ContainsKey(a))
                    _currentAttackables.Remove(a);
            }
        }
    }
}