using System.ComponentModel;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Net.NetworkInformation;

abstract class Currency
{
    private int big_part { get; set; }
    private int small_part { get; set; }

    public int BigPart
    {
        get => big_part;
        set => big_part = value >= 0 ? value : throw new ArgumentException("Значение должно быть неотрицательным");
    }

    public int SmallPart
    {
        get => small_part;
        set => small_part = (value >= 0 && value <= 99) ? value
               : throw new ArgumentException("Дробная часть должна быть от 0 до 99");
    }

    public abstract void ConvertInRubles();

    public abstract void OutputToScreen();
    public abstract JsonElement ToJson();
    public virtual void ConvertToPens()
    { }
    public virtual void ConvertToCents()
    { }

    public virtual void FromJson(JsonElement element)
    {
        BigPart = element.GetProperty("BigPart").GetInt32();
        SmallPart = element.GetProperty("SmallPart").GetInt32();
    }

    protected double ReadExchangeRate(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            if (double.TryParse(Console.ReadLine(), out double rate) && rate > 0)
                return rate;
            Console.WriteLine("Ошибка: введите положительное число");
        }
    }

    protected int ReadInt(string message, int min, int max)
    {
        int result = 0;
        while (true)
        {
            Console.WriteLine(message);
            if (max == -1)
            {
                if (int.TryParse(Console.ReadLine(), out result) && result >= min)
                    return result;
                Console.WriteLine($"Ошибка: введите число от {min}");
            }
            else
            {
                if (int.TryParse(Console.ReadLine(), out result) && result >= min && result <= max)
                    return result;
                Console.WriteLine($"Ошибка: введите число от {min} до {max}");
            }
        }
    }
}

interface ICurrency
{
    void ConvertInRubles();
    void OutputToScreen();
}

class Dollar : Currency, ICurrency
{

    public Dollar(int bp, int sp)
    {
        BigPart = bp;
        SmallPart = sp;
    }
    public Dollar() //инициализатор
    {
        BigPart = ReadInt("Введите количество долларов:", 0, -1);
        SmallPart = ReadInt("Введите количество центов:", 0, 99);
    }

    public override void OutputToScreen() //вывод на экран
    {
        Console.WriteLine($"В наличии {BigPart} долларов {SmallPart} центов");
    }

    public override void ConvertInRubles() //конвертация в рубли
    {
        double rate = ReadExchangeRate("Введите текущий курс доллара к рублю:");
        double converted = BigPart * rate + (SmallPart / 100.0) * rate;
        Console.WriteLine($"Эти доллары равны {converted} рублям");
    }

    public override void ConvertToCents() //Перевод в евроценты
    {
        double CourseToCents = ReadExchangeRate("Введите текущий курс доллара к евро");
        double converted = BigPart * 100 * CourseToCents + (SmallPart) * CourseToCents;
        Console.WriteLine($"При курсе доллара к евро {CourseToCents} общая сумма в долларах равна {converted} в евроцентах");
    }

    public override void ConvertToPens() //перевод в пенсы
    {
        double CourseToPens = ReadExchangeRate("Введите текущий курс доллара к фунтам");
        double converted = BigPart * 100 * CourseToPens + (SmallPart) * CourseToPens;
        Console.WriteLine($"При курсе доллара к фунту {CourseToPens} общая сумма в долларах равна {converted} в пенсах");
    }

    public override JsonElement ToJson()
    {
        return JsonSerializer.SerializeToElement(new
        {
            Type = "Dollar",
            BigPart = BigPart,
            SmallPart = SmallPart
        });
    }


    ~Dollar() //деструктор
    {
        Console.WriteLine($"Объект Dollar ({BigPart} долларов, {SmallPart} центов) уничтожен.");
    }
}


class Euro : Currency, ICurrency
{

    public Euro(int bp, int sp)
    {
        BigPart = bp;
        SmallPart = sp;
    }
    public Euro() //инициализатор
    {
        BigPart = ReadInt("Введите количество евро:", 0, -1);
        SmallPart = ReadInt("Введите количество евроцентов:", 0, 99);
    }

