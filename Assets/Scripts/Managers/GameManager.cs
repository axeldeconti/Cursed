using Cursed.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cursed.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameState _state = GameState.InGame;
        [SerializeField] private bool _showFPS = false;
        public static int FPS = 0;

        [SerializeField] GameObject[] _systemPrefabs = null;
        [SerializeField] private GameObject _loadingScreen = null;
        [SerializeField] private VoidEvent _levelLoaded;
        private List<GameObject> _instancedSystemPrefabs = null;

        private string _currentLevelName = string.Empty;
        private Dictionary<AsyncOperation, UnloadInfo> _loadOperations = null;
        private List<string> _loadedScene = null;

        private bool _pauseGame = false;
        private int _nbOfFramesPassed = 0;
        private float _lastTime = 0;

        [HideInInspector] public bool _mainMenuPassed = false;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            _loadOperations = new Dictionary<AsyncOperation, UnloadInfo>();
            _loadedScene = new List<string>();

            InstatiateSystemPrefabs();

            Application.targetFrameRate = GameSettings.FRAME_RATE;
            _nbOfFramesPassed = 0;
            _lastTime = 0;

            State = GameState.InGame;

            if (_showFPS)
                CursedDebugger.Instance.Add("FPS", () => FPS.ToString());

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Boot"))
                LoadLevel("Main", true);
        }

        private void Update()
        {
            ComputeFPS();

            if (ControlerManager.Instance._ControlerType == ControlerManager.ControlerType.XBOX)
            {
                // PAUSE GAME
                if (Input.GetButtonDown("PauseGame"))
                {
                    _pauseGame = !_pauseGame;

                    if (_pauseGame)
                        _state = GameState.Pause;
                    else
                        _state = GameState.InGame;
                }
            }
        }

        private void ComputeFPS()
        {
            _nbOfFramesPassed++;

            if (Time.realtimeSinceStartup - _lastTime >= 1)
            {
                FPS = _nbOfFramesPassed;

                _nbOfFramesPassed = 0;
                _lastTime = Time.realtimeSinceStartup;
            }
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
            if (_loadingScreen != null) _loadingScreen.SetActive(true);
            AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

            if (ao == null || _loadedScene.Contains(levelName))
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
            if (_loadingScreen != null) _loadingScreen.SetActive(false);
            Debug.Log("[GameManager] Load complete");
            State = GameState.InGame;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentLevelName));
            _levelLoaded.Raise();
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

        public GameState State
        {
            get { return _state; }
            set
            {
                _state = value;

                switch (_state)
                {
                    case GameState.InGame:
                        ShowMouseCursor(false);
                        Cursor.lockState = CursorLockMode.Locked;
                        Time.timeScale = 1f;
                        break;
                    case GameState.Pause:
                        ShowMouseCursor(false);
                        Cursor.lockState = CursorLockMode.Locked;
                        Time.timeScale = 0f;
                        break;
                    case GameState.InDevConsole:
                        ShowMouseCursor(true);
                        Cursor.lockState = CursorLockMode.Locked;
                        break;
                    case GameState.WinLoose:
                        ShowMouseCursor(false);
                        Cursor.lockState = CursorLockMode.Locked;
                        //Time.timeScale = 0f;
                        break;
                    default:
                        break;
                }
            }
        }

        public string CurrentLevelName => _currentLevelName;

        public enum GameState { InGame, Pause, InDevConsole, WinLoose, SceneTransition, Cinematic }


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
}