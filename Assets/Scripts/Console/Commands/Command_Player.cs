using UnityEngine;

namespace Cursed.Console
{
    public class Command_Player : ConsoleCommand
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
                case "heal":
                    HealCommand(input);
                    break;
                case "dmg":
                    DamageCommand(input);
                    break;
                default:
                    string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                    DevelopperConsole.AddStaticMessageToConsole($"<color=#{hexColour}>Player command not found : {input[1]} </color>");
                    break;
            }
        }

        public Command_Player()
        {
            Name = "Player";
            Command = "player";
            Description = "All commands for the player";
            Help = "Commands : \n - heal : heal the player \n - dmg : damage the player";

            AddCommandToConsole();
        }

        public static Command_Player CreateCommand()
        {
            return new Command_Player();
        }

        private void HealCommand(string[] input)
        {
            Character.HealthManager playerHealthMgr = GameObject.FindGameObjectWithTag("Player").GetComponent<Character.HealthManager>();
            bool canHeal = true;
            int value = 0;

            if (input.Length <= 2)
                value = playerHealthMgr.MaxHealth;
            else
            {
                try
                {
                    value = int.Parse(input[2]);
                }
                catch
                {
                    canHeal = false;
                    string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                    DevelopperConsole.AddStaticMessageToConsole($"<color=#{hexColour}>Must put a number or nothing after heal : {input[2]} </color>");
                }
            }

            if (canHeal)
                playerHealthMgr.AddCurrentHealth(value);
        }

        private void DamageCommand(string[] input)
        {
            Character.HealthManager playerHealthMgr = GameObject.FindGameObjectWithTag("Player").GetComponent<Character.HealthManager>();
            Combat.Attack attack = null;
            bool canAttack = true;

            if (input.Length <= 2)
                attack = new Combat.Attack(1, false, null);
            else
            {
                int value = 0;
                try
                {
                    value = int.Parse(input[2]);
                }
                catch
                {
                    canAttack = false;
                    string hexColour = ColorUtility.ToHtmlStringRGB(Color.red);
                    DevelopperConsole.AddStaticMessageToConsole($"<color=#{hexColour}>Must put a number or nothing after dmg : {input[2]} </color>");
                }

                if (canAttack)
                    attack = new Combat.Attack(value, false, null);
            }

            if(canAttack)
                playerHealthMgr.OnAttack(null, attack);
        }
    }
}