using static Novellus.Lib.Managers.Languages;

namespace Novellus.Lib.Managers;

// Maybe move this to a separete module, like "Data" or something like that
public static class Languages
{
    public enum P5RLanguage
    {
        Unknown,
        English,
        French,
        Italian,
        German,
        Spanish
    }
}

public static class P5RLanguageExt
{
    public static string Name(this P5RLanguage lang)
    {
        return lang switch
        {
            P5RLanguage.English => "English",
            P5RLanguage.French => "French",
            P5RLanguage.Italian => "Italian",
            P5RLanguage.German => "German",
            P5RLanguage.Spanish => "Spanish",
            _ => "Unknown"
        };
    }

    public static string CpkSuffix(this P5RLanguage lang)
    {
        return lang switch
        {
            P5RLanguage.French => "_F",
            P5RLanguage.Italian => "_I",
            P5RLanguage.German => "_G",
            P5RLanguage.Spanish => "_S",
            _ => String.Empty
        };
    }
}
