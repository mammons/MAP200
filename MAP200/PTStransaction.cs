using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using Newtonsoft.Json;
using System.Net.Http;

namespace MAP200
{
    class PTStransaction
    {
        public string sendMessage { get; set; }
        public string serverUri { get; set; }
        //public TestSetMessage postMessage { get; set; }    

        public delegate void PTSMessageSendingEventHandler(object source, EventArgs args);
        public event PTSMessageSendingEventHandler PTSMessageSending;

        public delegate void PTSMessageReceivedEventHandler(object source, EventArgs args);
        public event PTSMessageReceivedEventHandler PTSMessageReceived;

        private string developmentUri = System.Configuration.ConfigurationManager.AppSettings["devURI"];
        private string productionUri = System.Configuration.ConfigurationManager.AppSettings["prodURI"];
        private string serviceUrl = "http://localhost:53962/api/PTSWebServices";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        

        public PTStransaction()
        {
            serverUri = System.Configuration.ConfigurationManager.AppSettings["Server"] == "DEV" ? developmentUri : productionUri;
        }


        public TestSetMessage PostToPTS(TestSetMessage json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUri);

                OnPTSMessageSending();
                HttpResponseMessage response = client.PostAsJsonAsync<TestSetMessage>(serviceUrl, json).Result;
                OnPTSMessageReceived();

                TestSetMessage responseMessage = new TestSetMessage();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        responseMessage = response.Content.ReadAsAsync<TestSetMessage>().Result;
                    }
                    catch (Exception e)
                    {
                        responseMessage = new TestSetMessage();
                        responseMessage.Response.Message = e.Message;
                        logger.Debug("Error: {0}", e.Message);
                    }
                }
                else
                {
                    responseMessage.Response.Message = "Communication with PTS failed";
                    logger.Debug("Error: {0} ", responseMessage.Response.Message);
                }
                return responseMessage;
            }
        }

        //TODO Change MAP200 to generic TestSet class with these basic properties so you can change the MAP200 type to a TestSet type for future sets
        public TestSetMessage SendUPFI(Jumper jumper, MAP200 testSet)
        {
            TestSetMessage postMessage = new TestSetMessage();
            BuildJson(jumper, testSet, out postMessage);
            postMessage.Header.Command1 = "UP";
            postMessage.Header.Command2 = "FI";

            return PostToPTS(postMessage);            
        }

        public TestSetMessage SendREFI(Jumper jumper, MAP200 testSet)
        {
            TestSetMessage postMessage = new TestSetMessage();
            BuildJson(jumper, testSet, out postMessage);
            postMessage.Header.Command1 = "RE";
            postMessage.Header.Command2 = "FI";

            return PostToPTS(postMessage);
        }

        public bool GetTestingRequired(Jumper jumper, MAP200 testSet)
        {
            TestSetMessage response = new TestSetMessage();
            response = SendREFI(jumper, testSet);

            bool testingRequired = response.Body["required"].Equals("yes");

            if (testingRequired) jumper.AssignLimits(response);

            return testingRequired;
        }


        private void BuildJson(Jumper jumper, MAP200 testSet, out TestSetMessage postMessage)
        {
            postMessage = new TestSetMessage();
            postMessage.Header.TestSetModel = testSet.Model;
            postMessage.Header.TestSetName = testSet.SerialNumber;
            postMessage.Header.Location = testSet.Location;
            postMessage.Header.OperId = testSet.OperId;
            postMessage.Body["serialNumber"] = jumper.SerialNumber;
            postMessage.Body["InsertionLoss1550SCA"] = jumper.Results.InsertionLoss1550SCA.ToString();
            postMessage.Body["InsertionLossLimit1550SCA"] = jumper.Limits.InsertionLossLimit1550SCA.ToString();
            postMessage.Body["ReturnLoss1550SCA"] = jumper.Results.ReturnLoss1550SCA.ToString();
            postMessage.Body["ReturnLossLimit1550SCA"] = jumper.Limits.ReturnLossLimit1550SCA.ToString();
            postMessage.Body["InsertionLoss1550UNC"] = jumper.Results.InsertionLoss1550UNC.ToString();
            postMessage.Body["InsertionLossLimit1550UNC"] = jumper.Limits.InsertionLossLimit1550UNC.ToString();
            postMessage.Body["ReturnLoss1550UNC"] = jumper.Results.ReturnLoss1550UNC.ToString();
            postMessage.Body["ReturnLossLimit1550UNC"] = jumper.Limits.ReturnLossLimit1550UNC.ToString();
            postMessage.Body["LengthInMeters"] = jumper.Results.LengthInMeters.ToString();
            postMessage.Body["LengthUpperLimit"] = jumper.Limits.LengthUpperLimitInMeters.ToString();
            postMessage.Body["LengthLowerLimit"] = jumper.Limits.LengthLowerLimitInMeters.ToString();
        }

        protected virtual void OnPTSMessageSending()
        {
            if (PTSMessageSending != null)
                PTSMessageSending(this, EventArgs.Empty);
        }

        protected virtual void OnPTSMessageReceived()
        {
            if (PTSMessageReceived != null)
                PTSMessageReceived(this, EventArgs.Empty);
        }
    }
}