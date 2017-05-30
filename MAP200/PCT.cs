﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JDSU.MAP200_PCT.Interop;
using System.Threading;
using Newtonsoft.Json;
using NLog;

namespace MAP200
{
    /// <summary>
    /// The PCT class manages the PCT with IVI drivers. The PCT is an application running on the MAP200 which actually runs the test
    /// to return the needed values.
    /// </summary>
    public class PCT
    {
        /// <summary>
        /// IVI driver objects
        /// </summary>
        public MAP200_PCT pctConnection;
        private IMAP200_PCTStatus IStatus;
        private IMAP200_PCTDevice IDevice;
        private IMAP200_PCTDeviceMeasurement IMeas;
        private IMAP200_PCTDevices IDevices;
        private IMAP200_PCTDeviceMeasurementSetup ISetup;
        private IMAP200_PCTDevicePath IPath;
        private IMAP200_PCTSystem ISystem;

        public MAP200_Results results { get; set; }

        public string pctResourceName { get; set; } = "TCPIP0::135.84.72.165::8301::SOCKET";
        public string status { get; set; }

        public bool isConnected { get; set; }
        public bool isReadyToTest { get; set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The PCT is the test application that is installed on the MAP200. It is necessary for the application to be running
        /// if any tests are to be run.
        /// </summary>
        public PCT()
        {
            pctConnection = new MAP200_PCT();
            results = new MAP200_Results();
        }


        /// <summary>
        /// Initializes a connection with the PCT application via IVI commands
        /// </summary>
        /// <returns></returns>
        public string Initialize()
        {
            if (!pctConnection.Initialized)
            {
                try
                {

                    pctConnection.Initialize(pctResourceName, false, false, "Simulate = false");
                    isConnected = true;
                    return "PCT Connected";
                }
                catch (Exception ex)
                {
                    isConnected = false;
                    return ex.Message;
                }
            }
            return ("PCT Initialized");
        }

        /// <summary>
        /// Once the connection is initialized we can assign values to our interface variables which we use to perform the test
        /// </summary>
        private void AssignInterfaces()
        {
            if (isConnected)
            {
                IStatus = pctConnection.Status;
                IDevices = pctConnection.Devices;
                ISystem = pctConnection.System;

                IDevice = IDevices.Item[IDevices.Name[1]];
                ISetup = IDevice.Measurement.Setup;
                IMeas = IDevice.Measurement;
                IPath = IDevice.Path;

                isReadyToTest = true;
            }
            else
            {
                isReadyToTest = false;
            }
        }

        /// <summary>
        /// Runs the insertion loss/return loss test on the MAP200 with the connected jumper
        /// </summary>
        /// <returns>A list that contains the values for insertion loss, return loss, and length</returns>
        public IEnumerable<string> runTest()
        {
            List<string> results = new List<string>();

            if (pctConnection.Initialized) { }
            else { Initialize(); }

            AssignInterfaces();

            if (isReadyToTest)
            {
                ISetup.Type = MAP200_PCTMeasurementTypeEnum.MAP200_PCTMeasurementTypeDUT;
                IMeas.Initiate();

                do
                {
                    Thread.Sleep(100);
                } while (IMeas.State == MAP200_PCTMeasurementStateEnum.MAP200_PCTMeasurementState_Busy);

                string msg = ISystem.GetWarning();
                if (!msg.Equals("No Warning"))
                {
                    Console.WriteLine(msg);
                }

                double insertionLoss = IMeas.GetIL();
                double returnLoss = IMeas.GetORL(MAP200_PCTORLMethodEnum.MAP200_PCTORLMethodIntegrate, MAP200_PCTORLOriginEnum.MAP200_PCTORLOriginABstart, 0.82, 0.82);
                double length = IMeas.GetLength();


                results.Add(string.Format("Insertion Loss: {0}", insertionLoss.ToString()));
                results.Add(string.Format("Return Loss: {0} ", returnLoss.ToString()));
                results.Add(string.Format("Length: {0} ", length.ToString()));

                pctConnection.Close();
            }
            else
            {
                results.Add("PCT not ready for test");
            }
            var jsonResults = JsonConvert.SerializeObject(results);
            return results;
        }

        public TestSetMessage runTestAndReturnAsJson()
        {
            if (pctConnection.Initialized) { }
            else { Initialize(); }

            AssignInterfaces();

            if (isReadyToTest)
            {
                try
                {
                    ISetup.Type = MAP200_PCTMeasurementTypeEnum.MAP200_PCTMeasurementTypeDUT;
                    IMeas.Initiate();

                    do
                    {
                        Thread.Sleep(100);
                    } while (IMeas.State == MAP200_PCTMeasurementStateEnum.MAP200_PCTMeasurementState_Busy);

                    string msg = ISystem.GetWarning();
                    if (!msg.Equals("No Warning"))
                    {
                        Console.WriteLine(msg);
                    }

                    results.insertionLoss = IMeas.GetIL().ToString();
                    results.returnLoss = IMeas.GetORL(MAP200_PCTORLMethodEnum.MAP200_PCTORLMethodIntegrate, MAP200_PCTORLOriginEnum.MAP200_PCTORLOriginABstart, 0.82, 0.82).ToString();
                    results.length = IMeas.GetLength().ToString();
                }
                catch (Exception ex)
                {
                    results.message.Response.Message = ex.Message;
                }
                finally
                {
                    pctConnection.Close();
                }
            }
            else
            {
                results.message.Response.Message = "PCT not ready to test";
            }
            results.buildJson();
            return results.message;
        }


        public void closeConnection()
        {
            pctConnection.Close();
        }
    }
}