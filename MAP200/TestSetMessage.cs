using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAP200
{
    public class TestSetMessage
    {
        public TestSetMessageHeader Header { get; set; }
        public TestSetMessageResponse Response { get; set; }
        public Dictionary<string, string> Body { get; set; }

        public TestSetMessage()
        {
            Header = new TestSetMessageHeader();
            Response = new TestSetMessageResponse();
            Body = new Dictionary<string, string>();
        }
    }
}