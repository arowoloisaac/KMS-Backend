using System.Text.Json.Serialization;

namespace Key_Management_System.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CheckWith
    {
        InHand,
        InBoard,
        ThirdParty
    }
}
