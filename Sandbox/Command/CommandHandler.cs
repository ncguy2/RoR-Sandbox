using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Command {
    public class CommandHandler {
        private readonly List<ICommand> commands;

        public CommandHandler() {
            commands = new List<ICommand>();
            registerDefaultCommands();
        }

        public void registerCommand(ICommand command) {
            commands.Add(command);
        }

        public void invokeCommand(string commandLine) {
            Tuple<string, IEnumerable<string>> cmd = splitCommand(commandLine);

            ICommand command = getCommand(cmd.Item1);
            command?.invoke(cmd.Item2);
        }

        public ICommand getCommand(string cmd) {
            return commands.First(x => x.key().Equals(cmd, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<ICommand> getCommands() {
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