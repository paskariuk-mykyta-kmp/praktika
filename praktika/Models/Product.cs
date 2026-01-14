using praktika.Models;
using System;

namespace praktika.Models
{
    public abstract class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }

        public Product(int id, string title, double price)
        {
            Id = id;
            Title = title;
            Price = price;
        }

        public abstract string GetTypeString();
        public abstract string GetDetails();

        public virtual string ToCsv()
        {
            return $"{Id};{GetTypeString()};{Title};{Price}";
        }

        public static Product FromCsv(string line)
        {
            var parts = line.Split(';');

            if (parts.Length < 4) return null;

            int id = int.Parse(parts[0]);
            string type = parts[1];
            string title = parts[2];
            double price = double.Parse(parts[3]);

            if (type == "Book")
            {
                return new Book(id, title, price, parts[4], int.Parse(parts[5]));
            }
            else if (type == "Manga")
            {
                return new Manga(id, title, price, parts[4], int.Parse(parts[5]));
            }
            else if (type == "Bookmark")
            {
                return new Bookmark(id, title, price, parts[4], parts[5]);
            }
            else if (type == "Postcard")
            {
                return new Postcard(id, title, price, bool.Parse(parts[4]));
            }

            return null;
        }
    }
}