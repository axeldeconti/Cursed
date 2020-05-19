namespace Cursed.Props
{
    public class EnemyCount : Singleton<EnemyCount>
    {
        public enum CountType { All, A1, A2, A3, B1, B2, B3, C1, C2, C3 };
        public static int _numberOfEnemy { get; private set; }
        private int _myNumberOfEnemy;

        public event System.Action enemyCountUpdate;

        protected override void Awake()
        {
            _numberOfEnemy = 0;
            _myNumberOfEnemy = _numberOfEnemy;
        }

        public int GetEnemyCount(CountType type)
        {
            int count = 0;
            switch (type)
            {
                case CountType.All:
                    count = _numberOfEnemy;
                    break;
                case CountType.A1:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 1)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.A2:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 4)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.A3:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 7)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.B1:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 2)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.B2:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 5)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.B3:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 8)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.C1:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 3)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.C2:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 6)
                            count = cell._enemyCount;
                    }
                    break;
                case CountType.C3:
                    foreach (CellInfo cell in FindObjectsOfType<CellInfo>())
                    {
                        if (cell.cellNumberInfo == 9)
                            count = cell._enemyCount;
                    }
                    break;
                default:
                    count = _numberOfEnemy;
                    break;
            }
            return count;
        }

        public void AddEnemy()
        {
            if (_myNumberOfEnemy == _numberOfEnemy)
            {
                _numberOfEnemy++;
                _myNumberOfEnemy++;
            }
            else
            {
                _myNumberOfEnemy = _numberOfEnemy;
            }
            enemyCountUpdate?.Invoke();
        }

        public void OnEnemyDeath()
        {
            if (_myNumberOfEnemy == _numberOfEnemy)
            {
                _numberOfEnemy--;
                _myNumberOfEnemy--;
            }
            else
            {
                _myNumberOfEnemy = _numberOfEnemy;
            }
            enemyCountUpdate?.Invoke();
        }
    }
}