using System.Collections;
using UnityEngine;
using Cursed.Character;

namespace Cursed.VisualEffect
{
    public class InvincibilityAnimation : MonoBehaviour
    {
        private HealthManager _healthManager;
        private SpriteRenderer _spriteRenderer;
        private float _spriteAlpha;
        [SerializeField] private float _fadeSpeed = .5f;

        private void Awake()
        {
            _healthManager = GetComponent<HealthManager>();
            _spriteAlpha = GetComponent<SpriteRenderer>().color.a;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (_healthManager.IsInvincible)
            {
                float _alpha = Mathf.PingPong(_fadeSpeed * Time.time, _spriteAlpha);
                _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _alpha);
            }
            else
                _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _spriteAlpha);
        }
    }
}
