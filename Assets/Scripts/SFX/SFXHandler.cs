using UnityEngine;
using Cursed.Character;
using Cursed.Creature;
using Cursed.Traps;
using System.Collections;

public class SFXHandler : MonoBehaviour
{
    private Animator _anim = null;
    private CharacterAttackManager _attackManager = null;
    private HealthManager _healthManager = null;
    private LaserBeam _laserBeam = null;
    private CreatureManager _creatureManager = null;
    private EnemyHealth _enemyHealth = null;

    private bool _myIsDiveKicking;
    private bool _lowHealth1Played = false;
    private bool _lowHealth2Played = false;
    private bool _wallslideIsPlaying = false;
    private bool _creatureOnCharisPlaying = false;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _attackManager = GetComponent<CharacterAttackManager>();
        _creatureManager = GetComponent<CreatureManager>();
        _healthManager = GetComponent<HealthManager>();
        _laserBeam = GetComponent<LaserBeam>();
        _enemyHealth = GetComponent<EnemyHealth>();

        #region Spatialized
        if (_laserBeam != null)
        {
            if (_laserBeam._laserType == LaserType.Laser)
                AkSoundEngine.PostEvent("Play_Laser", gameObject);

            if (_laserBeam._laserType == LaserType.MultilaserHorizontal || _laserBeam._laserType == LaserType.MultiLaserVertical)
                AkSoundEngine.PostEvent("Play_MultiLaser", gameObject);
        }

        if (GetComponent<ElectricPlate>())
            AkSoundEngine.PostEvent("Play_ElectricTrap_Inactive", gameObject);
        #endregion
    }
    
    private void Update()
    {
        if (GetComponent<CharacterAttackManager>() && !_attackManager.IsDiveKicking)
            _myIsDiveKicking = false;

        //CreatureOnCharacter sound
        if (_creatureManager != null)
        {
            if (_creatureManager.CurrentState == CreatureState.OnCharacter && !_creatureOnCharisPlaying)
            {
                _creatureOnCharisPlaying = true;
                AkSoundEngine.PostEvent("Play_Creature_OnChar", gameObject);
            }
            else if (_creatureManager.CurrentState != CreatureState.OnCharacter && _creatureOnCharisPlaying)
            {
                AkSoundEngine.PostEvent("Stop_Creature_OnChar", gameObject);
                _creatureOnCharisPlaying = false;
            }
        }

    }

    #region Character
    public void WallSlideBeginSFX()
    {
        if (!_wallslideIsPlaying)
        AkSoundEngine.PostEvent("Play_Main_WallSlide", gameObject);
        _wallslideIsPlaying = true;
        StartCoroutine(WallSlideEnum());
    }

    public void WallSlideEndSFX()
    {
        AkSoundEngine.PostEvent("Play_End_WallSlide", gameObject);
        _wallslideIsPlaying = false;
    }

    IEnumerator WallSlideEnum()
    {
        yield return new WaitForSeconds(4.841f);
        if (_wallslideIsPlaying)
        {
            AkSoundEngine.PostEvent("Stop_Main_Play_Loop_WallSlide", gameObject);
        }
    }

    public void RunSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Run", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Run", gameObject);
        }
    }

    public void WallRunSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Run", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Run", gameObject);
        }
    }

    public void JumpSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_BasicJump", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_BasicJump", gameObject);
        }        
    }

    public void DoubleJumpSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_DoubleJump", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_DoubleJump", gameObject);
        }
    }

    public void DashSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Dash", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Dash", gameObject);
        }
    }

    public void LandingSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Landing", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Landing", gameObject);
        }
    }

    #region Attack
    public void FirstSwordAttackSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Attack_Sword_FirstSlice", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Attack_Sword_FirstSlice", gameObject);
        }
    }

    public void SecondSwordAttackSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Attack_Sword_SecondSlice", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Attack_Sword_SecondSlice", gameObject);
        }
    }

    public void ThirdSwordAttackSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Attack_Sword_ThirdSlice", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Attack_Sword_ThirdSlice", gameObject);
        }
    }

    public void FirstAxeAttackSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Attack_Axe_FirstSlice", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Attack_Axe_FirstSlice", gameObject);
        }
    }

    public void SecondAxeAttackSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Attack_Axe_SecondSlice", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Attack_Axe_SecondSlice", gameObject);
        }
    }

    public void ThirdAxeAttackSFX()
    {
        if (_enemyHealth == null)
        {
            AkSoundEngine.PostEvent("Play_Attack_Axe_ThirdSlice", gameObject);
        }
        if (_enemyHealth != null)
        {
            AkSoundEngine.PostEvent("Play_Enemy_Attack_Axe_ThirdSlice", gameObject);
        }
    }

    public void DiveKickSFX()
    {
        if (_enemyHealth == null)
        {
            if (!_myIsDiveKicking)
            {
                AkSoundEngine.PostEvent("Play_Divekick", gameObject);
                _myIsDiveKicking = true;
            }
        }
        if (_enemyHealth != null)
        {
            if (!_myIsDiveKicking)
            {
                AkSoundEngine.PostEvent("Play_Enemy_Divekick", gameObject);
                _myIsDiveKicking = true;
            }
        }

    }
    #endregion

    public void PlayerDamageSFX()
    {
        AkSoundEngine.PostEvent("Play_Player_Damage", gameObject);
    }

    public void FirstEnemyDamageSFX()
    {
        AkSoundEngine.PostEvent("Play_Enemy_FirstDamage", gameObject);
    }

    public void SecondEnemyDamageSFX()
    {
        AkSoundEngine.PostEvent("Play_Enemy_SecondDamage", gameObject);
    }

    public void ThirdEnemyDamageSFX()
    {
        AkSoundEngine.PostEvent("Play_Enemy_ThirdDamage", gameObject);
    }

    public void PlayerDeathSFX()
    {
        AkSoundEngine.PostEvent("Play_Player_Death", gameObject);
    }

    public void EnemyDeathSFX()
    {
        AkSoundEngine.PostEvent("Play_Enemy_Death", gameObject);
        AkSoundEngine.PostEvent("Play_DeathAnnouncement", gameObject);
    }

    public void LowHealth()
    {
        //First LowHealth warning
        if(_healthManager.CurrentHealth <= _healthManager.MaxHealth * 30 / 100 && !_lowHealth1Played)
        {
            AkSoundEngine.PostEvent("Play_LowHealth1", gameObject);
            _lowHealth1Played = true;
        }
        if (_healthManager.CurrentHealth > _healthManager.MaxHealth * 30 / 100)
            _lowHealth1Played = false;

        //Second LowHealth warning
        if (_healthManager.CurrentHealth <= _healthManager.MaxHealth * 15 / 100 && !_lowHealth2Played)
        {
            AkSoundEngine.PostEvent("Play_LowHealth2", gameObject);
            _lowHealth2Played = true;
        }
        if (_healthManager.CurrentHealth > _healthManager.MaxHealth * 15 / 100)
            _lowHealth2Played = false;
}
    #endregion

    #region Creature
    public void LaunchSFX()
    {
        AkSoundEngine.PostEvent("Play_Creature_Launch", gameObject);
    }

    public void CallSFX()
    {
        AkSoundEngine.PostEvent("Play_Creature_Call", gameObject);
    }

    #endregion

}
