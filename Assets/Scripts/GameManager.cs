using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        Application.targetFrameRate = GameSettings.FRAME_RATE;

        ShowMouseCursor(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ShowMouseCursor(bool visibility)
    {
        Cursor.visible = visibility;
    }
}
