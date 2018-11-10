using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoLatex.Authentication
{
    public class RegisterResponseModel
    {
        public bool Success { get; set; }
        public ErrorReason Error { get; set; }

        [JsonConverter(typeof(StringEnumConverter), true)]
        public enum ErrorReason
        {
            None,
            InternalError,
            UsernameInUse,
            EmailInUse,
            InvalidUsername,
            InvalidPassword,
            InvalidName,
            InvalidEmail
        }
    }
}