using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("Play_Music", gameObject);
    }
}
