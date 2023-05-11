using BIOCAD.Test;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(x =>
{
    string fileName = "books.json";
    var jsonString = File.ReadAllText(fileName);
    return JsonConvert.DeserializeObject<List<Book>>(jsonString);
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/books",
    async (List<Book>books,
    [FromQuery] string? title,
    [FromQuery] string? genre,
    [FromQuery] string? author) => books
        .Where(x => string.IsNullOrEmpty(title) || x.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase))
        .Where(x => string.IsNullOrEmpty(genre) || x.Category.Contains(genre, StringComparison.CurrentCultureIgnoreCase))
        .Where(x => string.IsNullOrEmpty(author) || x.Authors.Any(a => a.Name.Contains(author, StringComparison.CurrentCultureIgnoreCase))));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();  

