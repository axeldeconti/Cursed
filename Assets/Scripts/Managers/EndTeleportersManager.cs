using UnityEngine;
using Cursed.Props;

namespace Cursed.Managers
{
    public class EndTeleportersManager : Singleton<EndTeleportersManager>
    {
        public event System.Action<Transform> _onTeleportSpawn;

        private TeleporterEndMap[] _teleporters;

        protected override void Awake()
        {
            base.Awake();

            _teleporters = new TeleporterEndMap[transform.childCount];

            for(int i = 0; i < _teleporters.Length; i++)
            {
                _teleporters[i] = transform.GetChild(i).GetComponent<TeleporterEndMap>();
                _teleporters[i].gameObject.SetActive(false);
            }
        }

        public void ActiveTeleporter()
        {
            foreach(CellInfo cell in FindObjectsOfType<CellInfo>())
            {
                if(cell._playerOnThisCell)
                {
                    for(int i = 0; i < _teleporters.Length; i++)
                    {
                        if (_teleporters[i]._teleporterNumberCell == cell.cellNumberInfo)
                        {
                            _teleporters[i].gameObject.SetActive(true);
                            _onTeleportSpawn?.Invoke(_teleporters[i].transform);
                        }
                    }
                }
            }
        }
    }
}
