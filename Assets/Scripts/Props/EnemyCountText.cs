using TMPro;
using UnityEngine;

namespace Cursed.Props
{
    public class EnemyCountText : MonoBehaviour
    {
        private TMP_Text _text;
        [SerializeField] private EnemyCount.CountType _enemyCountType;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            EnemyCount.Instance.enemyCountUpdate += () => UpdateText();
        }

        public void UpdateText()
        {
            _text.text = EnemyCount.Instance.GetEnemyCount(_enemyCountType).ToString();
        }
    }
}