using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Command {
    public class CommandHandler {
        private readonly List<Command> commands;

        public CommandHandler() {
            commands = new List<Command>();
            registerDefaultCommands();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void registerCommand(Command command) {
            commands.Add(command);
        }

        public void invokeCommand(string commandLine) {
            Tuple<string, IEnumerable<string>> cmd = splitCommand(commandLine);

            Command command = getCommand(cmd.Item1);
            command?.invoke(cmd.Item2);
        }

        public Command getCommand(string cmd) {
            try {
                return commands.First(x => x.Key.Equals(cmd, StringComparison.OrdinalIgnoreCase));
            } catch (Exception e) {
                return null;
            }
        }

        public IEnumerable<Command> getCommands() {
            return commands;
        }

        private void registerDefaultCommands() {
            registerCommand(new AmbushCommand());
            registerCommand(new HelpCommand());
            registerCommand(new SetShared());
            registerCommand(new SpawnEnemy());
            registerCommand(new SpawnEquipment());
            registerCommand(new SpawnItem());
        }

        private Tuple<string, IEnumerable<string>> splitCommand(string commandLine) {
            string[] strings = commandLine.Split(' ');
            return new Tuple<string, IEnumerable<string>>(strings[0], separateArgs(strings, 1));
        }

        private IEnumerable<T> separateArgs<T>(T[] input, int startIdx) {
            T[] arr = new T[input.Length - startIdx];

            for (int i = startIdx, j = 0; i < input.Length; i++, j++) arr[j] = input[i];

            return arr;
        }
    }
}