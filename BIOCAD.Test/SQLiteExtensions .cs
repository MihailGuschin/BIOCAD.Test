using Dapper;
using Microsoft.Data.Sqlite;

namespace BIOCAD.Test
{
    public static class SQLiteExtensions
    {
        public static async Task<IEnumerable<Book>> GetBooks(
        this SqliteConnection dbConnection,
        string? title,
        string? genre,
        string? author)
        {
            var sqlBuilder = new SqlBuilder();

            sqlBuilder.Select("*");
            sqlBuilder.Select("a.Name AS Author");
            sqlBuilder.Join("[Authors] AS a ON a.Id = [Books].Author_Id");

            if (!string.IsNullOrEmpty(author))                           
                sqlBuilder.Where($"a.Name LIKE '%{author}%'");        
            if (!string.IsNullOrEmpty(title))
                sqlBuilder.Where($"[{nameof(Book.Title)}] LIKE '%{title}%'");
            if (!string.IsNullOrEmpty(genre))
                sqlBuilder.Where($"[{nameof(Book.Genre)}] LIKE '%{genre}%'");

            var builderTemplate = sqlBuilder.AddTemplate("SELECT /**select**/ FROM [Books] /**join**/ /**where**/");
            return await dbConnection.QueryAsync<Book>(builderTemplate.RawSql);
        }
    }
}
