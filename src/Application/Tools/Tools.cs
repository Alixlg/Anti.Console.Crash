using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace Application.Tools;

public class Tools
{
    public static bool IsProcessRunning(string processName)
    {
        var processes = Process.GetProcessesByName(processName);

        try
        {
            return processes.Length > 0;
        }
        finally
        {
            foreach (var p in processes)
            {
                try { p.Dispose(); } catch { }
            }
        }
    }

    public static void StartProcess(string processPath)
    {
        if (string.IsNullOrWhiteSpace(processPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error problem running the program : Invalid process path");
            Console.ResetColor();
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = processPath,
                UseShellExecute = true,
                CreateNoWindow = false
            });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The program was successfully implemented.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error problem running the program :{ex.Message}");
            Console.ResetColor();
        }
    }

    public static bool IsBatRunningFromPath(string batPath)
    {
        if (string.IsNullOrWhiteSpace(batPath))
        {
            return false;
        }

        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE Name='cmd.exe'"))
            using (var collection = searcher.Get())
            {
                foreach (ManagementBaseObject process in collection)
                {
                    try
                    {
                        string commandLine = process["CommandLine"]?.ToString() ?? "";

                        if (commandLine.Contains(batPath, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        // ignore individual process read errors
                    }
                    finally
                    {
                        if (process is IDisposable d) d.Dispose();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error : {ex.Message}");
            Console.ResetColor();
        }

        return false;
    }

    public static void KillOneInstance(string processName)
    {
        var processes = Process.GetProcessesByName(processName);

        if (processes.Length > 1)
        {
            using var processToKill = processes.OrderByDescending(p => p.StartTime).FirstOrDefault();

            try
            {

                if (processToKill != null)
                {
                    processToKill.Kill();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Process {processToKill.Id} is closed.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error to closing the program : {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                foreach (var p in processes)
                {
                    try
                    {
                        if (!ReferenceEquals(p, processToKill))
                        {
                            p.Dispose();
                        }
                    }
                    catch { }
                }

                try { processToKill?.Dispose(); } catch { }
            }
        }
    }

    public static OpenFileDialog OpenDialogFile(string titleName, string filter)
    {
        string selectedFileName = null!;
        var thread = new Thread(() =>
        {
            using var openFileDialog = new OpenFileDialog()
            {
                Title = titleName,
                Filter = $"Executable files (*{filter})|*{filter}"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFileName = openFileDialog.FileName;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("File: " + openFileDialog.FileName);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("No file selected");
                Console.ResetColor();
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        var result = new OpenFileDialog()
        {
            Title = titleName,
            Filter = $"Executable files (*{filter})|*{filter}",
            FileName = selectedFileName ?? string.Empty
        };

        return result;
    }
}