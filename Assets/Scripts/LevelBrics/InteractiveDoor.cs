using UnityEngine;

public class InteractiveDoor : MonoBehaviour
{
    [SerializeField] private bool _activeDoor = true;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetBool("Toggle", _activeDoor);
    }

    public void ToggleDoor()
    {
        _activeDoor = !_activeDoor;
        _animator.SetBool("Toggle", _activeDoor);
    }
}
