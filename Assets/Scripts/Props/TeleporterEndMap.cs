using UnityEngine;
using System.Collections;
using Cursed.Managers;

namespace Cursed.Props
{
    public class TeleporterEndMap : MonoBehaviour
    {
        [SerializeField] private Transform _targetLaunch;
        [SerializeField] private float _launchSpeed;
        [SerializeField] private string _sceneToLaunch;

        private Animator _animator;
        private bool _launchTeleporter;
        private Vector2 _targetVector;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _targetVector = _targetLaunch.position;
        }

        private void Update()
        {
            if (_launchTeleporter)
                Launch();
        }

        private void Launch()
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetVector, _launchSpeed * Time.deltaTime);

            CheckDistanceFromTarget();
        }

        private void CheckDistanceFromTarget()
        {
            if (transform.position.y == _targetLaunch.position.y)
                GameManager.Instance.LoadLevel(_sceneToLaunch, true);
        }

        private void CloseWindows()
        {
            _animator.SetTrigger("Close");
            AkSoundEngine.PostEvent("Play_EndLevelDoor", gameObject);
            StartCoroutine(WaitForLaunchTeleporter(_animator.GetCurrentAnimatorClipInfo(0).Length));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                CloseWindows();
                collision.transform.parent = this.transform;
                collision.transform.localPosition = new Vector3(0f, -2.75f, 0f);
                GameManager.Instance.State = GameManager.GameState.SceneTransition;
            }
        }

        IEnumerator WaitForLaunchTeleporter(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Load map : " + _sceneToLaunch);
            GameManager.Instance.LoadLevel(_sceneToLaunch, true);
            //_launchTeleporter = true;
        }
    }
}
