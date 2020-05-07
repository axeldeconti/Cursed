using UnityEngine;

public class InteractiveDoor : MonoBehaviour
{
    [SerializeField] private bool _activeDoor = true;
    [SerializeField] private GameObject _cablesLink;

    [Header("Cable Color")]
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _deactiveColor;

    private Animator _animator;
    private SpriteRenderer[] _cablesSprites;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _cablesSprites = new SpriteRenderer[_cablesLink.transform.childCount];

        for(int i = 0; i < _cablesLink.transform.childCount; i++)
        {
            _cablesSprites[i] = _cablesLink.transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        _animator.SetBool("Toggle", _activeDoor);
        UpdateCablesColor();
    }

    public void ToggleDoor()
    {
        _activeDoor = !_activeDoor;
        _animator.SetBool("Toggle", _activeDoor);
        UpdateCablesColor();
    }

    private void UpdateCablesColor()
    {
        if(_activeDoor)
        {
            for(int i = 0; i < _cablesSprites.Length; i++)
            {
                _cablesSprites[i].color = _activeColor;
            }
        }
        else
        {
            for (int i = 0; i < _cablesSprites.Length; i++)
            {
                _cablesSprites[i].color = _deactiveColor;
            }
        }
    }
}
