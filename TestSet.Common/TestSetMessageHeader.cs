﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestSetLib
{
    public class TestSetMessageHeader
    {
        public string TestSetModel { get; set; }
        public string TestSetName { get; set; }
        public string Location { get; set; }
        public string OperId { get; set; }
        public string Command1 { get; set; }
        public string Command2 { get; set; }
    }
}