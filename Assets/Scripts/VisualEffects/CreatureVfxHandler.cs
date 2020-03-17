using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureVfxHandler : MonoBehaviour
    {
        [Header("VFX Movement")]
        [SerializeField] private GameObject _vfxMoveParticle;
        [SerializeField] private GameObject _vfxTrailParticle;
        [SerializeField] private GameObject _vfxLauchParticle;
        [SerializeField] private GameObject _vfxTouchImpactParticle;

        [Header("Referencies")]
        [SerializeField] private Transform _creatureBack;


        public void CreatureMoveParticle()
        {
            Instantiate(_vfxMoveParticle, _creatureBack.position, Quaternion.identity, transform);
        }
        public GameObject CreatureTrailParticle()
        {
            GameObject particle = Instantiate(_vfxTrailParticle, _creatureBack.position, Quaternion.identity, transform);
            return particle;
        }
        public void CreatureLauchParticle(Vector2 direction)
        {
            GameObject go = Instantiate(_vfxLauchParticle, transform.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            go.transform.rotation = Quaternion.Euler(rotation.eulerAngles);
        }
        public void CreatureTouchImpactParticle(Transform hit)
        {
            Instantiate(_vfxTouchImpactParticle, hit.position, Quaternion.identity);
        }
    }
}
