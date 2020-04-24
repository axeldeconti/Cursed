using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class MapCellUI : MonoBehaviour
    {
        [Header("Cell Info")]
        [SerializeField] private int _cellInfo;
        public CellInfo _myCell { get; private set; }

        [Header("Sprites")]
        [SerializeField] private Sprite _emptyCell;
        [SerializeField] private Sprite _filledCell;
        private Image _spriteImage;

        [Header("Referencies")]
        [SerializeField] private RectTransform _avatarPositionUI;

        private void Awake()
        {
            _spriteImage = GetComponent<Image>();
        }

        private void Start()
        {
            CellInfo[] _cells = GameObject.FindObjectsOfType<CellInfo>();
            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i].cellNumberInfo == _cellInfo)
                    _myCell = _cells[i];
            }

            _myCell._onEnemyCellCountUpdate += () => UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (_myCell._emptyCell)
                _spriteImage.sprite = _emptyCell;
            else
                _spriteImage.sprite = _filledCell;
        }

        public void PlayerOnMyCell()
        {
            _avatarPositionUI.transform.parent = this.transform;
            _avatarPositionUI.localPosition = new Vector3(0f, 0f, 0f);
            _avatarPositionUI.localScale = new Vector3(1f, 1f, 1f);

            GetComponentInParent<MapUIManager>().DeactiveMap();
        }
    }
}