    public override void OutputToScreen() //вывод на экран
    {
        Console.WriteLine($"В наличии {BigPart} Евро {SmallPart} евроцентов");
    }

    public override void ConvertInRubles() //конвертация в рубли
    {
        double rate = ReadExchangeRate("Введите текущий курс евро к рублю:");
        double converted = BigPart * rate + (SmallPart / 100.0) * rate;
        Console.WriteLine($"Эти евро равны {converted} рублям");
    }

    public override void ConvertToCents() //Перевод в центы
    {
        double CourseToCents = ReadExchangeRate("Введите курс евро к доллару");
        double converted = BigPart * 100 * CourseToCents + (SmallPart) * CourseToCents;
        Console.WriteLine($"При курсе евро к доллару {CourseToCents} общая сумма в евро равна {converted} в центах");
    }

    public override void ConvertToPens() //перевод в пенсы
    {
        double CourseToPens = ReadExchangeRate("Введите курс евро к фунтам");
        double converted = BigPart * 100 * CourseToPens + (SmallPart) * CourseToPens;
        Console.WriteLine($"При курсе евро к фунту {CourseToPens} общая сумма в евро равна {converted} в пенсах");
    }

    public override JsonElement ToJson()
    {
        return JsonSerializer.SerializeToElement(new
        {
            Type = "Euro",
            BigPart = BigPart,
            SmallPart = SmallPart
        });
    }


    ~Euro() //деструктор
    {
        Console.WriteLine($"Объект Euro ({BigPart} евро, {SmallPart} евроцентов) уничтожен.");
    }
}

class Pound : Currency, ICurrency
{

    public Pound(int bp, int sp)
    {
        BigPart = bp;
        SmallPart = sp;
    }
    public Pound() //инициализатор
    {
        BigPart = ReadInt("Введите количество фунтов стерлингов:", 0, -1);
        SmallPart = ReadInt("Введите количество пенсов:", 0, 99);
    }

    public override void OutputToScreen() //вывод на экран
    {
        Console.WriteLine($"В наличии {BigPart} фунтов {SmallPart} пенсов");
    }

    public override void ConvertInRubles() //конвертация в рубли
    {
        double course = ReadExchangeRate("Введите курс фунта стерлинга к рублям");
        double converted = BigPart * course + (SmallPart / 100.0) * course;
        Console.WriteLine($"Эти фунты равны {converted} рублям");
    }

    public override void ConvertToCents() //Перевод в центы
    {
        double CourseToUsCents = ReadExchangeRate("введите курс фунта стерлинга к доллару");
        double CourseToEurCents = ReadExchangeRate("Введите курс фунта стерлинга к евро");

        double EurConverted = BigPart * 100 * CourseToEurCents + (SmallPart) * CourseToEurCents;
        Console.WriteLine($"При курсе фунта к евро {CourseToEurCents} общая сумма в фунтах равна {EurConverted} в евроцентах");

        double UsConverted = BigPart * 100 * CourseToUsCents + (SmallPart) * CourseToUsCents;
        Console.WriteLine($"При курсе фунта к доллару {CourseToUsCents} общая сумма в фунтах равна {UsConverted} в центах (доллара)");
    }

    public override JsonElement ToJson()
    {
        return JsonSerializer.SerializeToElement(new
        {
            Type = "Pound",
            BigPart = BigPart,
            SmallPart = SmallPart
        });
    }

    ~Pound() //деструктор
    {
        Console.WriteLine($"Объект Pound ({BigPart} фунтов, {SmallPart} пенсов) уничтожен.");
    }
}

class Program
{

    const string JsonFilePath = "currencies.json";

