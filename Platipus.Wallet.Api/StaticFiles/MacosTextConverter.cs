namespace Platipus.Wallet.Api.StaticFiles;

using System.Text;

public enum Layout
{
    Abc,
    Ukrainian,
    Russian
}

public class MacosTextConverter
{
    private static readonly char[] Abc;
    private static readonly char[] Ukrainian;
    private static readonly char[] Russian;

    private static object ComboLock { get; } = new();
    private static int CurrentCombo { get; set; }

    private static bool IsFinished  { get; set; }

    // private static void FinishCurrentCombo()
    // {
    //     lock (ComboLock)
    //     {
    //         CurrentCombo = 1;
    //     }
    // }

    private static void FinishCombo()
    {
        lock (ComboLock)
        {
            Task.Run(
                () =>
                {

                    lock (ComboLock)
                    {

                    }
                });
        }
    }

    private static (Layout from, Layout to) GetCombo()
    {
        lock (ComboLock)
        {
            var currentCombo = CurrentCombo++;
            return currentCombo switch
            {
                1 => (Layout.Abc, Layout.Ukrainian),
                2 => (Layout.Abc, Layout.Russian),
                3 => (Layout.Ukrainian, Layout.Abc),
                4 => (Layout.Ukrainian, Layout.Russian),
                5 => (Layout.Russian, Layout.Ukrainian),
                6 => (Layout.Russian, Layout.Abc),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public static (bool noDiff, string newText) Convert(string text, Layout from, Layout to)
    {
        var charsFrom = Abc;
        var charsTo = Ukrainian;
        var noDiff = true;
        var sb = new StringBuilder(text.Length);

        foreach (var ch in text)
        {
            var indexOf = Array.IndexOf(Abc, ch);

            if (indexOf is -1)
            {
                sb.Append(ch);
                continue;
            }

            var newCh = Ukrainian[indexOf];
            if (newCh != ch)
                noDiff = false;

            sb.Append(newCh);
        }

        if (noDiff)
        {
        }

        var newText = sb.ToString();
        return (noDiff, newText);
    }

    static MacosTextConverter()
    {
        var abcRegular = new[]
        {
            '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=',
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']', '\\',
            'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'',
            'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/'
        };

        var abcShift = new[]
        {
            '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+',
            'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', '{', '}', '|',
            'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ':', '\"',
            'Z', 'X', 'C', 'V', 'B', 'N', 'M', '<', '>', '?'
        };

        var ukrainianRegular = new[]
        {
            'ґ', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=',
            'й', 'ц', 'у', 'к', 'е', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ї', 'ʼ',
            'ф', 'і', 'в', 'а', 'п', 'р', 'о', 'л', 'д', 'ж', 'є',
            'я', 'ч', 'с', 'м', 'и', 'т', 'ь', 'б', 'ю', '.'
        };

        var ukrainianShift = new[]
        {
            'Ґ', '!', '\"', '№', ';', '%', ':', '?', '*', '(', ')', '_', '+',
            'Й', 'Ц', 'У', 'К', 'Е', 'Н', 'Г', 'Ш', 'Щ', 'З', 'Х', 'Ї', '₴',
            'Ф', 'І', 'В', 'А', 'П', 'Р', 'О', 'Л', 'Д', 'Ж', 'Є',
            'Я', 'Ч', 'С', 'М', 'И', 'Т', 'Ь', 'Б', 'Ю', ','
        };

        var russianRegular = new[]
        {
            'ґ', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=',
            'й', 'ц', 'у', 'к', 'е', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ъ', 'ё',
            'ф', 'ы', 'в', 'а', 'п', 'р', 'о', 'л', 'д', 'ж', 'э',
            'я', 'ч', 'с', 'м', 'и', 'т', 'ь', 'б', 'ю', '.'
        };

        var russianShift = new[]
        {
            'Ґ', '!', '\"', '№', ';', '%', ':', '?', '*', '(', ')', '_', '+',
            'Й', 'Ц', 'У', 'К', 'Е', 'Н', 'Г', 'Ш', 'Щ', 'З', 'Х', 'Ъ', 'Ё',
            'Ф', 'Ы', 'В', 'А', 'П', 'Р', 'О', 'Л', 'Д', 'Ж', 'Э',
            'Я', 'Ч', 'С', 'М', 'И', 'Т', 'Ь', 'Б', 'Ю', ','
        };

        Abc = abcRegular.Concat(abcShift).ToArray();
        Ukrainian = ukrainianRegular.Concat(ukrainianShift).ToArray();
        Russian = russianRegular.Concat(russianShift).ToArray();
    }
}