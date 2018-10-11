using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IntelligentKioskSample
{
    class WebhookHelper
    {
        private static HttpClient httpClient = new HttpClient();

        public async static void CallWebhookEndpoint(Dictionary<String, String> payload)
        {
            DateTime start = DateTime.Now;
            Uri endpoint = new Uri(SettingsHelper.Instance.WebhookEndpointURI);
            StringContent content = new StringContent("{\"results\": [\"" + payload["personname"] + "\"]}", System.Text.Encoding.UTF8, "application/json");
            String request = await content.ReadAsStringAsync();
            var response = await httpClient.PostAsync(endpoint, content);

            string responseData = await response.Content.ReadAsStringAsync();
            if (((int)response.StatusCode) == 200)
            {
                System.Diagnostics.Debug.WriteLine("[SUCCESS] HTTP 200 OK");
                System.Diagnostics.Debug.WriteLine("[VERBOST] RESPONSE:" + responseData);
                ApplicationInsightsHelper.TrackRequest("Webhook", endpoint, ((int)response.StatusCode).ToString(), true, DateTime.Now - start);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("[ERROR] HTTP {0}", response.StatusCode));
                ApplicationInsightsHelper.TrackRequest("Webhook", endpoint, ((int)response.StatusCode).ToString(), false, DateTime.Now - start);
            }
            System.Diagnostics.Debug.WriteLine(request);

        }
    }
}
