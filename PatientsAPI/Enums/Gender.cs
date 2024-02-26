using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PatientsAPI.Enums
{
    /// <summary>
    /// Enum with possible genders.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        Unknown,
        Male,
        Female,
        Other
    }
}
