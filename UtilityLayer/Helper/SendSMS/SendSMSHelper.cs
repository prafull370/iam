using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer.Helper.SendSMS
{
    public class SMSNotificationModel
    {
        public string? MOBILE { get; set; }
        public string? MESSAGE { get; set; }
        public string? LANGUAGE { get; set; }
        public string? TEMPLATE_ID { get; set; }
    }
    public static class SendSMSHelper
    {
        #region Send Message
        public static async Task<bool> SendMessage(SMSNotificationModel model)
        {
            try
            {
                using var client = new HttpClient();

                string message = Uri.EscapeDataString(model.MESSAGE);

                string url = $"http://net.smshubsolutions.com/api/pushsms?" +
                             $"user=211064&authkey=92lTeYJwyPt2U&sender=ISSPLN" +
                             $"&mobile={model.MOBILE}" +
                             $"&text={message}" +
                             $"&entityid=1001490989074107334" +
                             $"&templateid={model.TEMPLATE_ID}&rpt=1";

                var response = await client.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);

                if (json?.STATUS == "OK" && json?.RESPONSE?.CODE == "100")
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // log error
                return false;
            }
        }
        #endregion
    }
}
