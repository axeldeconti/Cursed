﻿using System;
using Cursed.Character;

namespace Cursed.Combat
{
    public class Attack
    {
        private readonly int _damage;
        private readonly bool _critical;
        private Action<CharacterStats> _effect;

        public Attack(int damage, bool critical, Action<CharacterStats> effect)
        {
            if(damage < 0)
            {
                throw new ArgumentOutOfRangeException("damage", "Damage amount can't be negative");
            }

            _damage = damage;
            _critical = critical;
            _effect = effect;
        }

        public int Damage => _damage;
        public bool IsCritical => _critical;
        public Action<CharacterStats> Effect => _effect;
    }
}