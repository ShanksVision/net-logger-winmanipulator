using Deployment.Core.Bases;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
namespace Deployment.Core.Logging
{
    public class Log : BaseClass
    {
        public const string cDateFormat = "yyyy.MM.dd HH:mm:ss.FFFFFFF";
        private const string cThreadFormat = "0000";
        private int mnThreadID;
        private string msThreadName = string.Empty;
        private DateTime mdteEventdate;
        private EventLogEntryType menumLogType = EventLogEntryType.Information;
        private string msAdditionalInfo = string.Empty;
        private LogLevel menumLogLevel = LogLevel.Info;
        private string msKeyWords = string.Empty;
        private string msMessage = string.Empty;
        private string msMethod = string.Empty;
        private string msClass = string.Empty;
        private string msUser = string.Empty;
        public DateTime Eventdate
        {
            get
            {
                return this.mdteEventdate;
            }
            set
            {
                this.mdteEventdate = value;
            }
        }
        public EventLogEntryType LogType
        {
            get
            {
                return this.menumLogType;
            }
            set
            {
                this.menumLogType = value;
            }
        }
        public LogLevel LogLevel
        {
            get
            {
                return this.menumLogLevel;
            }
            set
            {
                this.menumLogLevel = value;
            }
        }
        public string KeyWords
        {
            get
            {
                return this.msKeyWords.Replace(',', '_');
            }
            set
            {
                this.msKeyWords = value;
            }
        }
        public string AdditionalInfo
        {
            get
            {
                return this.msAdditionalInfo;
            }
            set
            {
                this.msAdditionalInfo = value;
            }
        }
        public string Message
        {
            get
            {
                return this.msMessage.Replace(',', '_');
            }
            set
            {
                this.msMessage = value;
            }
        }
        public string Method
        {
            get
            {
                return this.msMethod.Replace(',', '_');
            }
            set
            {
                this.msMethod = value;
            }
        }
        public string Class
        {
            get
            {
                return this.msClass.Replace(',', '_');
            }
            set
            {
                this.msClass = value;
            }
        }
        public string User
        {
            get
            {
                return this.msUser.Replace(',', '_');
            }
            set
            {
                this.msUser = value;
            }
        }
        private string ThreadName
        {
            get
            {
                return this.msThreadName;
            }
            set
            {
                this.msThreadName = value;
            }
        }
        public Log(string Message, EventLogEntryType LogType, LogLevel LogLevel, string Class, string Method, string KeyWords, string User, string AdditionalInfo = "")
            : base("Log")
        {
            this.mnThreadID = Thread.CurrentThread.GetHashCode();
            this.msThreadName = Thread.CurrentThread.Name;
            this.Eventdate = DateTime.Now;
            this.LogType = LogType;
            this.LogLevel = LogLevel;
            this.KeyWords = KeyWords;
            this.Message = Message;
            this.Method = Method;
            this.Class = Class;
            this.User = User;
        }
        public string FlatFileHeader()
        {
            string result;
            try
            {
                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append("Eventdate" + ',');
                objStringBuilder.Append("Message" + ',');
                objStringBuilder.Append("Class" + ',');
                objStringBuilder.Append("Method" + ',');
                objStringBuilder.Append("LogType" + ',');
                objStringBuilder.Append("LogLevel" + ',');
                objStringBuilder.Append("KeyWords" + ',');
                objStringBuilder.Append("User" + ',');
                objStringBuilder.Append("Thread" + ',');
                objStringBuilder.Append("ThreadName" + ',');
                objStringBuilder.Append("Additional Info");
                result = objStringBuilder.ToString();
            }
            catch (Exception)
            {
                result = null;
            }
            finally
            {
            }
            return result;
        }
        public string ToFlatFileString()
        {
            string result;
            try
            {
                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append(this.Eventdate.ToString("yyyy.MM.dd HH:mm:ss.FFFFFFF") + ',');
                objStringBuilder.Append(this.Message + ',');
                objStringBuilder.Append(this.Class + ',');
                objStringBuilder.Append(this.Method + ',');
                objStringBuilder.Append(this.LogType.ToString() + ',');
                objStringBuilder.Append(this.LogLevel.ToString() + ',');
                objStringBuilder.Append(this.KeyWords + ',');
                objStringBuilder.Append(this.User + ',');
                objStringBuilder.Append(this.mnThreadID.ToString("0000") + ',');
                objStringBuilder.Append(this.msThreadName + ',');
                objStringBuilder.Append(this.AdditionalInfo);
                result = objStringBuilder.ToString();
            }
            catch (Exception)
            {
                result = null;
            }
            finally
            {
            }
            return result;
        }
        public void ToWindowsLogger()
        {
            try
            {
                if (!EventLog.SourceExists("IntegroVisionLogger"))
                {
                    EventLog.CreateEventSource("IntegroVisionLogger", "ITC");
                }
                EventLog objEventLog = new EventLog();
                objEventLog.Source = "IntegroVisionLogger";
                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.AppendLine("ThreadID = " + this.mnThreadID.ToString("0000") + ',');
                objStringBuilder.AppendLine("EventDate = " + this.Eventdate.ToString("yyyy.MM.dd HH:mm:ss.FFFFFFF") + ',');
                objStringBuilder.AppendLine("Class = " + this.Class + ',');
                objStringBuilder.AppendLine("Method = " + this.Method + ',');
                objStringBuilder.AppendLine("Log Type = " + this.LogType.ToString() + ',');
                objStringBuilder.AppendLine("Log Level = " + this.LogLevel.ToString() + ',');
                objStringBuilder.AppendLine("Keywords = " + this.KeyWords + ',');
                objStringBuilder.AppendLine("Message = " + this.Message + ',');
                objStringBuilder.AppendLine("User = " + this.User);
                objEventLog.WriteEntry(objStringBuilder.ToString(), (System.Diagnostics.EventLogEntryType)this.LogType);               
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }
    }
}
