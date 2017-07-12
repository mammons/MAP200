using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NLog;
using System.Text.RegularExpressions;
using TestSetLib;

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
        }

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
           var start = StartPCT();
            if (start.Success)
            {
                WriteToLog(start.Messages);
            }
            else
            {
                WriteToLog(start.ErrorMessages);
            }
        }

        private OperationResult StartPCT()
        {
            bool pctRunningOnMap200 = Map200.HasPctRunning();
            var op = new OperationResult();
            if (!pctRunningOnMap200)
            {
                try
                {
                    Map200.StartPct();
                    op.Success = true;
                    op.Messages.Add("PCT Started");
                }
                catch (TimeoutException ex)
                {
                    op.Success = false;
                    op.ErrorMessages.Add(ex.Message);
                    logger.Debug(op.ErrorMessages);
                }
            }
            else
            {
                op.Success = true;
                op.Messages.Add("PCT already running");
            }
            return op;
        }

        //This button stops the PCT
        protected void stopPctBtn_Click(object sender, EventArgs e)
        {
            var stop = StopPCT();
            if (stop.Success)
            {
                WriteToLog(stop.Messages);
            }
            else
            {
                WriteToLog(stop.ErrorMessages);
            }
        }

        private OperationResult StopPCT()
        {
            bool pctRunningOnMap200 = Map200.HasPctRunning();
            var op = new OperationResult();
            if (pctRunningOnMap200)
            {
                try
                {
                    Map200.StopPct();
                    op.Success = true;
                    op.Messages.Add("PCT Stopped");
                }
                catch (TimeoutException ex)
                {
                    op.Success = false;
                    op.ErrorMessages.Add(ex.Message);
                    logger.Debug(op.ErrorMessages);
                }
            }
            else
            {
                op.Success = true;
                op.Messages.Add("PCT already stopped");
            }
            return op;
        }

        //This button runs the test on the PCT which should return the insertion loss, return loss, and length of the jumper
        protected void runBtn_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            button.Text = "Running test";

            var testOp = RunTest();

            if (testOp.Success)
            {
                PopulateFieldsOnPageWithResults(Jumper.Results);

                Jumper.FinalResponseMessage = SendResultsToPTS(Jumper, Map200);
                WriteToLog(Jumper.jsonResults);
            }
            else
            {
                WriteToLog(testOp.ErrorMessages);
            }

            button.Text = "Run";
        }

        private OperationResult RunTest()
        {
            var testOp = new OperationResult();
            var assignOp = AssignSerialIdToJumper();
            var testRequiredOp = CheckIfTestingRequired();
            if (!assignOp.Success)
            {
                WriteToLog(assignOp.ErrorMessages);
                testOp.Success = false;
                testOp.ErrorMessages.AddRange(assignOp.ErrorMessages);
                return testOp;
            }

            var testingRequired = testRequiredOp.Messages.Any();
            if (testingRequired)
            {
                if (Map200.HasPctRunning())
                {
                    testOp = Map200.pct.RunTest(Jumper);
                    testOp.Messages.Add(string.Format("Jumper {0} completed the test", Jumper.SerialNumber));
                }
                else
                {
                    string msg = "PCT needs to be started before you can run a test";
                    testOp.Success = false;
                    testOp.ErrorMessages.Add(msg);
                }
            }
            else
            {
                string msg = testRequiredOp.Success ? string.Format("Testing not required for jumper with serial number: {0}", Jumper.SerialNumber) : "No response from PTS";
                testOp.Success = false;
                testOp.ErrorMessages.Add(msg);
            }

            return testOp;
        }

        private OperationResult AssignSerialIdToJumper()
        {
            var op = ValidateSerialNumber(serialNumberTextBox.Text);

            if (op.Success)
            {
                Jumper.SerialNumber = serialNumberTextBox.Text;
            }
            else
            {
                WriteToLog(op.ErrorMessages);
            }
            return op;
        }

        public OperationResult ValidateSerialNumber(string serialNumber)
        {
            var enteredSerialNumber = serialNumber;
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

        private TestSetMessage SendResultsToPTS(Jumper jumper, MAP200 testSet)
        {
            MAP200_PTStransaction pts = new MAP200_PTStransaction();
            var json = pts.BuildJson(jumper, testSet);
            return pts.SendUPFI(json);
        }

        private OperationResult CheckIfTestingRequired()
        {
            var testingRequired = Jumper.GetTestingRequired(Map200);
            return testingRequired;
        }

        //This button gets the status of the PCT running on the MAP200
        protected void pctStatusBtn_Click(object sender, EventArgs e)
        {
            WriteToLog(Map200.VerbosePctStatus);
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