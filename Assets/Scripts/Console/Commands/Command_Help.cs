using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cursed.Console
{
    public class Command_Help : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public override void RunCommande(string[] input)
        {
            if(input.Length <= 1)
            {
                string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                DevelopperConsole.AddStaticMessageToConsole($"<color=#{hexColour}>Put something after help : the name of a function or 'all'</color>");
                return;
            }

            StringBuilder builder = new StringBuilder();
            Dictionary<string, ConsoleCommand> commands = DevelopperConsole.Commands;

            if (input[1].Equals("all"))
            {
                for (int i = 0; i < commands.Keys.Count; i++)
                {
                    List<string> s = new List<string>(commands.Keys);
                    ConsoleCommand c = commands[s[i]];
                    builder.Append(c.Name).Append(" : ").Append(c.Description);

                    if (i != commands.Keys.Count - 1)
                        builder.AppendLine();
                }
            }
            else if (commands.ContainsKey(input[1]))
            {
                builder.Append(commands[input[1]].Help);
            }
            else
            {
                string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                string s = $"<color=#{hexColour}>There are no function called : " + input[1] + "</color>";
                builder.Append(s);
            }


            DevelopperConsole.AddStaticMessageToConsole(builder.ToString());
        }

        public Command_Help()
        {
            Name = "Help";
            Command = "help";
            Description = "Print the help sentence for a command";
            Help = "Use this command to print the help sentence of a certain command";

            AddCommandToConsole();
        }

        public static Command_Help CreateCommand()
        {
            return new Command_Help();
        }
    }
}