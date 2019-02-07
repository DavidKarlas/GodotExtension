using Mono.Debugging.Client;
using Mono.Debugging.Soft;
using System.Net;

namespace Godot
{
    class GodotDebuggerStartInfo : SoftDebuggerStartInfo
    {
        public GodotExecutionCommand GodotCmd { get; }

        public GodotDebuggerStartInfo(GodotExecutionCommand godotCmd, SoftDebuggerRemoteArgs softDebuggerConnectArgs) :
            base(softDebuggerConnectArgs)
        {
            GodotCmd = godotCmd;
        }
    }
}