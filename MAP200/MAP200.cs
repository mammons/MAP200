﻿using System;
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
        public bool hasPctRunning { get; set; }
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

        ///// <summary>
        ///// Opens a connection to the MAP200 chassis. If no IP string is passed, uses the current default IP of 135.84.72.169
        ///// </summary>
        ///// <returns>Returns the IDN? of the test set if successful. Otherwise returns the exception</returns>
        //public string openConnection()
        //{
        //        //Try and retrieve set info and return success message
        //        setInfo = sendCommand("IDN?", requestResponse: true);
        //        return string.Format("{0} connected!", setInfo.Trim());

        //}

        public string getVerbosePctStatus()
        {
            string setResponse;
            string status;

            setResponse = getPctStatus();

            status = hasPctRunning ? "PCT Ready" : "PCT needs to be started";
            return status;
        }

        public string getPctStatus()
        {
            string status = sendCommand(":SUPer:STATus? PCT", requestResponse: true);
            hasPctRunning = status.Trim() == "1";
            return status;
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
            pct.closeConnection();
        }
    }
}