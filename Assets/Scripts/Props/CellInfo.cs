using System;
using UnityEngine;
using Cursed.Character;
using Cursed.UI;
using Cursed.Managers;

namespace Cursed.Props
{
    public class CellInfo : MonoBehaviour
    {
        public int cellNumberInfo;
        [SerializeField] private Cell _cell = Cell.A1;

        [HideInInspector] public bool _playerOnThisCell;
        public bool _emptyCell { get; private set; }
        public int _enemyCount { get; private set; }

        public event Action onEnemyCellCountUpdate;
        public Action<Cell> onPlayerEnterCell;

        private void CheckEnemyCount()
        {
            if (_enemyCount <= 0)
                _emptyCell = true;
            else
                _emptyCell = false;

            onEnemyCellCountUpdate?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<EnemyRegister>())
            {
                _enemyCount++;
                CheckEnemyCount();
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerOnCell(true);

                foreach (MapCellUI cell in FindObjectsOfType<MapCellUI>())
                {
                    if (this == cell._myCell)
                        cell.PlayerOnMyCell();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<EnemyRegister>())
            {
                _enemyCount--;
                CheckEnemyCount();
            }

            if (collision.gameObject.CompareTag("Player"))
                PlayerOnCell(false);
        }

        private void PlayerOnCell(bool value)
        {
            _playerOnThisCell = value;

            if (value)
                onPlayerEnterCell.Invoke(_cell);
        }
    }
}