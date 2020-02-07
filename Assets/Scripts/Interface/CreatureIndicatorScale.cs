using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class CreatureIndicatorScale : MonoBehaviour
    {
        private Transform _creature;
        private Transform _player;
        private float _size;

        private void Awake()
        {
            _creature = GameObject.FindGameObjectWithTag("Creature").transform;
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            float distance = GetDistancePlayerFromCreature();
            float _size = 175 - distance;
            if (_size < 50f)
                _size = 50f;
            GetComponent<RectTransform>().sizeDelta = new Vector2(_size, _size);
        }

        private float GetDistancePlayerFromCreature()
        {
            return Vector2.Distance(_creature.position, _player.position);
        }
    }
}
