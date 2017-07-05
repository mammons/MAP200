using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NLog;


namespace MAP200
{
    public partial class Default : System.Web.UI.Page
    {
        public MAP200 map200 { get; set; }
        public Jumper jumper { get; set; }

        Logger logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            map200 = new MAP200();
            jumper = new Jumper();

            //Event fired if the connection to the MAP200 fails
            map200.conman.MAP200ConnectionFailed += OnMAP200ConnectionFailed;

            if (!IsPostBack)
            {
                map200.GetSerialNumber();
            }
        }

        //This button returns the information from the MAP200 CMR or chassis. If anything is returned then we were able to make a connection
        protected void statusBtn_Click(object sender, EventArgs e)
        {
            CheckMAP200Status();
        }

        private void CheckMAP200Status()
        {
            if (map200.IsConnected())
            {
                writeToLog("MAP200 Connected");
            }
            else
            {
                writeToLog("MAP200 Not Connected");
            }
        }

        //This button sends the command to start the PCT on the MAP200 which is required to run a test
        protected void startPctBtn_Click(object sender, EventArgs e)
        {
            StartPCT();
        }

        private void StartPCT()
        {
            bool pctRunningOnMap200 = map200.hasPctRunning();
            if (!pctRunningOnMap200)
            {
                try
                {
                    map200.StartPct();
                    writeToLog("PCT Started");
                }
                catch (TimeoutException ex)
                {
                    writeToLog(ex.Message);
                }
            }
            else
            {
                writeToLog("PCT already running");
            }
        }

        //This button stops the PCT
        protected void stopPctBtn_Click(object sender, EventArgs e)
        {
            StopPCT();
        }

        private void StopPCT()
        {
            bool pctRunningOnMap200 = map200.hasPctRunning();
            if (pctRunningOnMap200)
            {
                try
                {
                    map200.StopPct();
                    writeToLog("PCT Stopped");
                }
                catch (TimeoutException ex)
                {
                    writeToLog(ex.Message);
                }
            }
            else
            {
                writeToLog("PCT already stopped");
            }
        }

        //This button runs the test on the PCT which should return the insertion loss, return loss, and length of the jumper
        protected void runBtn_Click(object sender, EventArgs e)
        {
            RunTest();
        }

        private void RunTest()
        {
            if (CheckIfTestingRequired())
            {
                if (map200.hasPctRunning())
                {
                    map200.pct.runTest(jumper);

                    PopulateFieldsOnPageWithResults(jumper.results);

                    jumper.finalResponseMessage = SendResultsToPTS(jumper, map200);

                    writeToLog(jumper.jsonResults);
                }
                else
                {
                    writeToLog("PCT needs to be started before you can run a test");
                }
            }
            else
            {
                writeToLog(string.Format("Testing not required for jumper with serial number: {0}", jumper.serialNumber));
            }
        }

        private TestSetMessage SendResultsToPTS(Jumper jumper, MAP200 testSet)
        {
            PTStransaction pts = new PTStransaction();
            return pts.SendUPFI(jumper, testSet);
        }

        private bool CheckIfTestingRequired()
        {
            return jumper.GetTestingRequired(map200);
        }

        //This button gets the status of the PCT running on the MAP200
        protected void pctStatusBtn_Click(object sender, EventArgs e)
        {
            writeToLog(map200.verbosePctStatus);
        }

        /// <summary>
        /// Sends the results to the big log box
        /// </summary>
        /// <param name="results"></param>
        private void populateLogWithResults(IEnumerable<string> results)
        {
            foreach(var result in results)
            {
                writeToLog(result);
            }
        }

        /// <summary>
        /// Fills in the actual fields of the form with the results
        /// </summary>
        /// <param name="results"></param>
        private void PopulateFieldsOnPageWithResults(MAP200_Results results)
        {
            insertionLossTextBox.Text = jumper.results.InsertionLoss1550SCA.ToString();
            returnLossTextBox.Text = jumper.results.ReturnLoss1550SCA.ToString();
            lengthTextBox.Text = jumper.results.LengthInMeters.ToString();
        }

        /// <summary>
        /// Adds text to the log box
        /// </summary>
        /// <param name="str"></param>
        private void writeToLog(string str)
        {
            logTextBox.Text += str + Environment.NewLine;
            logger.Info(str);
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
            writeToLog(args.response);
        }
    }
}