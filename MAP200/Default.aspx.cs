using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace MAP200
{
    public partial class Default : System.Web.UI.Page
    {
        public MAP200 map200 { get; set; }
        public PCT pct { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            map200 = new MAP200();
            pct = new PCT();
        }

        protected void statusBtn_Click(object sender, EventArgs e)
        {
            string status;

            status = map200.sendCommand("IDN?", requestResponse: true);
            logTextBox.Text = status;
        }

        protected void startPctBtn_Click(object sender, EventArgs e)
        {
            bool pctRunningOnMap200 = map200.hasPctRunning();
            if (!pctRunningOnMap200)
            {
                try
                {
                    map200.startPct();
                    writeToLog("PCT Started");
                }
                catch (TimeoutException)
                {
                    writeToLog("Timeout starting the PCT");
                }
            }
            else
            {
                writeToLog("PCT already running");
            }
        }

        protected void stopPctBtn_Click(object sender, EventArgs e)
        {
            bool pctRunningOnMap200 = map200.hasPctRunning();
            if (pctRunningOnMap200)
            {
                try
                {
                    map200.stopPct();
                    writeToLog("PCT Stopped");
                }
                catch (TimeoutException)
                {
                    writeToLog("Timeout stopping the PCT");
                }
            }
            else
            {
                writeToLog("PCT already stopped");
            }
        }

        protected void runBtn_Click(object sender, EventArgs e)
        {
            List<string> testResults = new List<string>();
            //string testResults;
            testResults = (List<string>)map200.pct.runTest();
            //testResults = map200.pct.runTest();
            populateLogWithResults(testResults);
        }


        protected void pctStatusBtn_Click(object sender, EventArgs e)
        {
            writeToLog(map200.verbosePctStatus);
        }

        private void populateLogWithResults(IEnumerable<string> results)
        {
            foreach(var result in results)
            {
                writeToLog(result);
            }
        }

        private void writeToLog(string str)
        {
            logTextBox.Text += str + Environment.NewLine;
        }
    }
}