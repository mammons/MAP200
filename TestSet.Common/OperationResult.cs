using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestSetLib
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<string> Messages { get; set; }

        public OperationResult()
        {
            ErrorMessages = new List<string>();
            Messages = new List<string>();
        }
    }
}