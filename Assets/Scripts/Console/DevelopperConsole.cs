using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cursed.Managers;

namespace Cursed.Console
{
    public class DevelopperConsole : Singleton<DevelopperConsole>
    {
        public static Dictionary<string, ConsoleCommand> Commands { get; private set; }

        [SerializeField] private Canvas _consoleCanvas = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private TextMeshProUGUI _consoleText = null;
        [SerializeField] private TextMeshProUGUI _inputText = null;
        [SerializeField] private TMP_InputField _consoleInput = null;

        protected override void Awake()
        {
            base.Awake();

            Commands = new Dictionary<string, ConsoleCommand>();
        }

        private void Start()
        {
            _consoleCanvas.gameObject.SetActive(false);
            CreateCommands();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                _consoleCanvas.gameObject.SetActive(!_consoleCanvas.gameObject.activeSelf);

                if (_consoleCanvas.gameObject.activeSelf)
                {
                    _consoleInput.text = "";
                    _consoleInput.ActivateInputField();
                    GameManager.Instance.State = GameManager.GameState.InDevConsole;
                }
                else
                    GameManager.Instance.State = GameManager.GameState.InGame;
            }

            if (_consoleCanvas.gameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(_inputText.text != "")
                    {
                        //Get rid of the last char
                        char[] c = _inputText.text.ToCharArray();
                        string s = "";
                        for (int i = 0; i < c.Length - 1; i++)
                        {
                            s += c[i];
                        }

                        AddMessageToConsole("- " + s);
                        ParseInput(s);
                    }
                }
            }
        }

        private void CreateCommands()
        {
            //Create all commands here
            Command_Help commandHelp = Command_Help.CreateCommand();
            Command_Test commandTest = Command_Test.CreateCommand();
            Command_Quit commandQuit = Command_Quit.CreateCommand();
            Command_Player commandPlayer = Command_Player.CreateCommand();
            Command_Creature commandCreature = Command_Creature.CreateCommand();
        }

        public static void AddCommandsToConsole(string key, ConsoleCommand command)
        {
            if (!Commands.ContainsKey(key))
            {
                Commands.Add(key, command);
            }
        }

        private void AddMessageToConsole(string msg)
        {
            _consoleText.text += msg + "\n";
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        public static void AddStaticMessageToConsole(string msg)
        {
            DevelopperConsole.Instance._consoleText.text += msg + "\n\n";
            DevelopperConsole.Instance._scrollRect.verticalNormalizedPosition = 0f;
        }

        public void ParseInput(string input)
        {
            string[] tmp = input.Split(null);

            if (tmp.Length == 0 || tmp == null)
            {
                string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                AddMessageToConsole($"<color=#{hexColour}>Command not recognized !</color>");
                return;
            }

            if (!Commands.ContainsKey(tmp[0]))
            {
                string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                AddMessageToConsole($"<color=#{hexColour}>Command not recognized !</color>");
            }
            else
            {
                Commands[tmp[0]].RunCommande(tmp);
            }
        }
    }
}