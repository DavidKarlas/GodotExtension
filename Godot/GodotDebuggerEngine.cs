using System;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;
using MonoDevelop.Core.Execution;
using MonoDevelop.Debugger;
using System.Net;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace Godot
{
    public class GodotDebuggerEngine : DebuggerEngineBackend
    {
        public GodotDebuggerEngine()
        {
            IdeApp.ProjectOperations.EndBuild += ProjectOperations_EndBuild;
        }

        void ProjectOperations_EndBuild(object sender, MonoDevelop.Projects.BuildEventArgs args)
        {
            foreach (var session in DebuggingService.GetSessions())
            {
                if(session is GodotDebuggerSession godotSession)
                {
                    godotSession.SendReloadScipts();
                }
            }
        }

        public override bool CanDebugCommand(ExecutionCommand cmd)
        {
            return cmd is GodotExecutionCommand;
        }

        public override DebuggerStartInfo CreateDebuggerStartInfo(ExecutionCommand cmd)
        {
            var godotCmd = (GodotExecutionCommand)cmd;
            var godotProjectPath = godotCmd.GodotProjectPath;
            //TODO: Read "mono/debugger_agent/port" under [mono] from project.godot file
            SoftDebuggerRemoteArgs args;
            if (godotCmd.ExecutionType == ExecutionType.Launch)
                args = new SoftDebuggerListenArgs("Godot", IPAddress.Loopback, 0);
            else
                args = new SoftDebuggerConnectArgs("Godot", IPAddress.Loopback, 23685);
            return new GodotDebuggerStartInfo(godotCmd, args)
            {
                WorkingDirectory = godotCmd.WorkingDirectory
            };
        }

        public override DebuggerSession CreateSession()
        {
            return new GodotDebuggerSession();
        }

        public override bool IsDefaultDebugger(ExecutionCommand cmd)
        {
            return true;
        }
    }
}
