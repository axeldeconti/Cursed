using Cursed.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTriggerTutoriel : MonoBehaviour
{
    private PlayableDirector _playableDirector;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerInputController>())
        {
            GameManager.Instance.State = GameManager.GameState.Cinematic;
            _playableDirector.Play();
            StartCoroutine(WaitForComebackToGame((float)_playableDirector.duration));
        }
    }

    private IEnumerator WaitForComebackToGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.State = GameManager.GameState.InGame;
        Destroy(gameObject);
    }
}
