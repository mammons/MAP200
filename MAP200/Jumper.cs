using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MAP200
{
    public class Jumper
    {
        public string SerialNumber { get; set; } 
        public bool LossTestRequired { get; set; }
        public string Commcode { get; set; }
        public MAP200_Limits Limits { get; set; }
        public MAP200_Results Results { get; set; }
        public TestSetMessage PostMessage { get; set; }
        public TestSetMessage FinalResponseMessage { get; set; }
        private string _jsonResults;
        public string jsonResults
        {
            get
            {
                return JsonConvert.SerializeObject(FinalResponseMessage, Formatting.Indented);
            }
            set
            {
                _jsonResults = value;
            }
        }

        public Jumper()
        {
            Limits = new MAP200_Limits();
            Results = new MAP200_Results();
        }
        
        public bool GetTestingRequired(MAP200 testSet)
        {
            if(LossTestRequired){
                return true;
            }
            else{
                PTStransaction pts = new PTStransaction();
                LossTestRequired = pts.GetTestingRequired(this, testSet);
            }

            return LossTestRequired;
        }

        public void AssignLimits(TestSetMessage response)
        {
            Limits.InsertionLossLimit1550SCA = response.Body["InsertionLossLimit1550SCA"];
            Limits.ReturnLossLimit1550SCA = response.Body["ReturnLossLimit1550SCA"];
            Limits.InsertionLossLimit1550UNC = response.Body["InsertionLossLimit1550UNC"];
            Limits.ReturnLossLimit1550UNC = response.Body["ReturnLossLimit1550UNC"];
            Limits.LengthUpperLimitInMeters = response.Body["LengthUpperLimitInMeters"];
            Limits.LengthLowerLimitInMeters = response.Body["LengthLowerLimitInMeters"];
        }


    }
}