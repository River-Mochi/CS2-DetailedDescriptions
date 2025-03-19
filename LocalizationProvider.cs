namespace DetailedDescriptions;

public static class LocalizationProvider
{
    public static string GetLocalizedText(string locale)
    {
        switch (locale)
        {
            case "en-US":
                return "Lot Sizes: %data%";
            case "de-DE":
                return "Grundstücksgrößen: %data%";
            case "es-ES":
                return "Tamaños de lotes: %data%";
            case "fr-FR":
                return "Tailles de lots: %data%";
            case "it-IT":
                return "Dimensioni dei lotti: %data%";
            case "ja-JP":
                return "区画サイズ: %data%";
            case "ko-KR":
                return "대지 크기: %data%";
            case "pl-PL":
                return "Rozmiary działek: %data%";
            case "pt-BR":
                return "Tamanhos dos lotes: %data%";
            case "ru-RU":
                return "Размеры участков: %data%";
            case "zh-HANS":
                return "地块大小: %data%";
            case "zh-HANT":
                return "地塊大小: %data%";
            default:
                return "Missing translation: %data%";
        }
    }
    
    public static string GetLocalizedAnd(string locale)
    {
        switch (locale)
        {
            case "en-US":
                return "and";
            case "de-DE":
                return "und";
            case "es-ES":
                return "y";
            case "fr-FR":
                return "et";
            case "it-IT":
                return "e";
            case "ja-JP":
                return "と";
            case "ko-KR":
                return "및";
            case "pl-PL":
                return "i";
            case "pt-BR":
                return "e";
            case "ru-RU":
                return "и";
            case "zh-HANS":
                return "和";
            case "zh-HANT":
                return "和";
            default:
                return "?";
        }

    }
}