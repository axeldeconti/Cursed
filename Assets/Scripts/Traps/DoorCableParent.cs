using UnityEngine;

public class DoorCableParent : MonoBehaviour
{
    [SerializeField] private InteractiveDoor _interactiveDoor;
    private SpriteRenderer[] _cablesSprites;

    [Header("Cable Color")]
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _deactiveColor;

    private void Awake()
    {
        _cablesSprites = new SpriteRenderer[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _cablesSprites[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }

        _interactiveDoor.doorToggle += UpdateCableColor;
    }

    private void UpdateCableColor(bool toggle)
    {
        toggle = !toggle;
        if (toggle)
        {
            for (int i = 0; i < _cablesSprites.Length; i++)
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
