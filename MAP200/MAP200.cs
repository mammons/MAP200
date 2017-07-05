using System;
using System.Threading;
using System.Diagnostics;
using NLog;
using System.Linq;

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
            get { return GetVerbosePctStatus(); }
        }
        public string Location { get; set; } = "CAR";
        public string Model { get; set; }
        public string Name { get; set; } = "DevMap";
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
            GetSetInfo();
        }

        public bool GetSetInfo()
        {
            var op = new OperationResult();
            op = SendCommand("IDN?", requestResponse: true);
            setInfo = op.Success ? op.Messages[0] : "Could not retrieve set info";

            if (op.Success)
            {
                Model = setInfo.Split(',')[1];
                SerialNumber = setInfo.Split(',')[2];
            }

            return op.Success;
        }

        public bool IsConnected()
        {
            var op = new OperationResult();
            op = SendCommand("IDN?", requestResponse: true);
            return op.Messages.Any();
        }

        public OperationResult SendCommand(string command, bool requestResponse)
        {
           return conman.SendCommandToCmr(command, requestResponse);            
        }

        public string GetVerbosePctStatus()
        {
            string status;

            status = HasPctRunning() ? "PCT Ready" : "PCT needs to be started";
            logger.Debug("Verbose PCT Status: {0}", status);

            return status;
        }

        public string GetPctStatus()
        {
            logger.Debug("Getting PCT Status...");

            var op = new OperationResult();

            op = SendCommand(":SUPer:STATus? PCT", requestResponse: true);

            string status = op.Messages[0];

            logger.Debug("PCT Status: {0}", status.Trim());

            return status;
        }

        /// <summary>
        /// Sends the command to get the PCT status and returns whether it's running or not
        /// </summary>
        /// <returns>True if PCT is running. Otherwise false</returns>
        public bool HasPctRunning()
        {
            return GetPctStatus().Trim().Equals("1");
        }

        /// <summary>
        /// The Launch command does not return a response from the MAP200 so we have to check the status until we get a 1 indicating it's running.
        /// If we don't get a 1 within 5 seconds, we throw a timeout exception
        /// </summary>
        public void StartPct()
        {
            //send the command to launch the PCT 
            SendCommand("SUPer:LAUNch PCT", requestResponse: false);

            //create and start the timeout timer
            Stopwatch sw = new Stopwatch();
            sw.Start();


            while(GetPctStatus().Trim() != "1")
            {
                Thread.Sleep(500);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException("Timed out starting the PCT");
            }
            pct = new PCT();
        }

        /// <summary>
        /// The exit command does not return a response from the MAP200 so we have to check the status until we get a 0 indicating it's stopped.
        /// If we don't get a 0 within 5 seconds, we throw a timeout exception
        /// </summary>
        public void StopPct()
        {
            SendCommand("SUPer:EXIT PCT", requestResponse: false);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while(GetPctStatus().Trim() != "0")
            {
                Thread.Sleep(500);
                if (sw.ElapsedMilliseconds > 5000) throw new TimeoutException("Timed out stopping the PCT");
            }
        }
    }
}