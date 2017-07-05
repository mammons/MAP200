using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NLog;
using System.Text.RegularExpressions;

namespace MAP200
{
    public partial class Default : System.Web.UI.Page
    {
        public MAP200 Map200 { get; set; }
        public Jumper Jumper { get; set; }

        Logger logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            Map200 = new MAP200();
            Jumper = new Jumper();

            //Event fired if the connection to the MAP200 fails
            Map200.conman.MAP200ConnectionFailed += OnMAP200ConnectionFailed;

            //if (!IsPostBack)
            //{
            //    Map200.GetSetInfo();
            //}
        }

        //This button returns the information from the MAP200 CMR or chassis. If anything is returned then we were able to make a connection
        protected void statusBtn_Click(object sender, EventArgs e)
        {
            CheckMAP200Status();
        }

        private void CheckMAP200Status()
        {
            if (Map200.IsConnected())
            {
                WriteToLog("MAP200 Connected");
            }
            else
            {
                WriteToLog("MAP200 Not Connected");
            }
        }

        //This button sends the command to start the PCT on the MAP200 which is required to run a test
        protected void startPctBtn_Click(object sender, EventArgs e)
        {
            StartPCT();
        }

        private void StartPCT()
        {
            bool pctRunningOnMap200 = Map200.HasPctRunning();
            if (!pctRunningOnMap200)
            {
                try
                {
                    Map200.StartPct();
                    WriteToLog("PCT Started");
                }
                catch (TimeoutException ex)
                {
                    WriteToLog(ex.Message);
                }
            }
            else
            {
                WriteToLog("PCT already running");
            }
        }

        //This button stops the PCT
        protected void stopPctBtn_Click(object sender, EventArgs e)
        {
            StopPCT();
        }

        private void StopPCT()
        {
            bool pctRunningOnMap200 = Map200.HasPctRunning();
            if (pctRunningOnMap200)
            {
                try
                {
                    Map200.StopPct();
                    WriteToLog("PCT Stopped");
                }
                catch (TimeoutException ex)
                {
                    WriteToLog(ex.Message);
                }
            }
            else
            {
                WriteToLog("PCT already stopped");
            }
        }

        //This button runs the test on the PCT which should return the insertion loss, return loss, and length of the jumper
        protected void runBtn_Click(object sender, EventArgs e)
        {
            RunTest();
        }

        private void RunTest()
        {
            AssignSerialIdToJumper();

            if (CheckIfTestingRequired())
            {
                if (Map200.HasPctRunning())
                {
                    var testOp = new OperationResult();
                    testOp = Map200.pct.RunTest(Jumper);

                    PopulateFieldsOnPageWithResults(Jumper.Results);

                    Jumper.FinalResponseMessage = SendResultsToPTS(Jumper, Map200);

                    WriteToLog(Jumper.jsonResults);
                }
                else
                {
                    WriteToLog("PCT needs to be started before you can run a test");
                }
            }
            else
            {
                WriteToLog(string.Format("Testing not required for jumper with serial number: {0}", Jumper.SerialNumber));
            }
        }

        private void AssignSerialIdToJumper()
        {
            var op = ValidateSerialNumber();

            if (op.Success)
            {
                Jumper.SerialNumber = serialNumberTextBox.Text;
            }
            else
            {
                WriteToLog(op.ErrorMessages);
            }
        }

        private OperationResult ValidateSerialNumber()
        {
            var enteredSerialNumber = serialNumberTextBox.Text;
            var op = new OperationResult();
            op.Success = true;

            if(enteredSerialNumber.Length == 0)
            {
                op.Success = false;
                op.ErrorMessages.Add("Serial ID length cannot be zero");
                return op;
            }

            Regex onlyNumbers = new Regex(@"^[0-9]+$");
            if (onlyNumbers.IsMatch(enteredSerialNumber))
            {
                return op;
            }else
            {
                op.Success = false;
                op.ErrorMessages.Add("Serial number must only contain numbers");
            }

            return op;
        }

        private TestSetMessage SendResultsToPTS(Jumper Jumper, MAP200 testSet)
        {
            PTStransaction pts = new PTStransaction();
            return pts.SendUPFI(Jumper, testSet);
        }

        private bool CheckIfTestingRequired()
        {
            return Jumper.GetTestingRequired(Map200);
        }

        //This button gets the status of the PCT running on the MAP200
        protected void pctStatusBtn_Click(object sender, EventArgs e)
        {
            WriteToLog(Map200.verbosePctStatus);
        }

        /// <summary>
        /// Sends the results to the big log box
        /// </summary>
        /// <param name="results"></param>
        private void populateLogWithResults(IEnumerable<string> results)
        {
            foreach(var result in results)
            {
                WriteToLog(result);
            }
        }

        /// <summary>
        /// Fills in the actual fields of the form with the results
        /// </summary>
        /// <param name="results"></param>
        private void PopulateFieldsOnPageWithResults(MAP200_Results results)
        {
            insertionLossTextBox.Text = Jumper.Results.InsertionLoss1550SCA.ToString();
            returnLossTextBox.Text = Jumper.Results.ReturnLoss1550SCA.ToString();
            lengthTextBox.Text = Jumper.Results.LengthInMeters.ToString();
        }

        /// <summary>
        /// Adds text to the log box
        /// </summary>
        /// <param name="str"></param>
        private void WriteToLog(string str)
        {
            logTextBox.Text += str + Environment.NewLine;
            logger.Info(str);
        }

        private void WriteToLog(List<string> list)
        {
            foreach(var msg in list)
            {
                WriteToLog(msg);
            }
        }

        /// <summary>
        /// Clears the form text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearBtn_Click(object sender, EventArgs e)
        {
            insertionLossTextBox.Text = "";
            returnLossTextBox.Text = "";
            lengthTextBox.Text = "";
        }

        public void OnMAP200ConnectionFailed(object source, MAP200MessageEventArgs args)
        {
            WriteToLog(args.Connection.ErrorMessages);
        }
    }
}