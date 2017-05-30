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
            message.Header.Command1 = "UP";
            message.Header.Command2 = "FI";
            message.Body["insertionLoss"] = insertionLoss.ToString();
            message.Body["returnLoss"] = returnLoss.ToString();
            message.Body["length"] = length.ToString();

            jsonResults = JsonConvert.SerializeObject(message);
        }
    }
}