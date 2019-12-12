using UnityEngine;
using Cursed.Character;

namespace Cursed.Combat
{
    [System.Serializable]
    public abstract class DamageType_SO : ScriptableObject
    {
        [SerializeField] protected string _name = "New Damage Type";
        [SerializeField] protected Stat _modifier = Stat.FixedDamage;

        public abstract int GetDamages();
        public abstract void Effect(CharacterStats statsToAffect);

        public string Name { get => _name; set => _name = value; }
        public Stat Modifier => _modifier;
    }
}