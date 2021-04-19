using System;
using Deployment.Core.Logging;

namespace Deployment.Core.Bases
{
    public class BaseClass
    {
        #region Attributes
        private String msClassName = String.Empty;
        private Logger mobjLogger = null;
        #endregion

        #region Properties
        protected String ClassName
        {
            get { return msClassName; }
        }
        protected Logger Logger
        {
            get { return mobjLogger; }
            set { mobjLogger = value; }
        }
        #endregion

        #region Constructors
        public BaseClass(String ClassName)
        {
            msClassName = ClassName;
        }
        #endregion
    }
}
