using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using praktika.Models;

namespace praktika
{
    static class FileManager
    {
        static string prodFile = "products.csv";
        static string userFile = "users.csv";
        static string orderFile = "orders.csv";

        public static void CheckFiles()
        {
            if (!File.Exists(userFile))
            {
                string adminLine = "1;admin;" + GetHash("admin") + ";true";
                string userLine = "2;user;" + GetHash("1234") + ";false";

                File.WriteAllText(userFile, "Id;Login;Pass;IsAdmin\n" + adminLine + "\n" + userLine + "\n");
            }

            if (!File.Exists(prodFile))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Id;Type;Title;Price;P1;P2");

                sb.AppendLine("1;Book;Кобзар;350;Т. Шевченко;700");
                sb.AppendLine("2;Manga;Наруто;200;М. Кішімото;1");
                sb.AppendLine("3;Bookmark;Котики;50;Картон;Лапки");
                sb.AppendLine("4;Postcard;З Днем Народження;25;True;0");
                sb.AppendLine("5;Bookmark;Герб;120;Метал;Тризуб");
                sb.AppendLine("6;Book;C# in Depth;1200;Jon Skeet;500");

                File.WriteAllText(prodFile, sb.ToString());
            }

            if (!File.Exists(orderFile))
            {
                File.WriteAllText(orderFile, "Id;Date;User;Sum\n");
            }
        }

        public static List<Product> ReadProducts()
        {
            List<Product> list = new List<Product>();

            if (!File.Exists(prodFile)) return list;

            string[] lines = File.ReadAllLines(prodFile);

            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        Product p = Product.FromCsv(lines[i]);
                        if (p != null) list.Add(p);
                    }
                }
                catch
                {
                }
            }
            return list;
        }

        public static List<User> ReadUsers()
        {
            List<User> users = new List<User>();
            if (!File.Exists(userFile)) return users;

            string[] lines = File.ReadAllLines(userFile);
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                        users.Add(User.FromCsv(lines[i]));
                }
                catch { }
            }
            return users;
        }

        public static void SaveAllProducts(List<Product> products)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id;Type;Title;Price;P1;P2");

            foreach (var p in products)
            {
                sb.AppendLine(p.ToCsv());
            }

            File.WriteAllText(prodFile, sb.ToString());
        }

        public static void AddUserToFile(string login, string pass, bool isAdmin)
        {
            int id = GetNewId(userFile);
            string hash = GetHash(pass);
            User u = new User(id, login, hash, isAdmin);

            File.AppendAllText(userFile, u.ToCsv() + "\n");
        }

        public static void AddOrder(string userLogin, double sum)
        {
            int id = GetNewId(orderFile);
            string date = DateTime.Now.ToString();
            string line = $"{id};{date};{userLogin};{sum}";
            File.AppendAllText(orderFile, line + "\n");
        }

        public static int GetNewId(string path)
        {
            if (!File.Exists(path)) return 1;
            string[] lines = File.ReadAllLines(path);

            int maxId = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    string[] p = lines[i].Split(';');
                    int id = int.Parse(p[0]);
                    if (id > maxId) maxId = id;
                }
                catch { }
            }
            return maxId + 1;
        }

        public static string GetHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}