using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PatientsAPI.Enums
{
    /// <summary>
    /// Enum with possible activity status.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Active
    {
        True,
        False        
    }
}
