using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CineSocial.Api.Extensions;

public class SwaggerExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
        {
            schema.Example = new OpenApiString("2023-12-25T10:30:00Z");
        }
        else if (context.Type == typeof(Guid) || context.Type == typeof(Guid?))
        {
            schema.Example = new OpenApiString("550e8400-e29b-41d4-a716-446655440000");
        }
        else if (context.Type == typeof(decimal) || context.Type == typeof(decimal?))
        {
            schema.Example = new OpenApiDouble(8.5);
        }
        else if (context.Type == typeof(string))
        {
            var propertyName = context.MemberInfo?.Name?.ToLower();
            
            if (propertyName?.Contains("email") == true)
            {
                schema.Example = new OpenApiString("user@example.com");
            }
            else if (propertyName?.Contains("password") == true)
            {
                schema.Example = new OpenApiString("SecurePassword123!");
            }
            else if (propertyName?.Contains("username") == true)
            {
                schema.Example = new OpenApiString("cinemafan01");
            }
            else if (propertyName?.Contains("token") == true)
            {
                schema.Example = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...");
            }
            else if (propertyName?.Contains("title") == true)
            {
                schema.Example = new OpenApiString("Harika bir film!");
            }
            else if (propertyName?.Contains("content") == true)
            {
                schema.Example = new OpenApiString("Bu film gerçekten muhteşemdi. Özellikle görsel efektler ve oyunculuklar çok etkileyiciydi.");
            }
            else if (propertyName?.Contains("name") == true)
            {
                schema.Example = new OpenApiString("Leonardo DiCaprio");
            }
            else if (propertyName?.Contains("bio") == true)
            {
                schema.Example = new OpenApiString("Film tutkunu ve eleştirmeni. Özellikle bilim kurgu filmleri seviyorum.");
            }
        }
        else if (context.Type.IsEnum)
        {
            var enumValues = Enum.GetValues(context.Type);
            if (enumValues.Length > 0)
            {
                schema.Example = new OpenApiString(enumValues.GetValue(0)?.ToString());
            }
        }

        // Handle validation attributes for better examples
        if (context.MemberInfo != null)
        {
            var rangeAttribute = context.MemberInfo.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null && context.Type == typeof(int))
            {
                var midValue = ((int)rangeAttribute.Minimum + (int)rangeAttribute.Maximum) / 2;
                schema.Example = new OpenApiInteger(midValue);
            }
        }
    }
}