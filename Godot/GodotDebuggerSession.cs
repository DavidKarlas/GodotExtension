using Mono.Debugging.Client;
using Mono.Debugging.Soft;
using System.Diagnostics;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using System.Threading;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Godot
{
    class GodotDebuggerSession : SoftDebuggerSession
    {
        bool attached;
        public void SendReloadScipts()
        {
            //32 is length, rest after [3] is GD.Var2Bytes(new Array().Add("reload_scripts"))
            var arr = new byte[] { 32, 0, 0, 0, 19, 0, 0, 0, 1, 0, 0, 0, 4, 0, 0, 0, 14, 0, 0, 0, 114, 101, 108, 111, 97, 100, 95, 115, 99, 114, 105, 112, 116, 115, 0, 0 };
            stream.Write(arr, 0, arr.Length);
            stream.Flush();
        }

        protected override void OnRun(DebuggerStartInfo startInfo)
        {
            var godotStartInfo = (GodotDebuggerStartInfo)startInfo;
            if (godotStartInfo.GodotCmd.ExecutionType == ExecutionType.Launch)
            {
                attached = false;
                StartListening(godotStartInfo, out var assignedDebugPort);
                var tcpListener = new TcpListener(IPAddress.Any, 0);
                tcpListener.Start();
                tcpListener.AcceptTcpClientAsync().ContinueWith(ProcessNewTcpClient);
                var psi = new ProcessStartInfo(Godot.Options.GodotExecutable)
                {
                    Arguments = $"--path {startInfo.WorkingDirectory} --remote-debug 127.0.0.1:{((IPEndPoint)tcpListener.LocalEndpoint).Port}",
                    WorkingDirectory = startInfo.WorkingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                psi.EnvironmentVariables["GODOT_MONO_DEBUGGER_AGENT"] = $"--debugger-agent=transport=dt_socket,address=127.0.0.1:{assignedDebugPort},server=n";
                var p = Process.Start(psi);
                var thread = new Thread(OutputReader);
                thread.Name = "Godot output reader";
                thread.IsBackground = true;
                thread.Start((false, p.StandardOutput));
                thread = new Thread(OutputReader);
                thread.Name = "Godot error reader";
                thread.IsBackground = true;
                thread.Start((true, p.StandardError));
                OnDebuggerOutput(false, $"Godot PID:{p.Id}{Environment.NewLine}");
            }
            else if (godotStartInfo.GodotCmd.ExecutionType == ExecutionType.Attach)
            {
                attached = true;
                StartConnecting(godotStartInfo);
            }
            else
            {
                throw new NotImplementedException(godotStartInfo.GodotCmd.ExecutionType.ToString());
            }
        }

        NetworkStream stream;
        private async Task ProcessNewTcpClient(Task<TcpClient> task)
        {
            var tcp = task.Result;
            stream = tcp.GetStream();
            byte[] buffer = new byte[1000];
            while (tcp.Connected)
            {
                // There is no library to decode this messages, so
                // we just pump buffer so it doesn't go out of memory
                var readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            }
        }

        protected override bool HandleException(Exception ex)
        {
            // When we attach to running Mono process it sends us AssemblyLoad, ThreadStart and other
            // delayed events, problem is, that we send VM_START command back since we have to do that on
            // every event, but in this case when Mono is sending delayed events when we attach
            // runtime is not really suspended, hence it's throwing this exceptions, just ignore...
            if (attached && ex is Mono.Debugger.Soft.VMNotSuspendedException)
                return true;
            return base.HandleException(ex);
        }

        protected override void OnExit()
        {
            if (attached)
                base.OnDetach();
            else
                base.OnExit();
        }

        void OutputReader(object args)
        {
            var (isErr, stream) = ((bool, StreamReader))args;
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                try
                {
                    OnTargetOutput(false, line + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}