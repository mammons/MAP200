using System;
using Ivi.Visa.Interop;
using System.Threading.Tasks;
using NLog;

namespace MAP200
{
    public class MAP200_ConnectionManager
    {
        //Object Variables
        public FormattedIO488 ioObject { get; set; }
        public ResourceManager resourceManager { get; set; }

        //Class variables
        public string resourceName { get; set; } = "TCPIP0::135.84.72.170::INST0::INSTR";
        public bool cmrIsConnected { get; set; } = false;
        public bool pctIsConnected { get; set; } = false;
        public string response { get; set; } = "";

        //Log
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //Events
        public delegate void MAP200MessageSendingEventHandler(object sender, EventArgs e);
        public event MAP200MessageSendingEventHandler MAP200MessageSending;

        public delegate void MAP200MessageReceivedEventHandler(object sender, MAP200MessageEventArgs e);
        public event MAP200MessageReceivedEventHandler MAP200MessageReceived;

        public delegate void MAP200ConnectionFailedEventHandler(object sender, MAP200MessageEventArgs e);
        public event MAP200ConnectionFailedEventHandler MAP200ConnectionFailed;


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

        public void OpenConnectionToCmr()
        {
            logger.Debug("Opening connection to CMR...");

            //Instantiates a new object if ioObject is null otherwise uses the existing object
            ioObject = ioObject ?? new FormattedIO488();

            //Instantiates a new object if resourceManager is null
            resourceManager =  resourceManager ?? new ResourceManager();

            //Try to open a connection to the test set
            try
            {
                //Open a new connection to the MAP200 CMR
                ioObject.IO = (IMessage)resourceManager.Open(ResourceName: resourceName, mode: AccessMode.NO_LOCK, openTimeout: 5000, OptionString: "");
                ioObject.IO.TerminationCharacterEnabled = true;
                cmrIsConnected = true;
                logger.Debug("Connection open");
            }
            catch (Exception ex)
            {
                cmrIsConnected = false;
                logger.Debug("Connect failed: {0}", ex.Message);
                response = "Connection to MAP200 failed. Check that the device is correctly configured.";
                OnMAP200ConnectionFailed();
            }
        }

        public string SendCommandToCmr(string command, bool requestResponse)
        {
            OpenConnectionToCmr();

            if (cmrIsConnected)
            {                
                try
                {
                    ioObject.WriteString(command + "\n", true);

                    if (requestResponse)
                    {
                        //ReadString parameter corresponds to timeout
                        response = ioObject.IO.ReadString(5000);
                    }
                    else
                    {
                        response = "Command sent";
                    }
                }
                catch (Exception ex)
                {
                    response = "An error occurred: " + ex.Message;
                    logger.Debug(response);
                }
                finally
                {
                    try
                    {
                        //seems like close takes a while and doesn't return anything anyway so this should put it on another
                        //thread so it doesn't block
                        TaskEx.Run(() => ioObject.IO.Close());
                    }
                    catch { }
                }
            }
            //if response is null then the command failed, otherwise we want to return whatever response we got or assigned
            response = response ?? "Failed to send command";
            logger.Debug(response);
            return response;
        }

        protected virtual void OnMAP200MessageReceived()
        {
            MAP200MessageReceived?.Invoke(this, new MAP200MessageEventArgs(response));
        }

        protected virtual void OnMAP200ConnectionFailed()
        {
            MAP200ConnectionFailed?.Invoke(this, new MAP200MessageEventArgs(response));
        }
    }
}