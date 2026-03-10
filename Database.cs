using System.Data.SQLite;

namespace SmartCalendarApp
{
    public class Database
    {
        public static SQLiteConnection GetConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=SmartCalendar.db;Version=3;");
            conn.Open();
            return conn;
        }
    }
}