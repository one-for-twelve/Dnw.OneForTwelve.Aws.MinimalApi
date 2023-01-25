using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Shared;

namespace Api;

internal static class ResponseWriter
{
    public static async Task WriteResponse<TResponseType>(this HttpContext context, HttpStatusCode statusCode, TResponseType body) where TResponseType : class
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, typeof(TResponseType), ApiSerializerContext.Default));
    }
}