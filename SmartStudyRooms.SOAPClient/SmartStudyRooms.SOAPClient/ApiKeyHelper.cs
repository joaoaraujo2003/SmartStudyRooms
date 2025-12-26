using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SmartStudyRooms.SOAPClient
{
    public static class ApiKeyHelper
    {
        public static void AddApiKey(string apiKey)
        {
            var request = new HttpRequestMessageProperty();
            request.Headers["X-API-KEY"] = apiKey;

            OperationContext.Current.OutgoingMessageProperties[
                HttpRequestMessageProperty.Name] = request;
        }
    }
}
