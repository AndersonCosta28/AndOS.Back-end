using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace AndOS.Infrastructure.Database.Converters;

public class JsonDocumentConverter : ValueConverter<JsonDocument, string>
{
    public JsonDocumentConverter() : base(
        v => convertJsonDocumentToString(v),
        v => convertStringToJsonDocument(v))
    {
    }

    private static string convertJsonDocumentToString(JsonDocument doc) => doc.RootElement.GetRawText();

    private static JsonDocument convertStringToJsonDocument(string json) => JsonDocument.Parse(json);
}