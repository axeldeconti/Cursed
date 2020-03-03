using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Character;

public class SFXHandler : MonoBehaviour
{
    private Animator _anim = null;
    private CharacterMovement _move = null;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _move = GetComponentInParent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_move.IsDashing)
            AkSoundEngine.PostEvent("Play_Dash", gameObject);

    }
}
