using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivi.Driver.Interop;
using JDSU.CMR.Interop;
using Ivi.Visa.Interop;

namespace MAP200
{
    public class MAP200
    {
        public string resourceName { get; set; } = "TCPIP0::135.84.72.169::INST0::INSTR";
        public string setInfo { get; set; }
        public bool isConnected { get; set; }
        public bool isReadyToTest { get; set; }
        public FormattedIO488 ioObject { get; set; }
        public ResourceManager resourceManager { get; set; }

        public string sendCommand(string command, bool requestResponse)
        {
            string response;
            if (isConnected)
            {

                try
                {
                    ioObject.WriteString(command + "\n", true);

                    if (requestResponse)
                    {
                        //ReadString parameter corresponds to timeout
                        response = ioObject.IO.ReadString(5000);
                        return response;
                    }
                    else
                    {
                        return "Command sent";
                    }
                }
                catch (Exception ex)
                {
                    return "An error occurred: " + ex.Message;
                }
                finally
                {
                    try { ioObject.IO.Close(); }
                    catch { }
                }
            }
            else
            {
                return "The MAP200 is not connected";
            }
        }

        /// <summary>
        /// Opens a connection to the MAP200 chassis. If no IP string is passed, uses the current default IP of 135.84.72.169
        /// </summary>
        /// <returns>Returns the IDN? of the test set if successful. Otherwise returns the exception</returns>
        public string openConnection()
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
                isConnected = true;

                //Try and retrieve set info and return success message
                setInfo = sendCommand("IDN?", requestResponse: true);
                return string.Format("{0} connected!", setInfo);
            }
            catch(Exception ex)
            {
                isConnected = false;
                return ex.Message;
            }
        }
    }
}