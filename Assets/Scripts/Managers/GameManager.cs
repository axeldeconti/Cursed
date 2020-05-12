using Cursed.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameState _state = GameState.InGame;
    [SerializeField] private bool _showFPS = false;
    public static int FPS = 0;

    [SerializeField] GameObject[] _systemPrefabs = null;
    [SerializeField] private VoidEvent _loadingLevel;
    private List<GameObject> _instancedSystemPrefabs = null;

    private string _currentLevelName = string.Empty;
    private Dictionary<AsyncOperation, UnloadInfo> _loadOperations = null;
    private List<string> _loadedScene = null;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _loadOperations = new Dictionary<AsyncOperation, UnloadInfo>();
        _loadedScene = new List<string>();

        InstatiateSystemPrefabs();

        Application.targetFrameRate = GameSettings.FRAME_RATE;

        State = GameState.InGame;

        if (_showFPS)
            CursedDebugger.Instance.Add("FPS", () => FPS.ToString());

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Boot"))
            LoadLevel("Main", true);
    }

    private void Update()
    {
        FPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
    }

    public string GetGameVersion()
    {
        string version = Application.version;
        return version;
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

    public void LoadLevel(string levelName, bool unloadAll)
    {
        _loadingLevel.Raise();

        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        if(ao == null || _loadedScene.Contains(levelName))
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }

        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao, new UnloadInfo(levelName, unloadAll));
        _loadedScene.Add(levelName);
        _currentLevelName = levelName;

        //Handle the button sound issue
        ButtonHandler.isFirstSelected = true;
    }

    private void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.ContainsKey(ao))
        {
            UnloadAll(_loadOperations[ao]);
            _loadOperations.Remove(ao);
        }

        Debug.Log("[GameManager] Load complete");
        State = GameState.InGame;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentLevelName));
    }

    private void UnloadAll(UnloadInfo info)
    {
        if (!info.UnloadAll)
            return;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (!SceneManager.GetSceneAt(i).name.Equals("Boot") && !SceneManager.GetSceneAt(i).name.Equals(info.Name))
            {
                UnloadLevel(SceneManager.GetSceneAt(i).name);
            }
        }
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);

        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return;
        }

        if (_loadedScene.Contains(levelName))
            _loadedScene.Remove(levelName);
        ao.completed += OnUnloadOperationComplete;
    }

    private void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("[GameManager] Unload complete");
    }

    private bool HasScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals(sceneName))
                return true;
        }

        return false;
    }

    public void ShowMouseCursor(bool visibility)
    {
        Cursor.visible = visibility;
    }

    public void QuitGame()
    {
        Debug.Log("[GameManager] !! Quit Game !!");
        Application.Quit();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_instancedSystemPrefabs == null)
            return;

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
                case GameState.WinLoose:
                    ShowMouseCursor(false);
                    Cursor.lockState = CursorLockMode.Confined;
                    Time.timeScale = 0f;
                    break;
                default:
                    break;
            }
        } 
    }

    public string CurrentLevelName => _currentLevelName;

    public enum GameState { InGame, Pause, InDevConsole, WinLoose, SceneTransition }

    private struct UnloadInfo
    {
        public string Name;
        public bool UnloadAll;

        public UnloadInfo(string name, bool unloadAll)
        {
            Name = name;
            UnloadAll = unloadAll;
        }
    }
}