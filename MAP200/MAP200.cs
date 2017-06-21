using System;
using System.Threading;
using System.Diagnostics;
using NLog;

namespace MAP200
{
    public class MAP200
    {
        //Object Variables
        public MAP200_ConnectionManager conman { get; set; }
        public PCT pct { get; set; }

        //Class variables
        public string setInfo { get; set; }
        public string verbosePctStatus {
            get { return getVerbosePctStatus(); }
        }
        public string Location { get; set; } = "CAR";
        public string Model { get; set; } 
        public string SerialNumber { get; set; }
        public string OperId { get; set; } = "MRA";
        private bool isConnected { get; set; }
        public bool isReadyToTest { get; set; }

        //NLog Instance
        private static Logger logger = LogManager.GetCurrentClassLogger();



        public MAP200()
        {
            conman = new MAP200_ConnectionManager();
            pct = new PCT();
            GetSerialNumber();
        }

        public void GetSerialNumber()
        {
            setInfo = sendCommand("IDN?", requestResponse: true);
            Model = setInfo.Split(',')[1];
            SerialNumber = setInfo.Split(',')[2];
        }

        public bool IsConnected()
        {
            isConnected = !string.IsNullOrWhiteSpace(sendCommand("IDN?", requestResponse: true));
            return isConnected;
        }

        public string sendCommand(string command, bool requestResponse)
        {
           return conman.sendCommandToCmr(command, requestResponse);            
        }

        public string getVerbosePctStatus()
        {
            string status;

            status = hasPctRunning() ? "PCT Ready" : "PCT needs to be started";
            logger.Debug("Verbose PCT Status: {0}", status);

            return status;
        }

        public string getPctStatus()
        {
            logger.Debug("Getting PCT Status...");
            string status = sendCommand(":SUPer:STATus? PCT", requestResponse: true);
            logger.Debug("PCT Status: {0}", status.Trim());

            return status;
        }

        /// <summary>
        /// Sends the command to get the PCT status and returns whether it's running or not
        /// </summary>
        /// <returns>True if PCT is running. Otherwise false</returns>
        public bool hasPctRunning()
        {
            return getPctStatus().Trim().Equals("1");
        }

        /// <summary>
        /// The Launch command does not return a response from the MAP200 so we have to check the status until we get a 1 indicating it's running.
        /// If we don't get a 1 within 5 seconds, we throw a timeout exception
        /// </summary>
        public void startPct()
        {
            //send the command to launch the PCT 
            sendCommand("SUPer:LAUNch PCT", requestResponse: false);

            //create and start the timeout timer
            Stopwatch sw = new Stopwatch();
            sw.Start();


            while(getPctStatus().Trim() != "1")
            {
                Thread.Sleep(500);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException();
            }
            pct = new PCT();
        }

        /// <summary>
        /// The exit command does not return a response from the MAP200 so we have to check the status until we get a 0 indicating it's stopped.
        /// If we don't get a 0 within 5 seconds, we throw a timeout exception
        /// </summary>
        public void stopPct()
        {
            sendCommand("SUPer:EXIT PCT", requestResponse: false);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while(getPctStatus().Trim() != "0")
            {
                Thread.Sleep(500);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException();
            }
        }
    }
}