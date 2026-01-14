namespace praktika.Models
{
    public class Manga : Product
    {
        public string Mangaka { get; set; }
        public int Volume { get; set; }

        public Manga(int id, string title, double price, string mangaka, int volume)
            : base(id, title, price)
        {
            Mangaka = mangaka;
            Volume = volume;
        }

        public override string GetTypeString() => "Manga";
        public override string GetDetails() => $"Мангака: {Mangaka}, Том: {Volume}";

        public override string ToCsv()
        {
            return $"{base.ToCsv()};{Mangaka};{Volume}";
        }
    }
}