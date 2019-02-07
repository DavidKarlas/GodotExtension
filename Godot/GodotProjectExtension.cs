using System;
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Core.Execution;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using System.Threading.Tasks;

namespace Godot
{
    [ExportProjectModelExtension]
    class GodotProjectExtension : DotNetProjectExtension
    {
        static SolutionItemRunConfiguration[] runConfigurations = new []
        {
            new ProjectRunConfiguration("Launch"),
            new ProjectRunConfiguration("Attach")
        };
        protected override IEnumerable<SolutionItemRunConfiguration> OnGetRunConfigurations(OperationContext ctx)
        {
            if (IsGodotProject())
                return runConfigurations;
            return base.OnGetRunConfigurations(ctx);
        }

        protected override ExecutionCommand OnCreateExecutionCommand(ConfigurationSelector configSel, DotNetProjectConfiguration configuration, ProjectRunConfiguration runConfiguration)
        {
            if (IsGodotProject())
            {
                var runConfigurationIndex = runConfigurations.IndexOf(runConfiguration);
                if (runConfigurationIndex == -1)
                    LoggingService.LogError($"Unexpected RunConfiguration {runConfiguration.Id} {runConfiguration.GetType().FullName}");
                var executionType = runConfigurations[0] == runConfiguration ? ExecutionType.Launch : ExecutionType.Attach;
                return new GodotExecutionCommand(
                    GetGodotProjectPath(),
                    executionType,
                    Path.GetDirectoryName(GetGodotProjectPath()));
            }
            return base.OnCreateExecutionCommand(configSel, configuration, runConfiguration);
        }

        private string GetGodotProjectPath()
        {
            return Path.Combine(Path.GetDirectoryName(Project.FileName), "project.godot");
        }

        bool? cachedIsGodotProject;
        private bool IsGodotProject()
        {
            if (!cachedIsGodotProject.HasValue)
                cachedIsGodotProject = File.Exists(GetGodotProjectPath());
            return cachedIsGodotProject.Value;
        }

        protected override ProjectFeatures OnGetSupportedFeatures()
        {
            var features = base.OnGetSupportedFeatures();
            if (IsGodotProject())
                features |= ProjectFeatures.Execute;
            return features;
        }

        protected override bool OnGetCanExecute(ExecutionContext context, ConfigurationSelector configuration, SolutionItemRunConfiguration runConfiguration)
        {
            if (IsGodotProject())
                return true;
            return base.OnGetCanExecute(context, configuration, runConfiguration);
        }

        protected override async Task OnExecuteCommand(ProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configuration, ExecutionCommand executionCommand)
        {
            if (executionCommand is GodotExecutionCommand godotCmd)
            {
                if (godotCmd.ExecutionType == ExecutionType.Launch)
                {
                    if (!File.Exists(Options.GodotExecutable))
                    { 
                        // Delay for 1 sec so it's not overriden by build message
                        await Task.Delay(1000);
                        monitor.ReportError(GettextCatalog.GetString($"Godot executable \"{Options.GodotExecutable.Value}\" not found. Update Godot executable setting in perferences."));
                        return;
                    }
                }
            }
            await base.OnExecuteCommand(monitor, context, configuration, executionCommand);
        }
    }
}
