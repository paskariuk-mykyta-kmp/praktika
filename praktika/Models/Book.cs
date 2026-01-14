namespace praktika.Models
{
    public class Book : Product
    {
        public string Author { get; set; }
        public int PageCount { get; set; }

        public Book(int id, string title, double price, string author, int pageCount)
            : base(id, title, price)
        {
            Author = author;
            PageCount = pageCount;
        }

        public override string GetTypeString() => "Book";
        public override string GetDetails() => $"Автор: {Author}, Стор: {PageCount}";

        public override string ToCsv()
        {
            return $"{base.ToCsv()};{Author};{PageCount}";
        }
    }
}