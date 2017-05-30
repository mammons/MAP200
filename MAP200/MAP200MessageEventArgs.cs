using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAP200
{
    public class MAP200MessageEventArgs
    {
        private string _response;

        public MAP200MessageEventArgs(string response)
        {
            _response = response;
        }

        public string response
        {
            get { return _response; }
        }
    }
}