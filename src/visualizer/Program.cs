var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("https://localhost:8081");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/showtranscription", async (HttpContext context) =>
{
    using StreamReader streamReader = new StreamReader(context.Request.Body);
    string translation = await streamReader.ReadToEndAsync();
    Console.Clear(); 
    
    Console.WriteLine(translation);

})
.WithName("ShowTranscription")
.WithOpenApi();

app.Run();
