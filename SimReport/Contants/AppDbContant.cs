using Microsoft.EntityFrameworkCore;
using SimReport.Entities.Users;

namespace SimReport.Contants;

public class AppDbContant
{
    public const string DB_CONNECTIONSTRING = $"Host={"localhost"};" +
        $"Port={"5432"};" +
        $"Database={"SimReportDB"};" +
        $"User ID={"postgres"};" +
        $"Password={"root"}";
}
