using Npgsql;

namespace messenger
{
    internal class Program
    {
        const string CONNECTIONSTRING = "Server=localhost;Port=5432;Database=Assignment;User Id=postgres;Password=135;";
        
        static void Main(string[] args)
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            CreateTables();

            bool insertion = true;
            while (insertion)
            {
                Console.WriteLine("Please write 'username password' in this order...");
                string[] parts = Console.ReadLine()!.Split(' ');
                
                connection.Open();
                string query = $"Insert into users(username, password) values ({parts[0]}, {parts[1]});";
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                command.ExecuteNonQuery();
                connection.Close();

                Console.WriteLine("Inserted successfully!\nDo you want to continue? (Y/n)");
                if (Console.ReadLine()!.ToUpper() == "N")
                {
                    insertion = false;
                }
            }
            

        }

        public static void CreateTables()
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Create table users if not exists (id bigserial primary key, username varchar(50) unique, password varchar(50)); Create table messengers if not exists (id bigserial primary key, user varchar(50), fromUser varchar(50), message text);";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
            Console.WriteLine("Success in creating tables!");
        }

        public static void SelectUsers()
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Create table users (id bigserial primary key, username varchar(50) unique, password varchar(50)); Create table messengers (id bigserial primary key, user varchar(50), fromUser varchar(50), message text);";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

    }
}
