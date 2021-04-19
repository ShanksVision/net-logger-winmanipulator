using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace Deployment.Core.Windows
{    

    public static class ExecuteCommands
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hwnd, bool enabled);


        /// <span class="code-SummaryComment"><summary></span>
        /// Executes a shell command synchronously.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="command">string command</param></span>
        /// <span class="code-SummaryComment"><returns>string, as output of the command.</returns></span>
        public static string ExecuteCommandSync(string command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                
                // Display the command output.
                return result;
                //Console.WriteLine(result);
            }
            catch (Exception)
            {
                // Log the exception
                return "No output";
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Execute the command Asynchronously.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="command">string command.</param></span>
        private static string ExecuteCommandAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Task<string> ExecuteCommandAsyncTask = Task.Factory.StartNew(() => ExecuteCommandSync(command));
                //Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                //objThread.IsBackground = true;
                //Set the Priority of the thread.
                //objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                //objThread.Start(command);
                return ExecuteCommandAsyncTask.Result;

            }
            catch (Exception)
            {
                // Log the exception
                return String.Empty;
            }
          
        }

        public static bool LaunchApplication(string ApplicationPath)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo(ApplicationPath);                
                procStartInfo.RedirectStandardOutput = false;
                procStartInfo.UseShellExecute = false;                
                procStartInfo.CreateNoWindow = false;
                // Now we create a process, assign its ProcessStartInfo and start it
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();   
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Process LaunchApplicationWithComms(string ApplicationPath)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo(ApplicationPath);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = false;
                // Now we create a process, assign its ProcessStartInfo and start it
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                return proc;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool LaunchApplicationWithNoWindow(string ApplicationPath)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo(ApplicationPath);
                procStartInfo.RedirectStandardOutput = false;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ActivateAndMaximizeWindow(string ProcessName)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(ProcessName);
                if (proc.Length > 0)
                {
                    ShowWindow(proc[0].MainWindowHandle, 3);
                    SetForegroundWindowNative(proc[0].MainWindowHandle);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ActivateAndMinimizeWindow(string ProcessName)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(ProcessName);
                if (proc.Length > 0)
                {
                    ShowWindow(proc[0].MainWindowHandle, 2);
                    SetForegroundWindowNative(proc[0].MainWindowHandle);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool EnableWindow(string ProcessName, bool Enabled)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(ProcessName);
                if (proc.Length > 0)
                {

                    EnableWindow(proc[0].MainWindowHandle, Enabled);                                      
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ShowProcessWindow(string WindowName)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(WindowName);
                if (proc.Length > 0)
                {
                    IntPtr HWND = FindWindow(null, WindowName);
                    ShowWindow(HWND, 5);
                    EnableWindow(HWND, true);
                    SetForegroundWindowNative(HWND);
                    return true;                    
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                
            }
        }

        public static bool HideProcessWindow(string ProcessName)
        {
            try
            {
                Process[] proc = Process.GetProcessesByName(ProcessName);
                if (proc.Length > 0)
                {
                    ShowWindow(proc[0].MainWindowHandle, 0);                   
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool GetScreenShot(string ImagePath)
        {
            try
            {
                Rectangle bounds = Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    bitmap.Save(ImagePath, System.Drawing.Imaging.ImageFormat.Png);
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static long GetCurrentProcessMemoryinMB()
        {
            try
            {
                // get the current process
                return (long) new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName).NextValue()/(1048576);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int GetAvailableMemoryinPercent()
        {
            try
            {
                //PerformanceCounter pCount = new PerformanceCounter();
                //pCount.CategoryName = "Memory";
                //pCount.CounterName = "Available MBytes";                
                //return pCount.RawValue.ToString();

                Microsoft.VisualBasic.Devices.ComputerInfo CompInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();

                return (int)(((double)CompInfo.AvailablePhysicalMemory / CompInfo.TotalPhysicalMemory) * 100);
                
            }
            catch (Exception)
            {
                return -1;
            }

        }

    }
}
