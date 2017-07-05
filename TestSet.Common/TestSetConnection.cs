using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestSetLib
{
    public class TestSetConnection
    {
        public OperationResult Response { get; set; }
        public bool Connected { get; set; }
        public List<string> ErrorMessages { get; set; }

        public TestSetConnection()
        {
            Response = new OperationResult();
            ErrorMessages = new List<string>();
        }
    }
}