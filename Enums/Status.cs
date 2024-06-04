using System.Text.Json.Serialization;

namespace Key_Management_System.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Pending,
        Accept,
        Decline,
        CheckReturn,//
        ThirdParty,
        AcceptSignOut,
        DeclineSignOut
    }
}
