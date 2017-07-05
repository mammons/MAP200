using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using Newtonsoft.Json;
using System.Net.Http;
using TestSetLib;

namespace MAP200
{
    class MAP200_PTStransaction : PTStransaction
    {

        //public TestSetMessage postMessage { get; set; }    


        private string _developmentUri = System.Configuration.ConfigurationManager.AppSettings["devURI"];
        private string _productionUri = System.Configuration.ConfigurationManager.AppSettings["prodURI"];
        private string _serviceUrl = "http://localhost:53962/api/PTSWebServices";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        

        public MAP200_PTStransaction()
        {
            ServerUri = System.Configuration.ConfigurationManager.AppSettings["Server"] == "DEV" ? _developmentUri : _productionUri;
            ServiceUrl = _serviceUrl;
        }

        
        public bool GetTestingRequired(Jumper jumper, MAP200 testSet)
        {
            var json = BuildJson(jumper, testSet);
            var response = SendREFI(json);

            if (response.Success)
            {
                bool testingRequired = response.Body["required"].Equals("yes");

                if (testingRequired) jumper.AssignLimits(response);

                return testingRequired;
            }
            return false;
        }


        public TestSetMessage BuildJson(Jumper jumper, MAP200 testSet)
        {
            var postMessage = new TestSetMessage();

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

            return postMessage;
        }


    }
}