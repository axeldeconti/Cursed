using UnityEngine.SceneManagement;

public class MusicHandler : Singleton<MusicHandler>
{   
    void Start()
    {

    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main")
            AkSoundEngine.PostEvent("Play_Music_MainMenu", gameObject);

        if (SceneManager.GetActiveScene().name == "Scene_Proto_Game")
            AkSoundEngine.PostEvent("Play_Music_InGame", gameObject);
    }
}
