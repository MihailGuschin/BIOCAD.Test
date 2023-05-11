using Microsoft.Data.Sqlite;
using Dapper;
using BIOCAD.Test;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "DataSource=BIOCAD.Test.DB;mode=memory;cache=shared";
builder.Services.AddSingleton(_ => new SqliteConnection(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    async (SqliteConnection db,
    [FromQuery] string? title,
    [FromQuery] string? genre,
    [FromQuery] string? author) => await db.GetBooks(title, genre, author));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await EnsureDb(app.Services);

app.Run();

async Task EnsureDb(IServiceProvider services)
{
    var db = services.GetRequiredService<SqliteConnection>();
    db.Open();
    var sqlBuilder = new SqlBuilder();

    var sql = $@"CREATE TABLE IF NOT EXISTS [Authors] (
                [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
                [Name] TEXT NOT NULL)";
    await db.ExecuteAsync(sql);

    sql = @$"CREATE TABLE IF NOT EXISTS [Books] (
                  [{nameof(Book.Id)}] INTEGER PRIMARY KEY AUTOINCREMENT,
                  [{nameof(Book.Title)}] TEXT NOT NULL,
                  [{nameof(Book.Genre)}] TEXT NOT NULL,
                  [{nameof(Book.Author)}_Id] INTEGER  REFERENCES [Authors] (Id)
                  )";
    await db.ExecuteAsync(sql);

    var insertQuery = @$"INSERT INTO [Authors] VALUES
                        (0, 'Carl Sagan'),
                        (1, 'Terry Pratchett'),
                        (2, 'Robert Cecil Martin'),
                        (3, 'Richter Jeffrey')";
    await db.ExecuteAsync(insertQuery);

    var query = $@"
        INSERT INTO [Books]
            ({nameof(Book.Title)}, {nameof(Book.Genre)}, {nameof(Book.Author)}_Id)
        VALUES
            ('The Demon-Haunted World: Science As a Candle in the Dark', '{Genre.Science.ToString()}', 0),            
            ('Cosmos', '{Genre.Science.ToString()}', 0),
            ('Broca''s Brain: Reflections on the Romance of Science', '{Genre.Science.ToString()}', 0),
            ('Contact', '{Genre.Science.ToString()}', 0),
            ('Pale Blue Dot', '{Genre.Science.ToString()}', 0),
            ('Small Gods','{Genre.Adventure.ToString()}',1),
            ('Small Gods','{Genre.Adventure.ToString()}',1),
            ('Going Postal','{Genre.Adventure.ToString()}',1),
            ('Pyramids','{Genre.Adventure.ToString()}',1),
            ('The Color of Magic','{Genre.Adventure.ToString()}',1),
            ('Guards! Guards!','{Genre.Adventure.ToString()}',1),
            ('Unseen Academicals', '{Genre.Adventure.ToString()}',1),
            ('Clean Agile: Back to Basics. Prentice Hall.', '{Genre.Development.ToString()}', 2),
            ('CLR via C#', '{Genre.Development.ToString()}', 3),
            ('Clean Architecture: A Craftsman''s Guide to Software Structure and Design', '{Genre.Development.ToString()}',2)";
    await db.ExecuteAsync(query);       
}
