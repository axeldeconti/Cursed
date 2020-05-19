using UnityEngine;

namespace Cursed.AI
{
    public class AiTarget : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = Vector3.zero;

        public Vector3 Position => transform.position + _offset;
    }
}