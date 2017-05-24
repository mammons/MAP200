using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivi.Driver.Interop;
using JDSU.CMR.Interop;
using Ivi.Visa.Interop;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MAP200
{
    public class MAP200
    {
        public string resourceName { get; set; } = "TCPIP0::135.84.72.169::INST0::INSTR";
        public string setInfo { get; set; }
        public string verbosePctStatus {
            get { return getVerbosePctStatus(); }
        }
        public bool isConnected { get; set; }
        public bool isReadyToTest { get; set; }
        public MAP200_ConnectionManager conman { get; set; }
        public PCT pct { get; set; }

        public MAP200()
        {
            conman = new MAP200_ConnectionManager();
            pct = new PCT();
        }

        public string sendCommand(string command, bool requestResponse)
        {
           return conman.sendCommandToCmr(command, requestResponse);            
        }

        public string getVerbosePctStatus()
        {
            string status;

            status = hasPctRunning() ? "PCT Ready" : "PCT needs to be started";
            return status;
        }

        public string getPctStatus()
        {
            string status = sendCommand(":SUPer:STATus? PCT", requestResponse: true);
            return status;
        }

        public bool hasPctRunning()
        {
            return getPctStatus().Trim().Equals("1");
        }

        /// <summary>
        /// The Launch command does not return a response from the MAP200.
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