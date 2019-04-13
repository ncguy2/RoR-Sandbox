using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Command {
    public class CommandHandler {
        private readonly List<Command> _commands;

        public CommandHandler() {
            _commands = new List<Command>();
            RegisterDefaultCommands();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void RegisterCommand(Command command) {
            _commands.Add(command);
        }

        public void InvokeCommand(string commandLine) {
            Tuple<string, IEnumerable<string>> cmd = SplitCommand(commandLine);

            Command command = GetCommand(cmd.Item1);
            command?.Invoke(cmd.Item2);
        }

        public Command GetCommand(string cmd) {
            try {
                return _commands.First(x => x.Key().Equals(cmd, StringComparison.OrdinalIgnoreCase));
            } catch (Exception) {
                return null;
            }
        }

        public IEnumerable<Command> GetCommands() {
            return _commands;
        }

        private void RegisterDefaultCommands() {
            RegisterCommand(new AmbushCommand());
            RegisterCommand(new GiveCommand());
            RegisterCommand(new HelpCommand());
            RegisterCommand(new RemoveItemCommand());
            RegisterCommand(new SetSharedCommand());
            RegisterCommand(new SpawnEnemyCommand());
            RegisterCommand(new SpawnEquipmentCommand());
            RegisterCommand(new SpawnItemCommand());
        }

        private static Tuple<string, IEnumerable<string>> SplitCommand(string commandLine) {
            string[] strings = commandLine.Split(' ');
            return new Tuple<string, IEnumerable<string>>(strings[0], SeparateArgs(strings, 1));
        }

        private static IEnumerable<T> SeparateArgs<T>(IReadOnlyList<T> input, int startIdx) {
            T[] arr = new T[input.Count - startIdx];

            for (int i = startIdx, j = 0; i < input.Count; i++, j++) {
                arr[j] = input[i];
            }

            return arr;
        }
    }
}