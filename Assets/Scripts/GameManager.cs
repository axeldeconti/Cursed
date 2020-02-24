using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool _showFPS = false;
    public static int FPS = 0;

    private void Start()
    {
        Application.targetFrameRate = GameSettings.FRAME_RATE;

        ShowMouseCursor(false);
        Cursor.lockState = CursorLockMode.Confined;

        if (_showFPS)
            CursedDebugger.Instance.Add("FPS", () => FPS.ToString());
    }

    private void Update()
    {
        FPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
    }

    public void ShowMouseCursor(bool visibility)
    {
        Cursor.visible = visibility;
    }
}
