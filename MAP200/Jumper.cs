using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MAP200
{
    public class Jumper
    {
        public string serialNumber { get; set; } = "222888777001";
        public bool lossesRequired { get; set; }
        public string Commcode { get; set; }
        public MAP200_Limits limits { get; set; }
        public MAP200_Results results { get; set; }
        public TestSetMessage postMessage { get; set; }
        public TestSetMessage responseMessage { get; set; }
        public string jsonResults { get; set; }

        public Jumper()
        {
            limits = new MAP200_Limits();
            results = new MAP200_Results();
        }
        
        public bool GetTestNeeded()
        {
            return true;
        }


    }
}