using Npgsql;

namespace messenger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string CONNECTIONSTRING = "Server=localhost;Port=5432;Database=Assignment;User Id=postgres;Password=135;";

            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);
            connection.Open();

            query = $"Create table users (id bigserial primary key, username varchar(50) unique, password varchar(50)); Create table messengers (id bigserial primary key, user varchar(50), fromUser varchar(50), message text);";

            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            command.ExecuteNonQuery();

            connection.Close();
            Console.WriteLine("Success in creating tables!");
        }
    }
}
