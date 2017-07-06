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
                TestSetMessage responseMessage = new TestSetMessage();
                HttpResponseMessage httpResponse;
                client.BaseAddress = new Uri(ServerUri);

                OnPTSMessageSending();
                try
                {
                    httpResponse = client.PostAsJsonAsync(ServiceUrl, json).Result;
                    OnPTSMessageReceived();
                }
                catch (Exception ex)
                {
                    responseMessage.Success = false;
                    responseMessage.Response.Message = (string.Format("Could not contact the PTS service. ServiceUrl: {0} , ServerUri: {1}, Error: {2}", ServiceUrl, ServerUri, ex));
                    return responseMessage;
                }                

                
                if (httpResponse.IsSuccessStatusCode)
                {
                    try
                    {
                        responseMessage = httpResponse.Content.ReadAsAsync<TestSetMessage>().Result;
                        responseMessage.Success = true;
                    }
                    catch (Exception ex)
                    {
                        responseMessage = new TestSetMessage();
                        responseMessage.Success = false;
                        responseMessage.Response.Message = string.Format("PTS service error ServiceUrl: { 0} , ServerUri: { 1}, Error: { 2}", ServiceUrl, ServerUri, ex);
                    }
                }
                else
                {
                    responseMessage.Response.Message = string.Format("Something went wrong communicating with PTS. Response: {0}", httpResponse);
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
