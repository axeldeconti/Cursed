using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.Console
{
    public class Command_Quit : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public override void RunCommande(string[] input)
        {
            Debug.Log("Quit game");
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        public Command_Quit()
        {
            Name = "Quit";
            Command = "quit";
            Description = "Quits the application";
            Help = "Use this command with no arguments to force Unity to quit";

            AddCommandToConsole();
        }

        public static Command_Quit CreateCommand()
        {
            return new Command_Quit();
        }
    }
}