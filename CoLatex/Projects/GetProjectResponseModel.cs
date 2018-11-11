﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoLatex.Projects
{
    public class GetProjectResponseModel
    {
        public bool Success { get; set; }
        public ProjectResponseModel Project { get; set; }
        public ErrorReason Error { get; set; }

        [JsonConverter(typeof(StringEnumConverter), true)]
        public enum ErrorReason
        {
            None,
            InternalError,
            Unauthorised
        }
    }
}