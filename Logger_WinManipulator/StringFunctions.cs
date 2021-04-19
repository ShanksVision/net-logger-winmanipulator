using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deployment.Core
{
    class StringFunctions
    {
        static public String GetFileNameWithoutDirectory(String FileName)
        {
            int nStart = FileName.LastIndexOf('\\') + 1;
            return FileName.Substring(nStart);
        }

        static public String RemoveNonLoggableChars(String str)
        {
            string strReturn = string.Empty;
            string result;
            try
            {
                strReturn = str.Replace('\r', '_');
                strReturn = strReturn.Replace('\n', '_');
                strReturn = strReturn.Replace(',', '_');
                strReturn = strReturn.Replace('\f', '_');
                strReturn = strReturn.Replace('\t', '_');
                strReturn = strReturn.Replace('\v', '_');
                result = strReturn;
            }
            catch
            {
                result = str;
            }
            return result;
        }
    }
}
