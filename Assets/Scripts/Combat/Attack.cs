namespace Cursed.Combat
{
    public class Attack
    {
        private readonly int _damage;
        private readonly bool _critical;
        private readonly float _duration;

        public Attack(int damage, bool critical, float duration)
        {
            _damage = damage;
            _critical = critical;
            _duration = duration;
        }

        public int Damage => _damage;
        public bool IsCritical => _critical;
        public float Duration => _duration;
    }
}