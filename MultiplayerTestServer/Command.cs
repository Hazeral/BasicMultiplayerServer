using System;
using System.Linq;

namespace MultiplayerTestServer
{
    enum CommandArgumentType
    {
        PlayerID,
        Float,
        String
    }

    class CommandArgument
    {
        public readonly string Name;
        public readonly bool Optional;
        public readonly CommandArgumentType Type;

        public CommandArgument(string name, bool optional, CommandArgumentType type)
        {
            Name = name;
            Optional = optional;
            Type = type;
        }
    }

    class Command
    {
        public readonly string Name;
        public readonly string[] Aliases;
        public readonly string Description;
        public readonly bool ServerOnly;
        public CommandArgument[] Arguments;
        private Action<string[], Player, Player> Execute;
        private int requiredArguments = 0;
        public readonly string UsageText = "";

        public Command(string name, string description, string[] aliases, CommandArgument[] arguments, Action<string[], Player, Player> execute, bool serverOnly = false)
        {
            Name = name;
            Aliases = aliases ?? (new string[] { });
            Description = description;
            ServerOnly = serverOnly;
            Arguments = arguments ?? (new CommandArgument[] { });
            Execute = execute;
            
            foreach(CommandArgument arg in Arguments)
            {
                UsageText += $" {arg.Name}";
                if (arg.Optional) UsageText += "(optional)";
                else requiredArguments++;
            }
        }

        public void Run(string[] args, Player author = null)
        {
            Commands.Log(author, $"Running command [{Name}]", false);

            if (author != null && ServerOnly)
            {
                Commands.Log(author, $"This command can only be used by the server");
                return;
            }

            if (args.Length < requiredArguments)
            {
                Commands.Log(author, $"Not enough arguments, usage:{UsageText}");
                return;
            }

            Player target = null;

            int argIndex = 0;
            foreach(CommandArgument arg in Arguments)
            {
                if (arg.Optional && (args.Length < (argIndex + 1))) break;

                switch(arg.Type)
                {
                    case CommandArgumentType.PlayerID:
                        if (!Server.players.Keys.Contains(args[argIndex]))
                        {
                            Commands.Log(author, $"Player [{args[argIndex]}] not found");
                            return;
                        }
                        target = Server.players.Where(p => p.Key == args[argIndex]).First().Value;
                        break;
                    case CommandArgumentType.Float:
                        if (!float.TryParse(args[argIndex], out _))
                        {
                            Commands.Log(author, $"[{args[argIndex]}] is invalid, usage:{UsageText}");
                            return;
                        }
                        break;
                }
                argIndex++;
            }
            
            Execute(args, author, target);
        }
    }
}
