using System;

namespace Sandbox.Command.Attribute {
    [AttributeUsage(AttributeTargets.Class)]
    public class SandboxCommandAttribute : System.Attribute {
        private readonly bool _wip;

        public SandboxCommandAttribute(bool wip = false) {
            _wip = wip;
        }

        public bool WorkInProgress => _wip;
    }
}