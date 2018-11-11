using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoLatex.Projects
{
    public class BuildInfo
    {
        private ProjectManager _projectManager;
        public string ProjectId { get; private set; }

        public BuildState State { get; private set; }

        public string LastLog { get; private set; }

        public BuildInfo(ProjectManager projectManager, string projectId)
        {
            _projectManager = projectManager;
            ProjectId = projectId;
        }

        public void PerformBuild()
        {
            LastLog = "Starting";
            State = BuildState.Building;
            _projectManager.OnBuilt(this);
            Task.Run(() =>
            {
                Directory.CreateDirectory(_projectManager.GetBuildDirectory(ProjectId));

                LastLog = "Deleting build files";
                _projectManager.OnBuilt(this);
                Directory.Delete(_projectManager.GetBuildDirectory(ProjectId), true);

                LastLog = "Copying build files";
                _projectManager.OnBuilt(this);
                Copy(_projectManager.GetProjectDirectory(ProjectId), _projectManager.GetBuildDirectory(ProjectId));


                LastLog = "Building";
                _projectManager.OnBuilt(this);

                try
                {
                    string cmd = $"cd {_projectManager.GetBuildDirectory(ProjectId)}; pwd; latexmk -pdf main.tex";
                    var escapedArgs = cmd.Replace("\"", "\\\"");

                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $"-c \"{escapedArgs}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };
                    process.Start();
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    LastLog = result + process.StandardError.ReadToEnd();

                    State = process.ExitCode == 0 ? BuildState.Built : BuildState.BuildFailed;
                }
                catch (Exception e)
                {
                    LastLog = e.ToString();
                    State = BuildState.BuildFailed;
                }

                _projectManager.OnBuilt(this);
            });
        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public enum BuildState
        {
            NotBuilt,
            Building,
            Built,
            BuildFailed
        }
    }
}