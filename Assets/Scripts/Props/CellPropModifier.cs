using UnityEngine;
using UnityEngine.Events;
using Cursed.Managers;
using System.Collections.Generic;

namespace Cursed.Props
{
    public class CellPropModifier : MonoBehaviour
    {
        [SerializeField] private Cell _cell = Cell.A1;

        [Header("Events")]
        [SerializeField] private UnityEvent _onActivate;
        [SerializeField] private UnityEvent _onDeactivate;

        private bool _isActivated = false;

        private void Awake()
        {
            _isActivated = false;
        }

        private void Start()
        {
            CellManager.Instance.onPlayerEnterCell += OnPlayerEnterCellCallback;
        }

        private void OnPlayerEnterCellCallback(Cell cell)
        {
            List<Cell> cells = new List<Cell>();

            //Fill cells
            switch (cell)
            {
                case Cell.A1:
                    cells.Add(Cell.A1);
                    cells.Add(Cell.B1);
                    cells.Add(Cell.A2);
                    break;
                case Cell.B1:
                    cells.Add(Cell.A1);
                    cells.Add(Cell.B1);
                    cells.Add(Cell.C1);
                    cells.Add(Cell.B2);
                    break;
                case Cell.C1:
                    cells.Add(Cell.B1);
                    cells.Add(Cell.C1);
                    cells.Add(Cell.C2);
                    break;
                case Cell.A2:
                    cells.Add(Cell.A1);
                    cells.Add(Cell.A2);
                    cells.Add(Cell.B2);
                    cells.Add(Cell.A3);
                    break;
                case Cell.B2:
                    cells.Add(Cell.B1);
                    cells.Add(Cell.A2);
                    cells.Add(Cell.B2);
                    cells.Add(Cell.C2);
                    cells.Add(Cell.B3);
                    break;
                case Cell.C2:
                    cells.Add(Cell.C1);
                    cells.Add(Cell.B2);
                    cells.Add(Cell.C2);
                    cells.Add(Cell.C3);
                    break;
                case Cell.A3:
                    cells.Add(Cell.A2);
                    cells.Add(Cell.A3);
                    cells.Add(Cell.B3);
                    break;
                case Cell.B3:
                    cells.Add(Cell.B2);
                    cells.Add(Cell.A3);
                    cells.Add(Cell.B3);
                    cells.Add(Cell.C3);
                    break;
                case Cell.C3:
                    cells.Add(Cell.C2);
                    cells.Add(Cell.B3);
                    cells.Add(Cell.C3);
                    break;
                default:
                    break;
            }

            Activate(cells.Contains(_cell));
        }

        private void Activate(bool value)
        {
            if(value != _isActivated)
            {
                _isActivated = value;

                if (_isActivated)
                    _onActivate.Invoke();
                else
                    _onDeactivate.Invoke();
            }
        }
    }
}