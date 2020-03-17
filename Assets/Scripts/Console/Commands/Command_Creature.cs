using Cursed.Creature;
using UnityEngine;

namespace Cursed.Console
{
    public class Command_Creature : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public override void RunCommande(string[] input)
        {
            if (input.Length <= 1)
            {
                string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                DevelopperConsole.AddStaticMessageToConsole($"<color=#{hexColour}>Put something after player : the name of a function </color>");
                return;
            }

            switch (input[1])
            {
                case "attackPlayer":
                    ToogleCanAttackPlayer();
                    break;
                default:
                    string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                    DevelopperConsole.AddStaticMessageToConsole($"<color=#{hexColour}>Player command not found : {input[1]} </color>");
                    break;
            }
        }

        public Command_Creature()
        {
            Name = "Creature";
            Command = "creature";
            Description = "All commands for the creature";
            Help = "Commands : \n - attackPlayer : Toggle if the creature can attack the player";

            AddCommandToConsole();
        }

        public static Command_Creature CreateCommand()
        {
            return new Command_Creature();
        }

        private void ToogleCanAttackPlayer()
        {
            CreatureHealthManager creaHealthMgr = GameObject.FindGameObjectWithTag("Creature").GetComponent<CreatureHealthManager>();
            creaHealthMgr.CanAttackPlayer = !creaHealthMgr.CanAttackPlayer;
        }
    }
}