using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivi.Visa.Interop;

namespace MAP200
{
    public class MAP200_ConnectionManager
    {
        public FormattedIO488 ioObject { get; set; }
        public ResourceManager resourceManager { get; set; }

        public string resourceName { get; set; } = "TCPIP0::135.84.72.169::INST0::INSTR";

        public bool cmrIsConnected { get; set; } = false;
        public bool pctIsConnected { get; set; } = false;


        public MAP200_ConnectionManager()
        {
            ioObject = new FormattedIO488();
            resourceManager = new ResourceManager();
        }

        public MAP200_ConnectionManager(string instrumentAddress)
        {
            ioObject = new FormattedIO488();
            resourceManager = new ResourceManager();
            resourceName = instrumentAddress;
        }

        public string openConnectionToCmr()
        {
            //Instantiates a new object if ioObject is null
            ioObject = new FormattedIO488() ?? ioObject;

            //Instantiates a new object if resourceManager is null
            resourceManager = new ResourceManager() ?? resourceManager;

            //Try to open a connection to the test set and if successful, return the IDN
            try
            {
                ioObject.IO = (IMessage)resourceManager.Open(resourceName, AccessMode.NO_LOCK, 0, "");
                ioObject.IO.TerminationCharacterEnabled = true;
                cmrIsConnected = true;
                return "1";
            }
            catch (Exception ex)
            {
                cmrIsConnected = false;
                return ex.Message;
            }
        }

        public void manageBaseConnection()
        {
            if(ioObject.IO == null)
            {
                openConnectionToCmr();
            }
            else if(ioObject.IO != null && ioObject.IO.)
        }
    }
}