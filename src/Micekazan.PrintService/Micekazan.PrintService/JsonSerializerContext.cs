using System.Text.Json;
using System.Text.Json.Serialization;
using Micekazan.PrintDispatcher.Domain.Contracts;

namespace Micekazan.PrintService;

[JsonSerializable(typeof(Acknowledgement))]
[JsonSerializable(typeof(Document))]
[JsonSerializable(typeof(Update))]
[JsonSerializable(typeof(UpdateKind))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
internal partial class GlobalJsonSerializerContext : JsonSerializerContext;

[JsonSerializable(typeof(MicekazanConfiguration))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class MicekazanConfigurationJsonSerializerContext : JsonSerializerContext;