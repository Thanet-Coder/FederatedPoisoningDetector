
using System;
using System.Diagnostics;
using System.IO;

namespace FederatedPoisoningDetector.Integration
{
    public class PythonPredictionRunner
    {
        public bool Run(string inputCsv, string outputCsv)
        {
            string projectRoot = Directory.GetCurrentDirectory();

            string pythonExe = Path.Combine(
                projectRoot,
                ".venv",
                "Scripts",
                "python.exe");

            string predictionScript = Path.Combine(
                projectRoot,
                "python",
                "predict_updates.py");

            if (!File.Exists(pythonExe))
            {
                Console.WriteLine(
                    "Python virtual environment not found.");
                return false;
            }

            if (!File.Exists(predictionScript))
            {
                Console.WriteLine(
                    "Python prediction script not found.");
                return false;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonExe,
                WorkingDirectory = projectRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add(predictionScript);
            startInfo.ArgumentList.Add(inputCsv);
            startInfo.ArgumentList.Add(outputCsv);

            using Process process = new Process
            {
                StartInfo = startInfo
            };

            try
            {
                Console.WriteLine();
                Console.WriteLine("Running Python classifier...");
                Console.WriteLine();

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                Console.WriteLine(output);

                if (process.ExitCode != 0)
                {
                    Console.WriteLine("Python execution failed.");
                    Console.WriteLine(errors);
                    return false;
                }

                return File.Exists(outputCsv);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error launching Python: {ex.Message}");
                return false;
            }
        }
    }
}