    static void Main(string[] args)
    {
        // json
        Currency dollar = new Dollar(50, 75);
        Currency euro = new Euro(30, 20);
        Currency pound = new Pound(10, 45);

        dollar.OutputToScreen();
        euro.OutputToScreen();
        pound.OutputToScreen();


        AddToJsonFile(dollar);
        AddToJsonFile(euro);
        AddToJsonFile(pound);

        Console.WriteLine("\nУдаление фунтов:");
        DeleteFromJsonFile("Pound");

        LoadFromJsonFile(JsonFilePath);


    }

    static void AddToJsonFile(Currency currency)  //Добавление в файл
    {
        List<JsonElement> currencies;

        if (File.Exists(JsonFilePath))
        {
            string jsonData = File.ReadAllText(JsonFilePath);
            try
            {
                currencies = JsonSerializer.Deserialize<List<JsonElement>>(jsonData);
            }
            catch { currencies = new List<JsonElement>(); }

            if (currencies == null)
            {
                currencies = new List<JsonElement>();
            }
        }
        else
        {
            currencies = new List<JsonElement>();
        }
        currencies.Add(currency.ToJson());

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(JsonFilePath, JsonSerializer.Serialize(currencies, options));

    }

    static void DeleteFromJsonFile(string currencyType) // Удаление из файла
    {
        if (!File.Exists(JsonFilePath))
        {
            Console.WriteLine("Файл не содержит данных");
            return;
        }

        var currencies = JsonSerializer.Deserialize<List<JsonElement>>(File.ReadAllText(JsonFilePath))
            .Where(c => c.GetProperty("Type").GetString() != currencyType)
            .ToList();

        File.WriteAllText(JsonFilePath, JsonSerializer.Serialize(currencies, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Данные {currencyType} успешно удалены");
    }






    static List<Currency> LoadFromJsonFile(string jsonFilePath) // Загрузка из файла
    {
        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine("Файл не существует");
            return new List<Currency>();
        }

        string jsonData = File.ReadAllText(jsonFilePath);
        var jsonElements = JsonSerializer.Deserialize<List<JsonElement>>(jsonData);

        if (jsonElements == null)
        {
            Console.WriteLine("Файл содержит некорректные данные");
            return new List<Currency>();
        }

        List<Currency> currencies = new List<Currency>();

        foreach (var element in jsonElements)
        {
            string currencyType = element.GetProperty("Type").GetString();

            Currency currency;
            switch (currencyType)
            {
                case "Dollar":
                    currency = new Dollar(
                        element.GetProperty("BigPart").GetInt32(),
                        element.GetProperty("SmallPart").GetInt32());
                    break;
                case "Euro":
                    currency = new Euro(
                        element.GetProperty("BigPart").GetInt32(),
                        element.GetProperty("SmallPart").GetInt32());
                    break;
                case "Pound":
                    currency = new Pound(
                        element.GetProperty("BigPart").GetInt32(),
                        element.GetProperty("SmallPart").GetInt32());
                    break;
                default:
                    Console.WriteLine($"Неизвестный тип валюты: {currencyType}");
                    continue;
            }

            currencies.Add(currency);
        }

        Console.WriteLine("\nЗагруженные данные:");
        foreach (var currency in currencies)
        {
            currency.OutputToScreen();
        }
        Console.WriteLine($"Всего загружено: {currencies.Count} записей");

        return currencies;

    }


}







//// XML

//using System.ComponentModel;
//using System.Text.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Drawing;
//using System.Net.NetworkInformation;
//using System.Xml;

//abstract class Currency
//{
//    private int big_part { get; set; }
//    private int small_part { get; set; }

//    public int BigPart
//    {
//        get => big_part;
//        set => big_part = value >= 0 ? value : throw new ArgumentException("Значение должно быть неотрицательным");
//    }

//    public int SmallPart
//    {
//        get => small_part;
//        set => small_part = (value >= 0 && value <= 99) ? value
//               : throw new ArgumentException("Дробная часть должна быть от 0 до 99");
//    }

//    public abstract void ConvertInRubles();

//    public abstract void OutputToScreen();
//    public virtual void ConvertToPens()
//    { }
//    public virtual void ConvertToCents()
//    { }

//    protected double ReadExchangeRate(string prompt)
//    {
//        while (true)
//        {
//            Console.WriteLine(prompt);
//            if (double.TryParse(Console.ReadLine(), out double rate) && rate > 0)
//                return rate;
//            Console.WriteLine("Ошибка: введите положительное число");
//        }
//    }

//    protected int ReadInt(string message, int min, int max)
//    {
//        int result = 0;
//        while (true)
//        {
//            Console.WriteLine(message);
//            if (max == -1)
//            {
//                if (int.TryParse(Console.ReadLine(), out result) && result >= min)
//                    return result;
//                Console.WriteLine($"Ошибка: введите число от {min}");
//            }
//            else
//            {
//                if (int.TryParse(Console.ReadLine(), out result) && result >= min && result <= max)
//                    return result;
//                Console.WriteLine($"Ошибка: введите число от {min} до {max}");
//            }
//        }
//    }
//}

//interface ICurrency
//{
//    void ConvertInRubles();
//    void OutputToScreen();
//}

//class Dollar : Currency, ICurrency
//{

//    public Dollar(int bp, int sp)
//    {
//        BigPart = bp;
//        SmallPart = sp;
//    }
//    public Dollar() //инициализатор
//    {
//        BigPart = ReadInt("Введите количество долларов:", 0, -1);
//        SmallPart = ReadInt("Введите количество центов:", 0, 99);
//    }

//    public override void OutputToScreen() //вывод на экран
//    {
//        Console.WriteLine($"В наличии {BigPart} долларов {SmallPart} центов");
//    }

//    public override void ConvertInRubles() //конвертация в рубли
//    {
//        double rate = ReadExchangeRate("Введите текущий курс доллара к рублю:");
//        double converted = BigPart * rate + (SmallPart / 100.0) * rate;
//        Console.WriteLine($"Эти доллары равны {converted} рублям");
//    }

//    public override void ConvertToCents() //Перевод в евроценты
//    {
//        double CourseToCents = ReadExchangeRate("Введите текущий курс доллара к евро");
//        double converted = BigPart * 100 * CourseToCents + (SmallPart) * CourseToCents;
//        Console.WriteLine($"При курсе доллара к евро {CourseToCents} общая сумма в долларах равна {converted} в евроцентах");
//    }

//    public override void ConvertToPens() //перевод в пенсы
//    {
//        double CourseToPens = ReadExchangeRate("Введите текущий курс доллара к фунтам");
//        double converted = BigPart * 100 * CourseToPens + (SmallPart) * CourseToPens;
//        Console.WriteLine($"При курсе доллара к фунту {CourseToPens} общая сумма в долларах равна {converted} в пенсах");
//    }

//    ~Dollar() //деструктор
//    {
//        Console.WriteLine($"Объект Dollar ({BigPart} долларов, {SmallPart} центов) уничтожен.");
//    }
//}


//class Euro : Currency, ICurrency
//{

//    public Euro(int bp, int sp)
//    {
//        BigPart = bp;
//        SmallPart = sp;
//    }
//    public Euro() //инициализатор
//    {
//        BigPart = ReadInt("Введите количество евро:", 0, -1);
//        SmallPart = ReadInt("Введите количество евроцентов:", 0, 99);
//    }

//    public override void OutputToScreen() //вывод на экран
//    {
//        Console.WriteLine($"В наличии {BigPart} Евро {SmallPart} евроцентов");
//    }

//    public override void ConvertInRubles() //конвертация в рубли
//    {
//        double rate = ReadExchangeRate("Введите текущий курс евро к рублю:");
//        double converted = BigPart * rate + (SmallPart / 100.0) * rate;
//        Console.WriteLine($"Эти евро равны {converted} рублям");
//    }

//    public override void ConvertToCents() //Перевод в центы
//    {
//        double CourseToCents = ReadExchangeRate("Введите курс евро к доллару");
//        double converted = BigPart * 100 * CourseToCents + (SmallPart) * CourseToCents;
//        Console.WriteLine($"При курсе евро к доллару {CourseToCents} общая сумма в евро равна {converted} в центах");
//    }

//    public override void ConvertToPens() //перевод в пенсы
//    {
//        double CourseToPens = ReadExchangeRate("Введите курс евро к фунтам");
//        double converted = BigPart * 100 * CourseToPens + (SmallPart) * CourseToPens;
//        Console.WriteLine($"При курсе евро к фунту {CourseToPens} общая сумма в евро равна {converted} в пенсах");
//    }

//    ~Euro() //деструктор
//    {
//        Console.WriteLine($"Объект Euro ({BigPart} евро, {SmallPart} евроцентов) уничтожен.");
//    }
//}

//class Pound : Currency, ICurrency
//{

//    public Pound(int bp, int sp)
//    {
//        BigPart = bp;
//        SmallPart = sp;
//    }
//    public Pound() //инициализатор
//    {
//        BigPart = ReadInt("Введите количество фунтов стерлингов:", 0, -1);
//        SmallPart = ReadInt("Введите количество пенсов:", 0, 99);
//    }

//    public override void OutputToScreen() //вывод на экран
//    {
//        Console.WriteLine($"В наличии {BigPart} фунтов {SmallPart} пенсов");
//    }

//    public override void ConvertInRubles() //конвертация в рубли
//    {
//        double course = ReadExchangeRate("Введите курс фунта стерлинга к рублям");
//        double converted = BigPart * course + (SmallPart / 100.0) * course;
//        Console.WriteLine($"Эти фунты равны {converted} рублям");
//    }

//    public override void ConvertToCents() //Перевод в центы
//    {
//        double CourseToUsCents = ReadExchangeRate("введите курс фунта стерлинга к доллару");
//        double CourseToEurCents = ReadExchangeRate("Введите курс фунта стерлинга к евро");

//        double EurConverted = BigPart * 100 * CourseToEurCents + (SmallPart) * CourseToEurCents;
//        Console.WriteLine($"При курсе фунта к евро {CourseToEurCents} общая сумма в фунтах равна {EurConverted} в евроцентах");

//        double UsConverted = BigPart * 100 * CourseToUsCents + (SmallPart) * CourseToUsCents;
//        Console.WriteLine($"При курсе фунта к доллару {CourseToUsCents} общая сумма в фунтах равна {UsConverted} в центах (доллара)");
//    }

//    ~Pound() //деструктор
//    {
//        Console.WriteLine($"Объект Pound ({BigPart} фунтов, {SmallPart} пенсов) уничтожен.");
//    }
//}

//class Program
//{
//    const string XmlFilePath = "currencies.xml";

//    static void Main(string[] args)
//    {
//        Currency dollar = new Dollar(50, 75);
//        Currency euro = new Euro(30, 20);
//        Currency pound = new Pound(10, 45);

//        AddCurrencyToXml(dollar);
//        AddCurrencyToXml(euro);
//        AddCurrencyToXml(pound);

//        Console.WriteLine("Все валюты в файле:");
//        LoadAllFromXml().ForEach(c => c.OutputToScreen());

//        Console.WriteLine("\nПоиск евро:");
//        var foundEuro = FindInXml("Euro");
//        foundEuro?.OutputToScreen();

//        Console.WriteLine("\nУдаление фунтов:");
//        RemoveFromXml("Pound");

//        Console.WriteLine("\nОстались в файле:");
//        LoadAllFromXml().ForEach(c => c.OutputToScreen());
//    }

//    static void AddCurrencyToXml(Currency currency)
//    {
//        XmlDocument doc = new XmlDocument();

//        if (File.Exists(XmlFilePath))
//        {
//            try
//            {
//                doc.Load(XmlFilePath);
//                // Проверяем есть ли корневой элемент
//                if (doc.DocumentElement == null)
//                {
//                    doc.AppendChild(doc.CreateElement("Currencies"));
//                }
//            }
//            catch (XmlException)
//            {
//                // Если файл поврежден, создаем новый
//                doc = new XmlDocument();
//                doc.AppendChild(doc.CreateElement("Currencies"));
//            }
//        }

//        else
//        {
//            doc.AppendChild(doc.CreateElement("Currencies"));
//        }

//        XmlElement currencyElement = doc.CreateElement("Currency");
//        currencyElement.SetAttribute("Type", currency.GetType().Name);

//        XmlElement bigPart = doc.CreateElement("BigPart");
//        bigPart.InnerText = currency.BigPart.ToString();
//        currencyElement.AppendChild(bigPart);

//        XmlElement smallPart = doc.CreateElement("SmallPart");
//        smallPart.InnerText = currency.SmallPart.ToString();
//        currencyElement.AppendChild(smallPart);

//        doc.DocumentElement.AppendChild(currencyElement);
//        doc.Save(XmlFilePath);
//    }

//    static List<Currency> LoadAllFromXml()
//    {
//        List<Currency> currencies = new List<Currency>();

//        if (!File.Exists(XmlFilePath))
//        {
//            Console.WriteLine("Файл не существует!");
//            return currencies;
//        }
//        try
//        {
//            XmlDocument doc = new XmlDocument();
//            doc.Load(XmlFilePath);

//            var currencyNodes = doc.SelectNodes("/Currencies/Currency");
//            if (currencyNodes == null || currencyNodes.Count == 0)
//            {
//                Console.WriteLine("Файл не содержит данных о валютах");
//                return currencies;
//            }
//            foreach (XmlNode node in currencyNodes)
//            {
//                try
//                {
//                    string type = node.Attributes["Type"].Value;
//                    int bigPart = int.Parse(node.SelectSingleNode("BigPart").InnerText);
//                    int smallPart = int.Parse(node.SelectSingleNode("SmallPart").InnerText);

//                    Currency currency = type switch
//                    {
//                        "Dollar" => new Dollar(bigPart, smallPart),
//                        "Euro" => new Euro(bigPart, smallPart),
//                        "Pound" => new Pound(bigPart, smallPart),
//                        _ => null
//                    };

//                    if (currency != null)
//                        currencies.Add(currency);
//                }
//                catch
//                {
//                    // Пропускаем некорректные записи
//                    continue;
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
//        }

//        return currencies;
//    }

//    static Currency FindInXml(string currencyType)
//    {
//        if (!File.Exists(XmlFilePath))
//            return null;

//        XmlDocument doc = new XmlDocument();
//        doc.Load(XmlFilePath);

//        XmlNode node = doc.SelectSingleNode($"/Currencies/Currency[@Type='{currencyType}']");
//        if (node == null)
//            return null;

//        try
//        {
//            int bigPart = int.Parse(node.SelectSingleNode("BigPart").InnerText);
//            int smallPart = int.Parse(node.SelectSingleNode("SmallPart").InnerText);

//            return currencyType switch
//            {
//                "Dollar" => new Dollar(bigPart, smallPart),
//                "Euro" => new Euro(bigPart, smallPart),
//                "Pound" => new Pound(bigPart, smallPart),
//                _ => null
//            };
//        }
//        catch
//        {
//            return null;
//        }
//    }

//    static void RemoveFromXml(string currencyType)
//    {
//        if (!File.Exists(XmlFilePath))
//            return;

//        XmlDocument doc = new XmlDocument();
//        doc.Load(XmlFilePath);

//        XmlNode node = doc.SelectSingleNode($"/Currencies/Currency[@Type='{currencyType}']");
//        if (node != null)
//        {
//            doc.DocumentElement.RemoveChild(node);
//            doc.Save(XmlFilePath);
//            Console.WriteLine($"{currencyType} удален из файла");
//        }
//        else
//        {
//            Console.WriteLine($"{currencyType} не найден в файле");
//        }
//    }
//}