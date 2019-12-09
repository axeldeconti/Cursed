using Cursed.Combat;
using NUnit.Framework;
using System;

namespace Tests
{
    public class Attack_Test
    {
        [Test]
        public void Attack_DamageAmountNegative_Exception()
        {
            //Assert.Throws<ArgumentOutOfRangeException>(new Attack(-1, false, null));
        }
    }
}