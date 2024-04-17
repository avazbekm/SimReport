namespace SimReport.Services.Helpers;

public static class ConvertToStandart
{
    public static string ConvertFirstToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
