using System;
using System.Collections.Generic;
using System.Text;
using praktika.Models;

namespace praktika
{
    class Program
    {
        static List<Product> products = new List<Product>();
        static List<Product> cart = new List<Product>();
        static User currentUser = null;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Console.InputEncoding = Encoding.UTF8;

            FileManager.CheckFiles();

            products = FileManager.ReadProducts();

            while (currentUser == null)
            {
                Console.Clear();
                Console.WriteLine("=== ВХІД ===");
                Console.WriteLine("1. Увійти");
                Console.WriteLine("2. Реєстрація");
                Console.WriteLine("0. Вихід");
                Console.Write("Вибір: ");
                string k = Console.ReadLine();

                if (k == "1") Login();
                else if (k == "2") Register();
                else if (k == "0") return;
                else Console.WriteLine("Невірний вибір.");
            }

            bool work = true;
            while (work)
            {
                Console.Clear();
                Console.WriteLine($"Привіт, {currentUser.Username}! Роль: {(currentUser.IsAdmin ? "Адмін" : "Клієнт")}");
                Console.WriteLine($"Товарів: {products.Count}");
                if (!currentUser.IsAdmin) Console.WriteLine($"В кошику: {cart.Count}");
                Console.WriteLine("----------------");

                Console.WriteLine("1. Показати товари");
                Console.WriteLine("2. Пошук");
                Console.WriteLine("3. Сортування (за ціною)");
                Console.WriteLine("4. Статистика");

                if (currentUser.IsAdmin)
                {
                    Console.WriteLine("5. Додати товар");
                    Console.WriteLine("6. Видалити товар");
                }
                else
                {
                    Console.WriteLine("5. Додати в кошик");
                    Console.WriteLine("6. Оформити покупку");
                }

                Console.WriteLine("0. Вихід");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ShowList(); break;
                    case "2": Search(); break;
                    case "3": Sort(); break;
                    case "4": Stats(); break;
                    case "5":
                        if (currentUser.IsAdmin) AddProd();
                        else ToCart();
                        break;
                    case "6":
                        if (currentUser.IsAdmin) DelProd();
                        else Buy();
                        break;
                    case "0": work = false; break;
                }
            }
        }

        static void Login()
        {
            Console.Write("Логін: ");
            string l = Console.ReadLine();
            Console.Write("Пароль: ");
            string p = Console.ReadLine();

            List<User> allUsers = FileManager.ReadUsers();
            string hash = FileManager.GetHash(p);

            bool found = false;
            foreach (User u in allUsers)
            {
                if (u.Username == l && u.PasswordHash == hash)
                {
                    currentUser = u;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("Помилка входу! Натисніть Enter.");
                Console.ReadLine();
            }
        }

        static void Register()
        {
            Console.Write("Новий логін: ");
            string l = Console.ReadLine();

            List<User> all = FileManager.ReadUsers();
            foreach (var u in all)
            {
                if (u.Username == l)
                {
                    Console.WriteLine("Такий юзер вже є.");
                    Console.ReadLine();
                    return;
                }
            }

            Console.Write("Новий пароль: ");
            string p = Console.ReadLine();

            FileManager.AddUserToFile(l, p, false);
            Console.WriteLine("Успішно! Тепер увійдіть.");
            Console.ReadLine();
        }

        static void ShowList()
        {
            Console.Clear();
            Console.WriteLine("ID | Тип | Назва | Ціна | Інфо");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id} | {p.GetTypeString()} | {p.Title} | {p.Price} | {p.GetDetails()}");
            }
            Console.WriteLine("\nEnter для далі...");
            Console.ReadLine();
        }

        static void Search()
        {
            Console.Write("Введіть текст: ");
            string q = Console.ReadLine().ToLower();
            foreach (var p in products)
            {
                if (p.Title.ToLower().Contains(q))
                    Console.WriteLine($"{p.Title} - {p.Price}");
            }
            Console.ReadLine();
        }

        static void Sort()
        {
            products.Sort((a, b) => a.Price.CompareTo(b.Price));
            Console.WriteLine("Відсортовано!");
            Console.ReadLine();
        }

        static void Stats()
        {
            if (products.Count == 0) return;
            double sum = 0;
            double min = products[0].Price;
            double max = products[0].Price;

            foreach (var p in products)
            {
                sum += p.Price;
                if (p.Price < min) min = p.Price;
                if (p.Price > max) max = p.Price;
            }

            Console.WriteLine($"Всього: {products.Count}");
            Console.WriteLine($"Сума: {sum}");
            Console.WriteLine($"Середня: {sum / products.Count}");
            Console.WriteLine($"Мін: {min}, Макс: {max}");
            Console.ReadLine();
        }

        static void AddProd()
        {
            Console.WriteLine("Тип: 1-Книга, 2-Манга, 3-Закладка, 4-Листівка");
            string t = Console.ReadLine();

            try
            {
                int id = FileManager.GetNewId("products.csv");
                Console.Write("Назва: "); string name = Console.ReadLine();
                Console.Write("Ціна: "); double price = double.Parse(Console.ReadLine());

                Product newItem = null;

                if (t == "1")
                {
                    Console.Write("Автор: "); string a = Console.ReadLine();
                    Console.Write("Сторінок: "); int pg = int.Parse(Console.ReadLine());
                    newItem = new Book(id, name, price, a, pg);
                }
                else if (t == "2")
                {
                    Console.Write("Мангака: "); string m = Console.ReadLine();
                    Console.Write("Том: "); int v = int.Parse(Console.ReadLine());
                    newItem = new Manga(id, name, price, m, v);
                }
                else if (t == "3")
                {
                    Console.Write("Матеріал: "); string m = Console.ReadLine();
                    Console.Write("Дизайн: "); string d = Console.ReadLine();
                    newItem = new Bookmark(id, name, price, m, d);
                }
                else if (t == "4")
                {
                    Console.Write("Є конверт (+/-): "); string env = Console.ReadLine();
                    newItem = new Postcard(id, name, price, env.Contains("+"));
                }

                if (newItem != null)
                {
                    products.Add(newItem);
                    FileManager.SaveAllProducts(products);
                    Console.WriteLine("Збережено!");
                }
            }
            catch
            {
                Console.WriteLine("Помилка введення.");
            }
            Console.ReadLine();
        }

        static void DelProd()
        {
            Console.Write("Введіть ID: ");
            int id = int.Parse(Console.ReadLine());

            Product toDel = null;
            foreach (var p in products)
            {
                if (p.Id == id) toDel = p;
            }

            if (toDel != null)
            {
                products.Remove(toDel);
                FileManager.SaveAllProducts(products);
                Console.WriteLine("Видалено.");
            }
            else
            {
                Console.WriteLine("Не знайдено.");
            }
            Console.ReadLine();
        }

        static void ToCart()
        {
            Console.Write("ID товару: ");
            int id = int.Parse(Console.ReadLine());
            foreach (var p in products)
            {
                if (p.Id == id)
                {
                    cart.Add(p);
                    Console.WriteLine("Додано в кошик.");
                    return;
                }
            }
            Console.WriteLine("Нема такого ID.");
            Console.ReadLine();
        }

        static void Buy()
        {
            if (cart.Count == 0)
            {
                Console.WriteLine("Кошик пустий.");
                Console.ReadLine();
                return;
            }

            double total = 0;
            Console.WriteLine("ЧЕК:");
            foreach (var p in cart)
            {
                Console.WriteLine($"{p.Title} - {p.Price}");
                total += p.Price;
            }
            Console.WriteLine($"РАЗОМ: {total}");

            FileManager.AddOrder(currentUser.Username, total);

            cart.Clear();
            Console.WriteLine("Дякуємо!");
            Console.ReadLine();
        }
    }
}