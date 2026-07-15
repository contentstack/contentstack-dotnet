using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Parses JSON error bodies from the Contentstack API (error_code, error_message, errors).
    /// </summary>
    internal static class ApiErrorBodyParser
    {
        internal static void TryApply(
            string body,
            ref int errorCode,
            ref string errorMessage,
            ref Dictionary<string, object> errors)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return;
            }

            try
            {
                var node = JsonNode.Parse(body.Replace("\r\n", ""));
                if (node is not JsonObject data)
                {
                    return;
                }

                if (data["error_code"] is JsonValue ec)
                {
                    try
                    {
                        errorCode = ec.GetValue<int>();
                    }
                    catch
                    {
                        if (int.TryParse(ec.ToString(), out var p))
                        {
                            errorCode = p;
                        }
                    }
                }

                if (data["error_message"] is JsonValue em)
                {
                    var s = em.GetValue<string>();
                    if (s != null)
                    {
                        errorMessage = s;
                    }
                }

                if (data["errors"] is JsonObject errObj)
                {
                    errors = JsonNodeConversion.JsonObjectToDictionary(errObj);
                }
                else if (data["errors"] is JsonArray)
                {
                    errors = new Dictionary<string, object>
                    {
                        { "errors", JsonNodeConversion.JsonNodeToClr(data["errors"]) }
                    };
                }
            }
            catch (JsonException)
            {
            }
        }
    }
}
