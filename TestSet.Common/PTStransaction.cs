using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace TestSetLib
{
    public class PTStransaction
    {
        public string ServerUri { get; set; }
        public string ServiceUrl { get; set; }

        public delegate void PTSMessageSendingEventHandler(object source, EventArgs args);
        public event PTSMessageSendingEventHandler PTSMessageSending;

        public delegate void PTSMessageReceivedEventHandler(object source, EventArgs args);
        public event PTSMessageReceivedEventHandler PTSMessageReceived;

        public TestSetMessage PostToPTS(TestSetMessage json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ServerUri);

                OnPTSMessageSending();
                HttpResponseMessage httpResponse = client.PostAsJsonAsync<TestSetMessage>(ServiceUrl, json).Result;
                OnPTSMessageReceived();

                TestSetMessage responseMessage = new TestSetMessage();
                if (httpResponse.IsSuccessStatusCode)
                {
                    try
                    {
                        responseMessage = httpResponse.Content.ReadAsAsync<TestSetMessage>().Result;
                        responseMessage.Success = true;
                    }
                    catch (Exception e)
                    {
                        responseMessage = new TestSetMessage();
                        responseMessage.Success = false;
                        responseMessage.Response.Message = e.Message;
                    }
                }
                else
                {
                    responseMessage.Response.Message = "Communication with PTS failed";
                    //logger.Debug("Error: {0} ", responseMessage.Response.Message);
                }
                return responseMessage;
            }
        }

        public TestSetMessage SendUPFI(TestSetMessage postMessage)
        {
            postMessage.Header.Command1 = "UP";
            postMessage.Header.Command2 = "FI";

            return PostToPTS(postMessage);
        }

        public TestSetMessage SendREFI(TestSetMessage postMessage)
        {
            postMessage.Header.Command1 = "RE";
            postMessage.Header.Command2 = "FI";

            return PostToPTS(postMessage);
        }

        protected virtual void OnPTSMessageSending()
        {
            if (PTSMessageSending != null)
                PTSMessageSending(this, EventArgs.Empty);
        }

        protected virtual void OnPTSMessageReceived()
        {
            if (PTSMessageReceived != null)
                PTSMessageReceived(this, EventArgs.Empty);
        }
    }
}
