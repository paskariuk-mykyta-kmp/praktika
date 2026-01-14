namespace praktika.Models
{
    public class Bookmark : Product
    {
        public string Material { get; set; }
        public string Design { get; set; }

        public Bookmark(int id, string title, double price, string material, string design)
            : base(id, title, price)
        {
            Material = material;
            Design = design;
        }

        public override string GetTypeString() => "Bookmark";
        public override string GetDetails() => $"Матеріал: {Material}, Дизайн: {Design}";

        public override string ToCsv() => $"{base.ToCsv()};{Material};{Design}";
    }
}