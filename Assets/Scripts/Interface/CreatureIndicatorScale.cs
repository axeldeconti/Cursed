using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class CreatureIndicatorScale : MonoBehaviour
    {
        private Transform _creature;
        private Transform _player;
        private float _size;
        private float _initialSize;

        private void Awake()
        {
            _initialSize = GetComponent<RectTransform>().sizeDelta.x;
        }

        private void Start()
        {
            _creature = GameObject.FindGameObjectWithTag("Creature").transform;
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            float distance = GetDistancePlayerFromCreature();
            float _size = 140 - distance;

            if (_size < 50f)
                _size = 50f;
            if (_size >= _initialSize)
                _size = _initialSize;

            GetComponent<RectTransform>().sizeDelta = new Vector2(_size, _size);
        }

        private float GetDistancePlayerFromCreature()
        {
            return Vector2.Distance(_creature.position, _player.position);
        }
    }
}
