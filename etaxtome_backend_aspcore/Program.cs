using Microsoft.OpenApi.Models;
using DotNetEnv;
using MyFirestoreApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use HTTPS on port 5003
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003); //
});

// Load environment variables from .env file
Env.Load();

// Add services to the container
// อย่าลืมมาเพิ่ม AppScoped Service เสมอ
builder.Services.AddControllers();
builder.Services.AddScoped<CorpService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<FireStoreService>();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
            new Uri(origin).Host == "localhost") // Allow any port on localhost
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Define a security scheme for the API key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "API Key needed to access the endpoints",
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey
    });

    // Apply the security scheme globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // สำหรับ SchemaFilter ใช้สำหรับ [FormBody] เท่านั้น 
    c.SchemaFilter<DocumentRequestSchemaFilter>();
});

var app = builder.Build();

/*
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/

app.UseCors("AllowLocalhost");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
