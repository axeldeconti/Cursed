using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Character;

public class SFXHandler : MonoBehaviour
{
    private Animator _anim = null;

    private void Start()
    {
        _anim = GetComponent<Animator>();
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
    #endregion

    #region Creature
    public void Launch()
    {
        AkSoundEngine.PostEvent("Play_Creature_Launch", gameObject);
    }

    public void Call()
    {
        AkSoundEngine.PostEvent("Play_Creature_Call", gameObject);
    }

    public void Grabbing()
    {
        AkSoundEngine.PostEvent("Play_Creature_Grabbing", gameObject);
    }

    public void HitWall()
    {
        AkSoundEngine.PostEvent("Play_Creature_HitWall", gameObject);
    }
    #endregion
}
