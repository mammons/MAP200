using System;
using Ivi.Visa.Interop;
using System.Threading.Tasks;
using NLog;
using System.Linq;

namespace MAP200
{
    public class MAP200_ConnectionManager
    {
        //Object Variables
        public FormattedIO488 ioObject { get; set; }
        public ResourceManager resourceManager { get; set; }

        //Class variables
        public string resourceName { get; set; } = "TCPIP0::135.84.72.169::INST0::INSTR";

        public string response { get; set; } = "";

        //Log
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //Events
        public delegate void MAP200MessageSendingEventHandler(object sender, MAP200MessageEventArgs e);
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

        public TestSetConnection OpenConnectionToCmr()
        {
            logger.Debug("Opening connection to CMR...");

            //Instantiates a new object if ioObject is null otherwise uses the existing object
            ioObject = ioObject ?? new FormattedIO488();

            //Instantiates a new object if resourceManager is null
            resourceManager =  resourceManager ?? new ResourceManager();

            var connection = new TestSetConnection();

            //Try to open a connection to the test set
            try
            {
                //Open a new connection to the MAP200 CMR
                ioObject.IO = (IMessage)resourceManager.Open(ResourceName: resourceName, mode: AccessMode.NO_LOCK, openTimeout: 5000, OptionString: "");
                ioObject.IO.TerminationCharacterEnabled = true;
                connection.Connected = true;
                logger.Debug("Connection open");
            }
            catch (Exception ex)
            {
                connection.Connected = false;
                logger.Debug("Connect failed: {0}", ex.Message);
                connection.ErrorMessages.Add("Connection to MAP200 failed. Check that the device is correctly configured.");
                OnMAP200ConnectionFailed(connection);
            }

            return connection;
        }

        public OperationResult SendCommandToCmr(string command, bool requestResponse)
        {
            var connection = OpenConnectionToCmr();

            var response = new OperationResult();

            if (connection.Connected)
            {                
                try
                {
                    ioObject.WriteString(command + "\n",flushAndEND: true);

                    if (requestResponse)
                    {
                        response.Messages.Add(ioObject.IO.ReadString(count: 5000));
                    }
                    else
                    {
                        response.Messages.Add("Command sent");                        
                    }
                }
                catch (Exception ex)
                {
                    response.ErrorMessages.Add("An error occurred: " + ex.Message);
                    logger.Debug(response.ErrorMessages);
                }
                finally
                {
                    response.Success = (response.Messages != null && response.Messages.Any());
                    try
                    {
                        //seems like close takes a while and doesn't return anything anyway so this should put it on another
                        //thread so it doesn't block
                        TaskEx.Run(() => ioObject.IO.Close());
                    }
                    catch { }
                }
            }
            logger.Debug(response);
            return response;
        }

        protected virtual void OnMAP200MessageReceived(OperationResult op)
        {
            MAP200MessageReceived?.Invoke(this, new MAP200MessageEventArgs(op));
        }

        protected virtual void OnMAP200ConnectionFailed(TestSetConnection connection)
        {
            MAP200ConnectionFailed?.Invoke(this, new MAP200MessageEventArgs(connection));
        }

        protected virtual void OnMAP200MessageSending(OperationResult op)
        {
            MAP200MessageSending?.Invoke(this, new MAP200MessageEventArgs(op));
        }
    }
}