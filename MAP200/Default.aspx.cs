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
        protected void Page_Load(object sender, EventArgs e)
        {
            map200 = new MAP200();
        }

        protected void statusBtn_Click(object sender, EventArgs e)
        {
            string status;

            if (map200.isConnected)
            {
                status = map200.sendCommand("IDN?", requestResponse: true);
            }
            else
            {
                status = map200.openConnection();
            }
            logTextBox.Text = status;
        }
    }
}