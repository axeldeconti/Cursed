namespace Cursed.Console
{
    public abstract class ConsoleCommand
    {
        public abstract string Name { get; protected set; }
        public abstract string Command { get; protected set; }
        public abstract string Description { get; protected set; }
        public abstract string Help { get; protected set; }

        public abstract void RunCommande(string[] input);

        public void AddCommandToConsole()
        {
            DevelopperConsole.AddCommandsToConsole(Command, this);
        }
    }
}