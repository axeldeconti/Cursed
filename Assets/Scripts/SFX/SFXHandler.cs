using UnityEngine;
using Cursed.Character;
using Cursed.Traps;

public class SFXHandler : MonoBehaviour
{
    private Animator _anim = null;
    private CharacterAttackManager _attackManager = null;
    private HealthManager _healthManager = null;

    private bool _myIsDiveKicking;
    private bool _lowHealth1Played = false;
    private bool _lowHealth2Played = false;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _attackManager = GetComponent<CharacterAttackManager>();
        _healthManager = GetComponent<HealthManager>();

        #region Spatialized
        if (GetComponent<LaserBeam>())
            AkSoundEngine.PostEvent("Play_Laser", gameObject);

        /*if (GetComponent<MultiLaserScript>())
            AkSoundEngine.PostEvent("Play_MultiLaser", gameObject);*/

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
