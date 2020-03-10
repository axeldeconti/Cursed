using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameState _state = GameState.InGame;
    [SerializeField] private bool _showFPS = false;
    public static int FPS = 0;

    [SerializeField] GameObject[] _systemPrefabs = null;
    private List<GameObject> _instancedSystemPrefabs = null;

    private string _currentLevelName = string.Empty;
    private List<AsyncOperation> _loadOperations = null;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _loadOperations = new List<AsyncOperation>();

        InstatiateSystemPrefabs();

        Application.targetFrameRate = GameSettings.FRAME_RATE;

        State = GameState.InGame;

        if (_showFPS)
            CursedDebugger.Instance.Add("FPS", () => FPS.ToString());

        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Boot"))
            LoadLevel("Main");
    }

    private void Update()
    {
        FPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
    }

    private void InstatiateSystemPrefabs()
    {
        _instancedSystemPrefabs = new List<GameObject>();
        
        if (_systemPrefabs.Length == 0)
            return;

        GameObject instancePrefab = null;

        foreach (GameObject prefab in _systemPrefabs)
        {
            instancePrefab = Instantiate(prefab);
            _instancedSystemPrefabs.Add(instancePrefab);
        }
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        if(ao == null)
        {
            Debug.LogError("[GameManager] Unable to load leve " + levelName);
            return;
        }

        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _currentLevelName = levelName;
    }

    private void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            //Transition between level
            //Change all this
            if (_currentLevelName == "Scene_Proto_Game")
                UnloadLevel("Main");

            if (_currentLevelName == "Main" && SceneManager.sceneCount >= 4)
                UnloadLevel("Scene_Proto_Game");
        }

        Debug.Log("Load complete");
        State = GameState.InGame;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentLevelName));
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);

        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return;
        }

        ao.completed += OnUnloadOperationComplete;
    }

    private void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload complete");
    }

    public void ShowMouseCursor(bool visibility)
    {
        Cursor.visible = visibility;
    }

    public void QuitGame()
    {
        Debug.Log("!! Quit Game !!");
        Application.Quit();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_instancedSystemPrefabs.Count == 0)
            return;

        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);
        }

        _instancedSystemPrefabs.Clear();
    }

    public GameState State { 
        get { return _state; } 
        set 
        { 
            _state = value;

            switch (_state)
            {
                case GameState.InGame:
                    ShowMouseCursor(false);
                    Cursor.lockState = CursorLockMode.Confined;
                    Time.timeScale = 1f;
                    break;
                case GameState.Pause:
                    ShowMouseCursor(false);
                    Cursor.lockState = CursorLockMode.Confined;
                    Time.timeScale = 0f;
                    break;
                case GameState.InDevConsole:
                    ShowMouseCursor(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
                default:
                    break;
            }
        } 
    }

    public enum GameState { InGame, Pause, InDevConsole }
}