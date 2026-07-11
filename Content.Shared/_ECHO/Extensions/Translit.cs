namespace Content.Shared._ECHO.Extensions;

public enum TransliterationType
{
    Gost,
    ISO
}

public static class Transliteration
{
    private static Dictionary<string, string> _gost = new Dictionary<string, string>(); //ГОСТ 16876-71
    private static Dictionary<string, string> _iso = new Dictionary<string, string>(); //ISO 9-95

    /// <summary>
    /// Транслитерация из русского в английский
    /// </summary>
    /// <param name="text">русский текст</param>
    /// <returns></returns>
    public static string ToEngTranslit(this string text, TransliterationType type = TransliterationType.ISO)
    {
        string output = text;
        Dictionary<string, string> tdict = GetDictionaryByType(type);

        foreach (KeyValuePair<string, string> key in tdict)
        {
            output = output.Replace(key.Key, key.Value);
        }
        return output;
    }

    public static string ToRuTranslit(this string text, TransliterationType type = TransliterationType.ISO)
    {
        string output = text;
        Dictionary<string, string> tdict = GetDictionaryByType(type);

        foreach (KeyValuePair<string, string> key in tdict)
        {
            output = output.Replace(key.Value, key.Key);
        }
        return output;
    }

    private static Dictionary<string, string> GetDictionaryByType(TransliterationType type)
    {
        Dictionary<string, string> tdict = _iso;
        if (type == TransliterationType.Gost) tdict = _gost;
        return tdict;
    }

    static Transliteration()
    {
        _gost.Add("Є", "EH");
        //gost.Add("І", "I");
        //gost.Add("і", "i");
        _gost.Add("№", "#");
        _gost.Add("є", "eh");
        _gost.Add("А", "A");
        _gost.Add("Б", "B");
        _gost.Add("В", "V");
        _gost.Add("Г", "G");
        _gost.Add("Д", "D");
        _gost.Add("Е", "E");
        _gost.Add("Ё", "JO");
        _gost.Add("Ж", "ZH");
        _gost.Add("З", "Z");
        _gost.Add("И", "I");
        _gost.Add("Й", "JJ");
        _gost.Add("К", "K");
        _gost.Add("Л", "L");
        _gost.Add("М", "M");
        _gost.Add("Н", "N");
        _gost.Add("О", "O");
        _gost.Add("П", "P");
        _gost.Add("Р", "R");
        _gost.Add("С", "S");
        _gost.Add("Т", "T");
        _gost.Add("У", "U");
        _gost.Add("Ф", "F");
        _gost.Add("Х", "KH");
        _gost.Add("Ц", "C");
        _gost.Add("Ч", "CH");
        _gost.Add("Ш", "SH");
        _gost.Add("Щ", "SHH");
        _gost.Add("Ъ", "`");
        _gost.Add("Ы", "Y");
        _gost.Add("Ь", "'");
        _gost.Add("Э", "EH");
        _gost.Add("Ю", "YU");
        _gost.Add("Я", "YA");
        _gost.Add("а", "a");
        _gost.Add("б", "b");
        _gost.Add("в", "v");
        _gost.Add("г", "g");
        _gost.Add("д", "d");
        _gost.Add("е", "e");
        _gost.Add("ё", "jo");
        _gost.Add("ж", "zh");
        _gost.Add("з", "z");
        _gost.Add("и", "i");
        _gost.Add("й", "jj");
        _gost.Add("к", "k");
        _gost.Add("л", "l");
        _gost.Add("м", "m");
        _gost.Add("н", "n");
        _gost.Add("о", "o");
        _gost.Add("п", "p");
        _gost.Add("р", "r");
        _gost.Add("с", "s");
        _gost.Add("т", "t");
        _gost.Add("у", "u");

        _gost.Add("ф", "f");
        _gost.Add("х", "kh");
        _gost.Add("ц", "c");
        _gost.Add("ч", "ch");
        _gost.Add("ш", "sh");
        _gost.Add("щ", "shh");
        _gost.Add("ъ", "`");
        _gost.Add("ы", "y");
        _gost.Add("ь", "'");
        _gost.Add("э", "eh");
        _gost.Add("ю", "yu");
        _gost.Add("я", "ya");
        _gost.Add("«", "\"");
        _gost.Add("»", "\"");
        _gost.Add("—", "-");

        _iso.Add("Є", "YE");
        _iso.Add("І", "I");
        _iso.Add("Ѓ", "G");
        _iso.Add("і", "i");
        _iso.Add("№", "");
        _iso.Add("є", "ye");
        _iso.Add("ѓ", "g");
        _iso.Add("А", "A");
        _iso.Add("Б", "B");
        _iso.Add("В", "V");
        _iso.Add("Г", "G");
        _iso.Add("Д", "D");
        _iso.Add("Е", "E");
        _iso.Add("Ё", "YO");
        _iso.Add("Ж", "ZH");
        _iso.Add("З", "Z");
        _iso.Add("И", "I");
        _iso.Add("Й", "J");
        _iso.Add("К", "K");
        _iso.Add("Л", "L");
        _iso.Add("М", "M");
        _iso.Add("Н", "N");
        _iso.Add("О", "O");
        _iso.Add("П", "P");
        _iso.Add("Р", "R");
        _iso.Add("С", "S");
        _iso.Add("Т", "T");
        _iso.Add("У", "U");
        _iso.Add("Ф", "F");
        _iso.Add("Х", "X");
        _iso.Add("Ц", "C");
        _iso.Add("Ч", "CH");
        _iso.Add("Ш", "SH");
        _iso.Add("Щ", "SHH");
        _iso.Add("Ъ", "");
        _iso.Add("Ы", "Y");
        _iso.Add("Ь", "");
        _iso.Add("Э", "E");
        _iso.Add("Ю", "YU");
        _iso.Add("Я", "YA");
        _iso.Add("а", "a");
        _iso.Add("б", "b");
        _iso.Add("в", "v");
        _iso.Add("г", "g");
        _iso.Add("д", "d");
        _iso.Add("е", "e");
        _iso.Add("ё", "yo");
        _iso.Add("ж", "zh");
        _iso.Add("з", "z");
        _iso.Add("и", "i");
        _iso.Add("й", "j");
        _iso.Add("к", "k");
        _iso.Add("л", "l");
        _iso.Add("м", "m");
        _iso.Add("н", "n");
        _iso.Add("о", "o");
        _iso.Add("п", "p");
        _iso.Add("р", "r");
        _iso.Add("с", "s");
        _iso.Add("т", "t");
        _iso.Add("у", "u");
        _iso.Add("ф", "f");
        _iso.Add("х", "x");
        _iso.Add("ц", "c");
        _iso.Add("ч", "ch");
        _iso.Add("ш", "sh");
        _iso.Add("щ", "shh");
        _iso.Add("ъ", "");
        _iso.Add("ы", "y");
        _iso.Add("ь", "");
        _iso.Add("э", "e");
        _iso.Add("ю", "yu");
        _iso.Add("я", "ya");
        _iso.Add("«", "");
        _iso.Add("»", "");
        _iso.Add("—", "-");
        _iso.Add("!", "");
        _iso.Add(".", "");
        _iso.Add(",", "-");
        _iso.Add(" ", "-");
        _iso.Add("'", "");
        _iso.Add("\"", "");
    }
}
