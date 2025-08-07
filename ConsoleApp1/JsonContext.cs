using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Payload))]
[JsonSerializable(typeof(System.Text.Json.JsonElement))]
internal partial class AppJsonContext : JsonSerializerContext { }