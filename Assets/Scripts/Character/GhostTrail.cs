using UnityEngine;
using DG.Tweening;

namespace Cursed.Character
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class GhostTrail : MonoBehaviour
    {
        private CharacterMovement _move;
        private AnimationHandler _anim;
        private SpriteRenderer _renderer;

        public Transform ghostsParent;
        public Color trailColor;
        public Color fadeColor;
        public FloatReference ghostInterval;
        public FloatReference fadeTime;

        private void Start()
        {
            _anim = FindObjectOfType<AnimationHandler>();
            _move = FindObjectOfType<CharacterMovement>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Show the ghosts and place them at the right position
        /// </summary>
        public void ShowGhosts()
        {
            Sequence s = DOTween.Sequence();

            for (int i = 0; i < ghostsParent.childCount; i++)
            {
                Transform currentGhost = ghostsParent.GetChild(i);
                s.AppendCallback(() => currentGhost.position = _move.transform.position);
                s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = _anim.Renderer.flipX);
                s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = _anim.Renderer.sprite);
                s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
                s.AppendCallback(() => FadeSprite(currentGhost));
                s.AppendInterval(ghostInterval);
            }
        }

        /// <summary>
        /// Fade a ghost back to invisible
        /// </summary>
        /// <param name="current"></param>
        public void FadeSprite(Transform current)
        {
            SpriteRenderer sr = current.GetComponent<SpriteRenderer>();

            if (sr == null)
                return;

            sr.material.DOKill();
            sr.material.DOColor(fadeColor, fadeTime);
        }
    }
}