using Deployment.Core.Bases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
namespace Deployment.Core.Logging
{
    public class Logger : BaseClass
    {
        private Thread mobjThreadWorkerLogMessages;
        private Thread mobjThreadWorkerDeleteFiles;
        private Queue<Log> mQueueLogs;
        private bool mbQuitLogging;
        private LogLevel menumLogLevel = LogLevel.Info;
        private LogDestination menumLogDestination;
        private string msPath = string.Empty;
        private int mnFileRetentionDays;
        public LogLevel LogLevel
        {
            get
            {
                return this.menumLogLevel;
            }
            private set
            {
                this.menumLogLevel = value;
            }
        }
        public LogDestination LogDestination
        {
            get
            {
                return this.menumLogDestination;
            }
            set
            {
                this.menumLogDestination = value;
            }
        }
        public string Path
        {
            get
            {
                return this.msPath;
            }
            private set
            {
                this.msPath = value;
            }
        }
        public bool QuitLogging
        {
            get
            {
                return this.mbQuitLogging;
            }
            set
            {
                this.mbQuitLogging = value;
            }
        }
        public int FileRetentionDays
        {
            get
            {
                return this.mnFileRetentionDays;
            }
            private set
            {
                this.mnFileRetentionDays = value;
            }
        }
        private string FileName
        {
            get
            {
                string result;
                try
                {
                    StringBuilder objStringBuilder = new StringBuilder();
                    objStringBuilder.Append(this.Path + '\\');
                    objStringBuilder.Append(DateTime.Now.Year.ToString("0000") + '.');
                    objStringBuilder.Append(DateTime.Now.Month.ToString("00") + '.');
                    objStringBuilder.Append(DateTime.Now.Day.ToString("00") + ' ');
                    objStringBuilder.Append("Log.csv");
                    result = objStringBuilder.ToString();
                }
                finally
                {
                }
                return result;
            }
        }
        public Logger(LogLevel LogLevel, int FileRetentionDays, string Path)
            : base("Logging")
        {
            this.LogLevel = LogLevel;
            this.LogDestination = this.LogDestination;
            this.FileRetentionDays = FileRetentionDays;
            this.Path = Path;
            this.mQueueLogs = new Queue<Log>();
            this.mobjThreadWorkerLogMessages = new Thread(new ThreadStart(this.WorkerMethodLogMessages));
            this.mobjThreadWorkerLogMessages.IsBackground = true;
            this.mobjThreadWorkerLogMessages.Name = "WorkerThreadLogMessage";
            this.mobjThreadWorkerLogMessages.Start();
            this.LogMessage(new Log("---------------------------------------- N E W  L O G G E R  S T A R T E D ----------------------------------------", EventLogEntryType.Information, LogLevel.Verbose, "", "", "", "", ""));
            if (this.FileRetentionDays > 0)
            {
                this.mobjThreadWorkerDeleteFiles = new Thread(new ThreadStart(this.WorkerMethodDeleteFiles));
                this.mobjThreadWorkerDeleteFiles.IsBackground = true;
                this.mobjThreadWorkerDeleteFiles.Name = "WorkerThreadDeleteFiles";
                this.mobjThreadWorkerDeleteFiles.Start();
            }
        }
        private void WorkerMethodLogMessages()
        {
            while (!this.mbQuitLogging)
            {
                try
                {
                    this.GetNextMessageFromQueue();
                    Thread.Sleep(2);
                }
                catch (Exception)
                {
                }
            }
        }
        private bool IsOldFile(string File)
        {
            bool result;
            try
            {
                string sFile = StringFunctions.GetFileNameWithoutDirectory(File);
                string sYear = sFile.Substring(0, 4);
                string sMonth = sFile.Substring(5, 2);
                string sDay = sFile.Substring(8, 2);
                DateTime dteFileDate = Convert.ToDateTime(string.Concat(new object[]
				{
					sYear,
					'-',
					sMonth,
					'-',
					sDay
				}));
                result = (DateTime.Now.Subtract(dteFileDate).TotalDays > (double)this.FileRetentionDays);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        private void WorkerMethodDeleteFiles()
        {
            while (!this.mbQuitLogging)
            {
                try
                {
                    string[] sFiles = Directory.GetFiles(this.Path, "*Log.csv", SearchOption.TopDirectoryOnly);
                    string[] array = sFiles;
                    for (int i = 0; i < array.Length; i++)
                    {
                        string sFile = array[i];
                        if (this.IsOldFile(sFile))
                        {
                            File.Delete(sFile);
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                }
                Thread.Sleep(300000);
            }
        }
        private void GetNextMessageFromQueue()
        {
            Log objLog = null;
            try
            {
                objLog = null;
                Queue<Log> obj;
                Monitor.Enter(obj = this.mQueueLogs);
                try
                {
                    if (this.mQueueLogs.Count > 0)
                    {
                        objLog = this.mQueueLogs.Dequeue();
                        this.mQueueLogs.TrimExcess();
                    }
                }
                finally
                {
                    Monitor.Exit(obj);
                }
                if (objLog != null && objLog.LogLevel <= this.menumLogLevel)
                {
                    switch (this.LogDestination)
                    {
                        case LogDestination.Flatfile:
                            this.LogToFlatFile(objLog);
                            break;
                        case LogDestination.WindowsLogger:
                            this.LogToWindowsLogger(objLog);
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                objLog = null;
            }
        }
        public void LogToFlatFile(Log objLog)
        {
            string arg_05_0 = string.Empty;
            try
            {
                if (!Directory.Exists(this.Path))
                {
                    Directory.CreateDirectory(this.Path);
                }
                if (!File.Exists(this.FileName))
                {
                    using (StreamWriter objStreamWriter = new StreamWriter(this.FileName, true))
                    {
                        objStreamWriter.WriteLine(objLog.FlatFileHeader());
                    }
                }
                using (StreamWriter objStreamWriter2 = new StreamWriter(this.FileName, true))
                {
                    objStreamWriter2.WriteLine(objLog.ToFlatFileString());
                }
            }
            catch (Exception)
            {
            }
        }
        public void LogToWindowsLogger(Log objLog)
        {
            try
            {
                objLog.ToWindowsLogger();
            }
            catch (Exception)
            {
            }
        }
        public void LogMessage(Log objLog)
        {
            try
            {
                Queue<Log> obj;
                Monitor.Enter(obj = this.mQueueLogs);
                try
                {
                    this.mQueueLogs.Enqueue(objLog);
                }
                finally
                {
                    Monitor.Exit(obj);
                }
            }
            catch
            {
            }
        }
        public void LogException(Log objLog, Exception objEx)
        {
            string sPad = string.Empty;
            try
            {
                sPad = sPad.PadLeft("yyyy.MM.dd HH:mm:ss.FFFFFFF".Length + 1, ' ');
                objLog.LogLevel = LogLevel.Errors;
                objLog.LogType = EventLogEntryType.Error;
                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append("Exception " + StringFunctions.RemoveNonLoggableChars(objEx.ToString()));
                objStringBuilder.Append(" : Exception StackTrace = ");
                objStringBuilder.Append(StringFunctions.RemoveNonLoggableChars(objEx.StackTrace));
                if (objEx.InnerException != null)
                {
                    objStringBuilder.AppendLine("");
                    objStringBuilder.Append("Exception InnerException = ");
                    objStringBuilder.Append(objEx.InnerException.ToString());
                }
                objLog.AdditionalInfo = objStringBuilder.ToString();
                Queue<Log> obj;
                Monitor.Enter(obj = this.mQueueLogs);
                try
                {
                    this.mQueueLogs.Enqueue(objLog);
                }
                finally
                {
                    Monitor.Exit(obj);
                }
            }
            catch
            {
            }
            finally
            {
            }
        }
    }
}