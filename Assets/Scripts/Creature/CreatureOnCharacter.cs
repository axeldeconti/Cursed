using UnityEngine;
using Cursed.Character;

namespace Cursed.Creature
{

    public class CreatureOnCharacter : MonoBehaviour
    {
        public static CreatureOnCharacter Instance;

        private SpriteMask _mask;
        private Transform _parent;
        private SpriteRenderer _spriteParent, _spriteRenderer;
        private CreatureManager _creatureManager;
        private CharacterMovement _characterMovemenent;
        private int _side;


        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
            
            _parent = transform.parent;

            if( _parent.GetComponent<SpriteMask>() == null) 
                _mask = gameObject.AddComponent(typeof(SpriteMask)) as SpriteMask;
            else 
                _mask = gameObject.GetComponent<SpriteMask>();

            _spriteParent = _parent.GetComponent<SpriteRenderer>();
            _spriteRenderer = transform.GetComponent<SpriteRenderer>();
            _creatureManager = (CreatureManager)FindObjectOfType(typeof(CreatureManager));
            _characterMovemenent = _parent.GetComponent<CharacterMovement>();
            _side = 1;
        }

        void Update()
        {
            UpdateSpriteMask();
            //UpdateFlip();

            if(_creatureManager.CurrentState == CreatureState.Moving || _creatureManager.CurrentState == CreatureState.OnComeBack || _creatureManager.CurrentState == CreatureState.Chasing)
            {
                Destroy(this.gameObject);
            }
        }

        private void UpdateSpriteMask()
        {
            _mask.sprite = _spriteParent.sprite;
        }

        private void UpdateFlip()
        {
            if(_side == 1 && _characterMovemenent.Side == -1)
            {
                _side = -1;
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
            if (_side == -1 && _characterMovemenent.Side == 1)
            {
                _side = 1;
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            }
        }
    }
}
