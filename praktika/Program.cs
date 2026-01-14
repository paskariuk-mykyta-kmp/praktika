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
                DrawHeader("ВХІД У СИСТЕМУ");
                Console.WriteLine("\n  1. Увійти");
                Console.WriteLine("  2. Реєстрація");
                Console.WriteLine("  0. Вихід");
                Console.Write("\n  Ваш вибір > ");
                string k = Console.ReadLine();

                if (k == "1") Login();
                else if (k == "2") Register();
                else if (k == "0") return;
                else PrintError("Невірний вибір.");
            }

            
            bool work = true;
            while (work)
            {
                Console.Clear();
                DrawHeader($"МАГАЗИН | 👤 {currentUser.Username}");

                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"  Роль: {(currentUser.IsAdmin ? "АДМІН" : "КЛІЄНТ")}");
                Console.Write($"  Товарів у базі: {products.Count}");

                if (!currentUser.IsAdmin)
                {
                    Console.Write(" | ");
                    Console.ForegroundColor = cart.Count > 0 ? ConsoleColor.Green : ConsoleColor.DarkGray;
                    Console.WriteLine($"У кошику: {cart.Count}");
                }
                else Console.WriteLine();

                Console.ResetColor();
                Console.WriteLine();

                
                PrintMenuOption("1", "📋 Таблиця товарів");
                PrintMenuOption("2", "🔍 Пошук");
                PrintMenuOption("3", "💰 Сортування (за ціною)");
                PrintMenuOption("4", "📊 Статистика");

                if (!currentUser.IsAdmin)
                {
                    Console.WriteLine();
                    WriteColor("  --- ПОКУПКИ ---", ConsoleColor.Cyan);
                    Console.WriteLine();
                    PrintMenuOption("5", "🛒 Додати в кошик");
                    PrintMenuOption("6", "💳 Оформити замовлення");
                }

                if (currentUser.IsAdmin)
                {
                    Console.WriteLine();
                    WriteColor("  --- АДМІН ПАНЕЛЬ ---", ConsoleColor.DarkYellow);
                    Console.WriteLine();
                    PrintMenuOption("5", "➕ Додати товар");
                    PrintMenuOption("6", "❌ Видалити товар");
                }

                Console.WriteLine();
                PrintMenuOption("0", "🚪 Вихід");
                Console.Write("\nВаш вибір > ");

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
                    default: PrintError("Невірний вибір!"); Console.ReadKey(); break;
                }
            }
        }

       

        static void Login()
        {
            DrawHeader("АВТОРИЗАЦІЯ");
            Console.WriteLine("\n(admin/admin) або (user/1234)\n");

            WriteColor("  Логін:  ", ConsoleColor.Cyan);
            string l = Console.ReadLine();
            WriteColor("  Пароль: ", ConsoleColor.Cyan);
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
                PrintError("Невірний логін або пароль!");
                Console.ReadKey();
            }
        }

        static void Register()
        {
            DrawHeader("РЕЄСТРАЦІЯ");
            WriteColor("  Новий логін: ", ConsoleColor.Cyan);
            string l = Console.ReadLine();

            List<User> all = FileManager.ReadUsers();
            foreach (var u in all)
            {
                if (u.Username == l)
                {
                    PrintError("Такий користувач вже існує.");
                    Console.ReadKey();
                    return;
                }
            }

            WriteColor("  Новий пароль: ", ConsoleColor.Cyan);
            string p = Console.ReadLine();

            FileManager.AddUserToFile(l, p, false);
            PrintSuccess("Успішно! Тепер увійдіть.");
            Console.ReadKey();
        }

        

        static void ShowList()
        {
            DrawHeader("АСОРТИМЕНТ");
            
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" {0,-4} | {1,-10} | {2,-25} | {3,-8} | {4,-30}", "ID", "Тип", "Назва", "Ціна", "Інфо");
            Console.ResetColor();

            foreach (var p in products)
            {
                Console.Write(" ");
                WriteColor($"{p.Id,-4}", ConsoleColor.DarkGray);
                Console.Write(" | ");
                Console.Write($"{p.GetTypeString(),-10}");
                Console.Write(" | ");

                string title = p.Title.Length > 22 ? p.Title.Substring(0, 19) + "..." : p.Title;
                WriteColor($"{title,-25}", ConsoleColor.White);

                Console.Write(" | ");
                WriteColor($"{p.Price,8} ₴", ConsoleColor.Green);
                Console.Write(" | ");
                WriteColor($"{p.GetDetails(),-30}", ConsoleColor.Gray);
                Console.WriteLine();
            }
            Console.WriteLine("\nНатисніть Enter...");
            Console.ReadLine();
        }

        static void Search()
        {
            DrawHeader("ПОШУК");
            Console.Write("Введіть текст: ");
            string q = Console.ReadLine().ToLower();
            Console.WriteLine();

            bool found = false;
            foreach (var p in products)
            {
                if (p.Title.ToLower().Contains(q))
                {
                    Console.Write(" -> ");
                    WriteColor($"{p.Title}", ConsoleColor.White);
                    WriteColor($" ({p.Price} ₴)\n", ConsoleColor.Green);
                    found = true;
                }
            }
            if (!found) PrintError("Нічого не знайдено.");
            Console.ReadLine();
        }

        static void Sort()
        {
            products.Sort((a, b) => a.Price.CompareTo(b.Price));
            PrintSuccess("Список відсортовано за ціною!");
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

            DrawHeader("СТАТИСТИКА");
            Console.WriteLine($"  Всього товарів:    {products.Count}");
            Console.Write($"  Загальна вартість: "); WriteColor($"{sum} ₴\n", ConsoleColor.Green);
            Console.Write($"  Середня ціна:      "); WriteColor($"{(sum / products.Count):F2} ₴\n", ConsoleColor.Yellow);
            Console.Write($"  Найдешевший:       "); WriteColor($"{min} ₴\n", ConsoleColor.Cyan);
            Console.Write($"  Найдорожчий:       "); WriteColor($"{max} ₴\n", ConsoleColor.Red);
            Console.ReadLine();
        }

      

        static void AddProd()
        {
            DrawHeader("ДОДАВАННЯ");
            Console.WriteLine("1. Книга");
            Console.WriteLine("2. Манга");
            Console.WriteLine("3. Закладка");
            Console.WriteLine("4. Листівка");
            Console.Write("Тип > ");
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
                    PrintSuccess("Товар збережено у файл!");
                }
            }
            catch { PrintError("Помилка даних."); }
            Console.ReadLine();
        }

        static void DelProd()
        {
            ShowList(); 
            Console.Write("Введіть ID для видалення: ");
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
                PrintSuccess("Видалено.");
            }
            else
            {
                PrintError("Не знайдено.");
            }
            Console.ReadLine();
        }

        

        static void ToCart()
        {
            ShowList();
            Console.Write("Введіть ID товару: ");
            int id = int.Parse(Console.ReadLine());
            foreach (var p in products)
            {
                if (p.Id == id)
                {
                    cart.Add(p);
                    PrintSuccess("Додано в кошик.");
                    return;
                }
            }
            PrintError("Нема такого ID.");
            Console.ReadLine();
        }

        static void Buy()
        {
            if (cart.Count == 0)
            {
                PrintError("Кошик порожній.");
                Console.ReadLine();
                return;
            }

            DrawHeader("ВАШ ЧЕК");
            double total = 0;
            Console.WriteLine(new string('-', 40));
            foreach (var p in cart)
            {
                Console.WriteLine($" {p.Title,-25} ... {p.Price} ₴");
                total += p.Price;
            }
            Console.WriteLine(new string('-', 40));
            WriteColor($" РАЗОМ ДО СПЛАТИ: {total} ₴", ConsoleColor.Green);

            FileManager.AddOrder(currentUser.Username, total);

            cart.Clear();
            Console.WriteLine("\n\n Дякуємо за покупку!");
            Console.ReadLine();
        }

        

        static void DrawHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.Write("║");
            int spaces = (60 - title.Length) / 2;
            Console.Write(new string(' ', spaces));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(title);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(new string(' ', 60 - spaces - title.Length));
            Console.WriteLine("║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        static void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        static void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n [X] ПОМИЛКА: {msg}");
            Console.ResetColor();
        }

        static void PrintSuccess(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n [OK] {msg}");
            Console.ResetColor();
        }

        static void PrintMenuOption(string key, string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  [{key}] ");
            Console.ResetColor();
            Console.WriteLine(text);
        }
    }
}