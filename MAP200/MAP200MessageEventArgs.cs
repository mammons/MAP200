using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestSetLib;

namespace MAP200
{
    public class MAP200MessageEventArgs
    {
        private OperationResult _response;
        private TestSetConnection _connection;

        public MAP200MessageEventArgs(OperationResult response)
        {
            _response = response;
        }

        public OperationResult response
        {
            get { return _response; }
        }

        public MAP200MessageEventArgs(TestSetConnection connection)
        {
            _connection = connection;
        }

        public TestSetConnection Connection
        {
            get { return _connection; }
        }
    }
}