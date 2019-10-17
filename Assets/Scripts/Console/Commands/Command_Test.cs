using UnityEngine;

namespace Cursed.Console
{
    public class Command_Test : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public override void RunCommande(string[] input)
        {
            string str = "";
            for (int i = 0; i < input.Length; i++)
            {
                str += input[i] + " ";
            }

            Debug.Log(str);
        }

        public Command_Test()
        {
            Name = "Test";
            Command = "test";
            Description = "Test the console";
            Help = "Use this command with a string for argument to test the console";

            AddCommandToConsole();
        }

        public static Command_Test CreateCommand()
        {
            return new Command_Test();
        }
    }
}