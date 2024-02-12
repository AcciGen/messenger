using Npgsql;
using System.ComponentModel.Design;

namespace messenger
{
    internal class Program
    {
        const string CONNECTIONSTRING = "Server=localhost;Port=5432;Database=Assignment;User Id=postgres;Password=135;";
        
        static void Main(string[] args)
        {
            CreateTables();
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);
            string fromUser = "";

            while (true)
            {
                Console.WriteLine("1.Sign in\n2.Sign Up\n3.Exit\n");
                string response = Console.ReadLine()!;
                
                if (response == "1")
                {
                    Console.Clear();
                    Console.WriteLine("Please write your 'username password' to sign in...");
                    string[] signInParts = Console.ReadLine()!.Split(' ');

                    if (!CheckUser(signInParts[0], signInParts[1]))
                    {
                        Console.WriteLine("404 error!\nUsername or Password is incorrect!");
                        break;
                    }

                    fromUser = signInParts[0].ToString();

                    Console.WriteLine("Signed In successfully");
                    Console.ReadKey();
                    break;
                }

                else if (response == "2")
                {
                    Console.Clear();
                    Console.WriteLine("Please write 'username password' to sign up...");
                    string[] SignUpParts = Console.ReadLine()!.Split(' ');

                    connection.Open();
                    string query = $"Insert into users(username, password) values ({SignUpParts[0]}, {SignUpParts[1]});";
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                    fromUser = SignUpParts[0].ToString();

                    Console.WriteLine("Signed Up successfully!");
                    Console.ReadKey();
                    break;
                }   

                else if (response == "3")
                {
                    Console.Clear();
                    Console.WriteLine("Good Bye!");
                    return;
                }
            }

            while (true)
            {
                Console.Clear();

                SelectAllUsers();
                Console.WriteLine("Enter username to write him/her...");
                string toUser = Console.ReadLine()!;

                Console.WriteLine("Enter your message...");
                string message = Console.ReadLine()!;
                AddMessage(fromUser, toUser, message);

                Console.WriteLine("Do you want to send message anew or view your messages?\n(1/2)");
                string response = Console.ReadLine()!;
                if (response == "2")
                {
                    ViewUserMessages(fromUser);
                    break;
                }
            }
        }

        public static void CreateTables()
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Create table if not exists users (id bigserial primary key, username varchar(50) unique, password varchar(50)); Create table if not exists messages (id bigserial primary key, fromUser varchar(50), toUser varchar(50), message text);";
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
                if (reader[1].ToString() == username && reader[2].ToString() == password)
                {
                    check = true;
                }
            }

            connection.Close();
            return check;
        }

        public static void SelectAllUsers()
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Select * from users;";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader[1]);
            }
            Console.WriteLine();

            connection.Close();
        }

        public static void AddMessage(string fromUser, string toUser, string message)
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Insert into messages(fromUser, toUser, message) values ({fromUser}, {toUser}, {message});";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
            Console.WriteLine("Message was sent successfully!");
        }

        public static void ViewUserMessages(string user)
        {
            NpgsqlConnection connection = new NpgsqlConnection(CONNECTIONSTRING);

            connection.Open();

            string query = $"Select * from messages;";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader[1].ToString() == user)
                {
                    Console.WriteLine($"To user >> {reader[2]}\n\tmessage >> {reader[3]}");
                }
            }

            connection.Close();
        }
    }
}
