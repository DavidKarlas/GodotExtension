using System;
using MonoDevelop.Core.Execution;

namespace Godot
{
    class GodotExecutionCommand : ExecutionCommand
    {
        public GodotExecutionCommand(string godotProjectPath, ExecutionType executionType, string workingDirectory)
        {
            GodotProjectPath = godotProjectPath;
            ExecutionType = executionType;
            WorkingDirectory = workingDirectory;
        }

        public string GodotProjectPath { get; }
        public ExecutionType ExecutionType { get; }
        public string WorkingDirectory { get; }
    }

    public enum ExecutionType
    {
        Launch,
        Attach
    }
}
