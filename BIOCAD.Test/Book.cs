namespace BIOCAD.Test
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; set; }
        public string Category { get; set; }
        public string PublicationDate {get;set;}
        public string Lang { get; set; }
        public int Pages { get; set; }
        public int AgeLimit { get; set; }
    }
}
