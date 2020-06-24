using System;
using Cursed.Props;

namespace Cursed.Managers
{
    public class CellManager : Singleton<CellManager>
    {
        public Action<Cell> onPlayerEnterCell = null;

        public Cell currentCell { get; private set; } = Cell.A1;

        private void Start()
        {
            foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
            {
                cell.onPlayerEnterCell += OnPlayerEnterCellCallback;
            }
        }

        private void OnPlayerEnterCellCallback(Cell cell)
        {
            currentCell = cell;
            onPlayerEnterCell.Invoke(cell);
        }
    }

    public enum Cell { A1, B1, C1, A2, B2, C2, A3, B3, C3 }
}