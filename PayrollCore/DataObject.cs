using System;
using System.Collections.Generic;
using System.Text;

namespace PayrollCore
{
    public class DataObject
    {
        public Exception lastEx
        {
            get
            {
                return lastEx;
            }
            protected set
            {
                Client.Instance.lastError = value;
                lastEx = value;
            }
        }

        public string connString;
    }
}
