﻿using System.Collections.Generic;
using System.Linq;

using Castle.Core.Internal;

using DavidFidge.MonoGame.Core.Interfaces.ConsoleCommands;

namespace DavidFidge.MonoGame.Core.ConsoleCommands
{
    public class ConsoleCommandServiceFactory : IConsoleCommandServiceFactory
    {
        private readonly Dictionary<string, IConsoleCommand> _consoleCommands;

        public ConsoleCommandServiceFactory(IConsoleCommand[] consoleCommands)
        {
            _consoleCommands = consoleCommands
                .ToDictionary(c => c.GetType().GetAttributes<ConsoleCommandAttribute>().Single().Name);
        }

        public IConsoleCommand CommandFor(ConsoleCommand command)
        {
            return _consoleCommands[command.Name];
        }
    }
}
