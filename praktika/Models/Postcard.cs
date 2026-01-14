namespace praktika.Models
{
    public class Postcard : Product
    {
        public bool HasEnvelope { get; set; }

        public Postcard(int id, string title, double price, bool hasEnvelope)
            : base(id, title, price)
        {
            HasEnvelope = hasEnvelope;
        }

        public override string GetTypeString() => "Postcard";
        public override string GetDetails() => HasEnvelope ? "З конвертом" : "Без конверта";

        public override string ToCsv() => $"{base.ToCsv()};{HasEnvelope}";
    }
}