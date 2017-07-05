using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using TestSetLib;
using NLog;

namespace MAP200
{
    public class Jumper : Fiber
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

        Logger logger = LogManager.GetCurrentClassLogger();

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
                MAP200_PTStransaction pts = new MAP200_PTStransaction();
                try
                {
                    LossTestRequired = pts.GetTestingRequired(this, testSet);
                }
                catch(Exception ex)
                {
                    logger.Debug(ex.Message);
                }                
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