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

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            AkSoundEngine.PostEvent("Play_Music_InGame", gameObject);
            AkSoundEngine.PostEvent("Play_Amb_Atmosphere", gameObject);
        }
    }
            
}
