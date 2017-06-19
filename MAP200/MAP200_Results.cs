using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MAP200
{
    public class MAP200_Results
    {
        public string insertionLoss { get; set; }
        public string returnLoss { get; set; }
        public string length { get; set; }
        public TestSetMessage message { get; set; }
        public string jsonResults { get; set; }

        public MAP200_Results()
        {
            message = new TestSetMessage();
        }

        public void buildJson()
        {
            message.Header.Workstation = "01";
            message.Header.TestSetModel = "MAP200";
            message.Header.TestSetName = "MAP200";
            message.Header.OperId = "MRA";
            message.Header.Command1 = "RE";
            message.Header.Command2 = "FI";
            //TODO verify these properties aren't null before trying to assign them
            message.Body["serialNumber"] = "222888777001";
            //message.Body["insertionLoss"] = insertionLoss.ToString();
            //message.Body["returnLoss"] = returnLoss.ToString();
            //message.Body["length"] = length.ToString();

            //-------DUMMY DATA--------------//
            message.Body["insertionLoss"] = ".15";
            message.Body["returnLoss"] = "60";
            message.Body["length"] = "9";
            //-------------------------------//

            jsonResults = JsonConvert.SerializeObject(message);
        }
    }
}