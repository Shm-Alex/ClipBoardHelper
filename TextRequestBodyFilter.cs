using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class TextRequestBodyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attr = context.MethodInfo.GetCustomAttributes(typeof(ConsumesAttribute), false)
            .FirstOrDefault() as ConsumesAttribute;

        if (attr?.ContentTypes.Contains("text/plain") == true ||
            attr?.ContentTypes.Contains("text/html") == true)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["text/plain"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "html" // или "string"
                        },
                        Example = new OpenApiString("<!DOCTYPE html><html>...</html>")
                    },
                    ["text/html"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    }
                },
                Required = true
            };
        }
    }
}