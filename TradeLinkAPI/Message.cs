
namespace TradeLink.API
{
    public struct GenericMessage
    {
        public MessageTypes Type;
        public uint Source;
        public uint Dest;
        public string Request;
        public string Response;
        public uint ID;
        public GenericMessage(MessageTypes type, string request)
        {
            Type = type;
            Request = request;
            Source = 0;
            Dest = 0;
            Response = string.Empty;
            ID = 0;
        }
        public GenericMessage(MessageTypes type, uint source, uint dest, uint msgid, string request, string response)
        {
            Type = type;
            Source = source;
            Dest = dest;
            Request = request;
            Response = response;
            ID = msgid;
        }
    }
}