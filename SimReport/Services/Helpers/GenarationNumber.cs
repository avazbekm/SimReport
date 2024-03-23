using System.Collections.Generic;

namespace SimReport.Services.Helpers;

public static class GenarationNumber
{
    public static List<long> GenNum(long number, int count)
    {
        var listNumbers = new List<long>();
        for (int i = 0; i < count; i++)
        {
            listNumbers.Add(number+i);
        }
        return listNumbers;
    }
}
