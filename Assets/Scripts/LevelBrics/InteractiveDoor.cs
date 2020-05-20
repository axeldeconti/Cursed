using UnityEngine;

public class InteractiveDoor : MonoBehaviour
{
    [SerializeField] private bool _activeDoor = true;
    private Animator _animator;

    public event System.Action<bool> doorToggle;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetBool("Toggle", _activeDoor);
        doorToggle?.Invoke(_activeDoor);
        if (_activeDoor)
            AkSoundEngine.PostEvent("Play_PuzzleDoor_Open", gameObject);
        if (!_activeDoor)
            AkSoundEngine.PostEvent("Play_PuzzleDoor_Close", gameObject);
    }

    public void ToggleDoor()
    {
        _activeDoor = !_activeDoor;
        _animator.SetBool("Toggle", _activeDoor);
        doorToggle?.Invoke(_activeDoor);
        if (_activeDoor)
            AkSoundEngine.PostEvent("Play_PuzzleDoor_Open", gameObject);
        if (!_activeDoor)
            AkSoundEngine.PostEvent("Play_PuzzleDoor_Close", gameObject);
    }
}
