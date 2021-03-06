﻿using UnityEngine;
using Cursed.Character;
using Cursed.Traps;
using System.Collections;

public class SFXHandler : MonoBehaviour
{
    private Animator _anim = null;
    private CharacterAttackManager _attackManager = null;
    private HealthManager _healthManager = null;
    private LaserBeam _laserBeam = null;

    private bool _myIsDiveKicking;
    private bool _lowHealth1Played = false;
    private bool _lowHealth2Played = false;
    private bool _wallslideIsPlaying = false;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _attackManager = GetComponent<CharacterAttackManager>();
        _healthManager = GetComponent<HealthManager>();
        _laserBeam = GetComponent<LaserBeam>();

        #region Spatialized
        if (_laserBeam != null)
        {
            if (_laserBeam._laserType == LaserType.laser)
                AkSoundEngine.PostEvent("Play_Laser", gameObject);

            if (_laserBeam._laserType == LaserType.multilaserHorizontal || _laserBeam._laserType == LaserType.multiLaserVertical)
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
        AkSoundEngine.PostEvent("Play_Run", gameObject);
    }

    public void WallRunSFX()
    {
        AkSoundEngine.PostEvent("Play_Run", gameObject);
    }

    public void JumpSFX()
    {
        AkSoundEngine.PostEvent("Play_BasicJump", gameObject);
    }

    public void DoubleJumpSFX()
    {
        AkSoundEngine.PostEvent("Play_DoubleJump", gameObject);
    }

    public void DashSFX()
    {
        AkSoundEngine.PostEvent("Play_Dash", gameObject);
    }

    public void LandingSFX()
    {
        AkSoundEngine.PostEvent("Play_Landing", gameObject);
    }

    public void SwordAttackSFX()
    {
        AkSoundEngine.PostEvent("Play_Attack_Sword_FirstSlice", gameObject);
    }

    public void AxeAttackSFX()
    {
        AkSoundEngine.PostEvent("Play_Attack_Axe_FirstSlice", gameObject);
    }

    public void DiveKickSFX()
    {
        if (!_myIsDiveKicking)
        {
            AkSoundEngine.PostEvent("Play_Divekick", gameObject);
            _myIsDiveKicking = true;
        }
    }

    public void PlayerDamageSFX()
    {
        AkSoundEngine.PostEvent("Play_Player_Damage", gameObject);
    }

    public void EnemyDamageSFX()
    {
        AkSoundEngine.PostEvent("Play_Enemy_Damage", gameObject);
    }

    public void PlayerDeathSFX()
    {
        AkSoundEngine.PostEvent("Play_Player_Death", gameObject);
    }

    public void EnemyDeathSFX()
    {
        AkSoundEngine.PostEvent("Play_Enemy_Death", gameObject);
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
