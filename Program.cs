using Npgsql;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Text;

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
                Console.Clear();
                Console.WriteLine("1.Sign in\n2.Sign Up\n3.Exit\n");
                string response = Console.ReadLine()!;
                
                if (response == "1")
                {
                    Console.Clear();
                    Console.Write("Username >> ");
                    string username = Console.ReadLine()!;
                    Console.Write("Password >> ");
                    string password = Console.ReadLine()!;

                    if (!CheckUser(username, password))
                    {
                        Console.WriteLine("404 error!\nUsername or Password is incorrect!");
                        break;
                    }

                    fromUser = username;

                    Console.WriteLine("Signed In successfully");
                    Console.ReadKey();
                    break;
                }

                else if (response == "2")
                {
                    Console.Clear();
                    Console.Write("Username >> ");
                    string username = Console.ReadLine()!;
                    Console.Write("Password >> ");
                    string password = Console.ReadLine()!;

                    connection.Open();
                    string query = $"Insert into users(username, password) values ('{username}', '{password}');";
                    NpgsqlCommand command = new NpgsqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();

                    fromUser = username;

                    Console.WriteLine("Signed Up successfully!");
                    Console.ReadKey();
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
                Console.WriteLine("1.Send Message\n2.View Messages\n3.Exit");
                string response = Console.ReadLine()!;
                
                if (response == "1")
                {
                    SelectAllUsers();
                    Console.WriteLine("Enter username to write him/her...");
                    string toUser = Console.ReadLine()!;

                    Console.WriteLine("Enter your message...");
                    string message = Console.ReadLine()!;
                    AddMessage(fromUser, toUser, message);
                }

                else if (response == "2")
                {
                    ViewUserMessages(fromUser);
                    Console.ReadKey();
                }

                else if (response == "3")
                {
                    Console.Clear();
                    Console.WriteLine("Good Bye!");
                    return;
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

            string query = $"Insert into messages(fromUser, toUser, message) values ('{fromUser}', '{toUser}', '{message}');";
            NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
            Console.WriteLine("Message was sent successfully!");
            Console.ReadKey();
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

        public static string HashPassword(string password)
        {
            const int keySize = 64;
            const int iterations = 350000;
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
            
            var salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }
    }
}
