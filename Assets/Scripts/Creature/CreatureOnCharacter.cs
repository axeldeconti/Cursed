using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{

    public class CreatureOnCharacter : MonoBehaviour
    {
        private SpriteMask _mask;
        private Transform _parent;
        private SpriteRenderer _spriteParent, _spriteRenderer;
        private CreatureManager _creatureManager;


        void Awake()
        {
            _parent = transform.parent;

            if( _parent.GetComponent<SpriteMask>() == null) 
                _mask = _parent.gameObject.AddComponent(typeof(SpriteMask)) as SpriteMask;
            else 
                _mask = _parent.GetComponent<SpriteMask>();

            _spriteParent = _parent.GetComponent<SpriteRenderer>();
            _spriteRenderer = transform.GetComponent<SpriteRenderer>();
            _creatureManager = (CreatureManager)FindObjectOfType(typeof(CreatureManager));
        }

        void Update()
        {
            UpdateSpriteMask();
            UpdateFlip();

            if(_creatureManager.CurrentState == CreatureState.Moving || _creatureManager.CurrentState == CreatureState.OnComeBack)
            {
                Destroy(this.gameObject);
            }
        }

        private void UpdateSpriteMask()
        {
            _mask.sprite = _parent.GetComponent<SpriteRenderer>().sprite;
        }

        private void UpdateFlip()
        {
            _spriteRenderer.flipX = _spriteParent.flipX;
        }
    }
}
