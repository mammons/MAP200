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
        public TestSetMessage postMessage { get; set; }
        public TestSetMessage responseMessage { get; set; }



        public delegate void PTSMessageSendingEventHandler(object source, EventArgs args);
        public event PTSMessageSendingEventHandler PTSMessageSending;

        public delegate void PTSMessageReceivedEventHandler(object source, EventArgs args);
        public event PTSMessageReceivedEventHandler PTSMessageReceived;

        private string developmentUri = System.Configuration.ConfigurationManager.AppSettings["devURI"];
        private string productionUri = System.Configuration.ConfigurationManager.AppSettings["prodURI"];
        private string serviceUrl = "http://localhost:53962/api/PTSWebServices";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private HttpResponseMessage response;

        public PTStransaction()
        {
            serverUri = System.Configuration.ConfigurationManager.AppSettings["Server"] == "DEV" ? developmentUri : productionUri;
        }


        public bool PostToPTS(TestSetMessage json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverUri);

                OnPTSMessageSending();
                response = client.PostAsJsonAsync<TestSetMessage>(serviceUrl, json).Result;
                OnPTSMessageReceived();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        responseMessage = response.Content.ReadAsAsync<TestSetMessage>().Result;
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.Debug("Error in Post: {0}", e.Message);
                        return false;
                    }
                }
                return false;
            }
        }

        //TODO Change MAP200 to generic TestSet class with these basic properties so you can change the MAP200 type to a TestSet type for future sets
        public void SendResult(Jumper jumper, MAP200 testSet)
        {
            BuildJson(jumper, testSet);
            postMessage.Header.Command1 = "UP";
            postMessage.Header.Command2 = "FI";
            jumper.postMessage = this.postMessage;
            PostToPTS(jumper.postMessage);
        }

        public void CheckLossNeeded(Jumper jumper, MAP200 testSet)
        {
            BuildJson(jumper, testSet);
            postMessage.Header.Command1 = "RE";
            postMessage.Header.Command2 = "FI";
            jumper.postMessage = this.postMessage;
            PostToPTS(jumper.postMessage);
        }

        private void BuildJson(Jumper jumper, MAP200 testSet)
        {
            postMessage = new TestSetMessage();

            postMessage.Header.TestSetModel = testSet.Model;
            postMessage.Header.TestSetName = testSet.SerialNumber;
            postMessage.Header.Workstation = testSet.Workstation;
            postMessage.Header.Location = testSet.Location;
            postMessage.Header.OperId = testSet.OperId;
            postMessage.Body["serialNumber"] = jumper.serialNumber;
            postMessage.Body["InsertionLoss1550SCA"] = jumper.results.InsertionLoss1550SCA.ToString();
            postMessage.Body["InsertionLossLimit"] = jumper.limits.InsertionLossLimit1550SCA.ToString();
            postMessage.Body["ReturnLoss1550SCA"] = jumper.results.ReturnLoss1550SCA.ToString();
            postMessage.Body["ReturnLossLimit"] = jumper.limits.ReturnLossLimit1550SCA.ToString();
            postMessage.Body["LengthInMeters"] = jumper.results.LengthInMeters.ToString();
            postMessage.Body["LengthUpperLimit"] = jumper.limits.LengthUpperLimitInMeters.ToString();
            postMessage.Body["LengthLowerLimit"] = jumper.limits.LengthLowerLimitInMeters.ToString();
        }

        protected virtual void OnPTSMessageSending()
        {
            logger.Info(this.postMessage);
            if (PTSMessageSending != null)
                PTSMessageSending(this, EventArgs.Empty);
        }

        protected virtual void OnPTSMessageReceived()
        {
            logger.Info(this.responseMessage);
            if (PTSMessageReceived != null)
                PTSMessageReceived(this, EventArgs.Empty);
        }
    }
}