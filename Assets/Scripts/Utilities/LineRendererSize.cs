using UnityEngine;

namespace Cursed.Utilities
{
    public class LineRendererSize : MonoBehaviour
    {
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;
        private LineRenderer _line;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            //_line.SetWidth(.2f, .2f);
        }

        private void Start()
        {
            _line.SetPosition(0, _start.localPosition);
            _line.SetPosition(1, _end.localPosition);
        }

    }
}
