﻿using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT;

public class ServerInfo
{
    [JsonPropertyName("response")] public Dictionary<string, string> Response { get; set; } = new Dictionary<string, string>();
}