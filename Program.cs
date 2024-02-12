using Npgsql;

namespace messenger
{
    internal class Program
    {
        const string CONNECTIONSTRING = "Server=localhost;Port=5432;Database=Assignment;User Id=postgres;Password=135;";
        
        static void Main(string[] args)
        {
            CreateTables();
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);
            string currentUser;

            bool signing = true;
            while (signing)
            {
                Console.WriteLine("1.Sign in\n2.Sign Up\n3.Exit\n");
                string response = Console.ReadLine()!;
                
                switch (response)
                {
                    case "1":

                        Console.WriteLine("Please write your 'username password' to sign in...");
                        string[] signInParts = Console.ReadLine()!.Split(' ');

                        if (!CheckUser(signInParts[0], signInParts[1]))
                        {
                            Console.WriteLine("404 error!\nUsername or Password is incorrect!");
                            break;
                        }

                        currentUser = signInParts[0];
                        signing = false;

                        Console.WriteLine("Signed In successfully");
                        break;

                    case "2":
                        
                        Console.WriteLine("Please write 'username password' to sign up...");
                        string[] SignUpParts = Console.ReadLine()!.Split(' ');

                        connection.Open();
                        string query = $"Insert into users(username, password) values ({SignUpParts[0]}, {SignUpParts[1]});";
                        NpgsqlCommand command = new NpgsqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        connection.Close();

                        currentUser = SignUpParts[0];
                        signing = false;

                        Console.WriteLine("Signed Up successfully!");
                        break;
                        
                    case "3":

                        return;
                }
            }

            bool messaging = false;
        }

        public static void CreateTables()
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Create table if not exists users (id bigserial primary key, username varchar(50) unique, password varchar(50)); Create table if not exists messengers (id bigserial primary key, toUser varchar(50), fromUser varchar(50), message text);";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static bool CheckUser(string username, string password)
        {
            bool check = false;
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Select * from users;";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader[1].ToString() == username && reader[2].ToString() == password)
                    {
                        check = true;
                    }
                }
            }

            connection.Close();

            return check;
        }

    }
}
