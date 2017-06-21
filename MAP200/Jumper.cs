using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MAP200
{
    public class Jumper
    {
        public string serialNumber { get; set; } = "222888777001";
        public bool lossTestRequired { get; set; }
        public string Commcode { get; set; }
        public MAP200_Limits limits { get; set; }
        public MAP200_Results results { get; set; }
        public TestSetMessage postMessage { get; set; }
        public TestSetMessage finalResponseMessage { get; set; }
        private string _jsonResults;
        public string jsonResults
        {
            get
            {
                return JsonConvert.SerializeObject(finalResponseMessage, Formatting.Indented);
            }
            set
            {
                _jsonResults = value;
            }
        }

        public Jumper()
        {
            limits = new MAP200_Limits();
            results = new MAP200_Results();
        }
        
        public bool GetTestingRequired(MAP200 testSet)
        {
            if(lossTestRequired){
                return true;
            }
            else{
                PTStransaction pts = new PTStransaction();
                lossTestRequired = pts.GetTestingRequired(this, testSet);
            }

            return lossTestRequired;
        }

        public void AssignLimits(TestSetMessage response)
        {
            limits.InsertionLossLimit1550SCA = response.Body["InsertionLossLimit1550SCA"];
            limits.ReturnLossLimit1550SCA = response.Body["ReturnLossLimit1550SCA"];
            limits.InsertionLossLimit1550UNC = response.Body["InsertionLossLimit1550UNC"];
            limits.ReturnLossLimit1550UNC = response.Body["ReturnLossLimit1550UNC"];
            limits.LengthUpperLimitInMeters = response.Body["LengthUpperLimitInMeters"];
            limits.LengthLowerLimitInMeters = response.Body["LengthLowerLimitInMeters"];
        }


    }
}