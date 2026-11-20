using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public static class Db
    {
        public const string ConnStr =
            "Server=DESKTOP-F92V228;Database=UniversityDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public const string LibraryConnStr =
            "Server=DESKTOP-F92V228;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}