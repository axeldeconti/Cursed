using System;
using Cursed.Props;
using UnityEngine;

namespace Cursed.Managers
{
    public class EndTeleportersManager : Singleton<EndTeleportersManager>
    {
        public event Action<Transform> _onTeleportSpawn;

        private TeleporterEndMap[] _teleporters;

        protected override void Awake()
        {
            base.Awake();

            _teleporters = new TeleporterEndMap[transform.childCount];

            for (int i = 0; i < _teleporters.Length; i++)
            {
                _teleporters[i] = transform.GetChild(i).GetComponent<TeleporterEndMap>();
                _teleporters[i].gameObject.SetActive(false);
            }
        }

        public void ActiveTeleporter()
        {
            for (int i = 0; i < _teleporters.Length; i++)
            {
                if (_teleporters[i]._cell == CellManager.Instance.currentCell)
                {
                    _teleporters[i].gameObject.SetActive(true);
                    _onTeleportSpawn?.Invoke(_teleporters[i].transform);
                }
            }
        }
    }
}
