using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Cursed.UI
{
    public class ControlButtonHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public static bool isFirstSelected = true;

        [SerializeField] private ControlsBehaviour _controlsBehaviour;
        [SerializeField] private Image[] _movementImg;
        [SerializeField] private Image[] _creatureImg;
        [SerializeField] private Image[] _combatImg;
        private Button _button = null;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void SetImageColor(Color color)
        {
            switch(_controlsBehaviour._currentIndex)
            {
                case 0:
                    for(int i = 0; i < _movementImg.Length; i++)
                    {
                        _movementImg[i].color = color;
                    }
                    break;

                case 1:
                    for(int i = 0; i < _creatureImg.Length; i++)
                    {
                        _creatureImg[i].color = color;
                    }
                    break;

                case 2:
                    for(int i = 0 ; i < _combatImg.Length; i++)
                    {
                        _combatImg[i].color = color;
                    }
                    break;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetImageColor(_button.colors.selectedColor);

            if (isFirstSelected)
                isFirstSelected = false;
            else
                AkSoundEngine.PostEvent("Play_Button_Selected", gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetImageColor(_button.colors.normalColor);
        }
    }
}
