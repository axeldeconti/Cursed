using UnityEngine;

public class DoorSwitchBattery : MonoBehaviour
{
    private DoorSwitch _doorSwitchParent;
    private Animator _animator;

    private bool _toggle = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _doorSwitchParent = GetComponentInParent<DoorSwitch>();
    }

    private void Start()
    {
        _doorSwitchParent._toggleDoor += () => ToggleFilledBattery();
    }

    private void ToggleFilledBattery()
    {
        _toggle = !_toggle;

        _animator.SetBool("UnFill", _toggle);
        _animator.SetBool("ReFill", !_toggle);
    }
}
