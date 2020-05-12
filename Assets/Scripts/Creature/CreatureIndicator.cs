using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using CodeMonkey.Utils;

namespace Cursed.Creature
{

    public class CreatureIndicator : MonoBehaviour
    {
        [SerializeField] private Sprite _indicator;
        private Transform targetPosition;
        [SerializeField] private RectTransform pointerRectTransform;
        [SerializeField] private float _pointerOffset = 75f;
        private Image pointerImage;

        private void Awake()
        {
            //pointerRectTransform = GetComponentInChildren<RectTransform>();
            pointerImage = GetComponentInChildren<Image>();
        }

        private void Start()
        {
            if(GameObject.FindGameObjectWithTag("Creature").GetComponentInChildren<Collider2D>() != null)
                targetPosition = GameObject.FindGameObjectWithTag("Creature").GetComponentInChildren<Collider2D>().transform;

            if (targetPosition == null)
                this.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (targetPosition == null)
            {
                if (GameObject.FindGameObjectWithTag("Creature").GetComponentInChildren<Collider2D>() != null)
                    targetPosition = GameObject.FindGameObjectWithTag("Creature").GetComponentInChildren<Collider2D>().transform;
            }

            float borderSize = 0f;
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition.position);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            if (isOffScreen)
            {
                RotatePointerTowardsTargetPosition();

                pointerImage.color = new Color(pointerImage.color.r, pointerImage.color.g, pointerImage.color.b, 1f);
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if (cappedTargetScreenPosition.x <= _pointerOffset) cappedTargetScreenPosition.x = _pointerOffset;
                if (cappedTargetScreenPosition.x >= Screen.width - _pointerOffset) cappedTargetScreenPosition.x = Screen.width - _pointerOffset;
                if (cappedTargetScreenPosition.y <= _pointerOffset) cappedTargetScreenPosition.y = _pointerOffset;
                if (cappedTargetScreenPosition.y >= Screen.height - _pointerOffset) cappedTargetScreenPosition.y = Screen.height - _pointerOffset;

                Vector3 pointerWorldPosition = cappedTargetScreenPosition;
                //Vector3 pointerWorldPosition = _uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);

                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

            }
            else
            {
                pointerImage.color = new Color(pointerImage.color.r, pointerImage.color.g, pointerImage.color.b, 0f);
                //Vector3 pointerWorldPosition = _uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
                Vector3 pointerWorldPosition = targetPositionScreenPoint;
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

                pointerRectTransform.localEulerAngles = Vector3.zero;
                //Hide();
            }
        }

        private void RotatePointerTowardsTargetPosition()
        {
            Vector3 toPosition = targetPosition.position;
            Vector3 fromPosition = Camera.main.transform.position;
            fromPosition.z = 0f;
            Vector3 dir = (toPosition - fromPosition).normalized;
            float angle = UtilsClass.GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(Vector3 targetPosition)
        {
            gameObject.SetActive(true);
            this.targetPosition.position = targetPosition;
        }
    }
}
