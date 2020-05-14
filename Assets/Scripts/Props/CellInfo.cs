using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cursed.Character;
using Cursed.UI;

public class CellInfo : MonoBehaviour
{
    public int cellNumberInfo;

    [HideInInspector] public bool _playerOnThisCell;
    public bool _emptyCell { get; private set; }
    public int _enemyCount { get; private set; }

    public event System.Action _onEnemyCellCountUpdate;

    private void CheckEnemyCount()
    {
        if (_enemyCount <= 0)
            _emptyCell = true;
        else
            _emptyCell = false;

        _onEnemyCellCountUpdate?.Invoke();
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
            _playerOnThisCell = true;
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
            _playerOnThisCell = false;
    }
}
