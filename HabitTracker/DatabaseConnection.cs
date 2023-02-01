using System.Data.SQLite;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace HabitTracker
{
    class DatabaseConnection
    {
        public SQLiteConnection sqliteConnection;

        public void DBConnection()
        {
            CreateDBConnection();
        }

        public void CloseConnection()
        {
            CloseDBConnection(sqliteConnection);
        }

        public void CreateTable()
        {
            CreateDBTable();
        }

        public void InsertRecord()
        {
            InsertDBRecord();
        }

        private SQLiteConnection CreateDBConnection()
        {
            sqliteConnection = new SQLiteConnection("Data Source=habitTracker.db; Version=3; New=True; Compress=True;");

            try
            {
                sqliteConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return sqliteConnection;
        }

        private void CloseDBConnection(SQLiteConnection sqliteConnection)
        {
            sqliteConnection.Close();
        }

        private void CreateDBTable()
        {
            SQLiteCommand sqliteCommand;
            string query = @"CREATE TABLE IF NOT EXISTS codingTracker (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Date TEXT,
                            Hours INTEGER
                            )";
            sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = query;
            sqliteCommand.ExecuteNonQuery();
        }

        private void InsertDBRecord()
        {
            string date = GetDataInput();

            int hours = GetNumberInput("Please enter the number of hours coded.");

            SQLiteCommand sqliteCommand;
            sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = $"INSERT INTO codingTracker(date, hours) VALUES('{date}', {hours});";
            sqliteCommand.ExecuteNonQuery();
        }

        private void DeleteDBRecord()
        {
            Console.Clear();
            ShowAllDBRecords();

            int recordId = GetNumberInput("Please type the Id of the record you want to delete. Type 0 to return.");

            SQLiteCommand sqliteCommand;
            sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = $"DELETE from codingTracker WHERE Id = '{recordId}'";
            int rowCount = sqliteCommand.ExecuteNonQuery();

            if (rowCount == 0)
            {
                Console.WriteLine($"Record with Id {recordId} does not exist.");
                DeleteDBRecord();
            }

            Console.WriteLine($"Record with Id {recordId} was deleted.");

            GetUserInput();
        }

        private void UpdateDBRecord()
        {
            Console.Clear();
            ShowAllDBRecords();

            int recordId = GetNumberInput("Please type the Id of the record you would like to update. Type 0 to return.");

            SQLiteCommand sqliteCommand;
            sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = $"SELECT EXISTS(SELECT 1 FROM codingTracker WHERE Id = {recordId}";
            int checkQuery = Convert.ToInt32(sqliteCommand.ExecuteScalar());

            if (checkQuery == 0 )
            {
                Console.WriteLine($"Record with Id {recordId} does not exist.");
                UpdateDBRecord();
            }
            else
            {
                string date = GetDataInput();

                int hours = GetNumberInput("Please enter the number of hours coded.");

                sqliteCommand= sqliteConnection.CreateCommand();
                sqliteCommand.CommandText = $"UPDATE codingTracker SET date = '{date}', hours = {hours} WHERE Id = {recordId}";
                sqliteCommand.ExecuteNonQuery();
            }
        }

        private void ShowAllDBRecords()
        {
            Console.Clear();
            List<CodingRecord> codingRecords = new();

            SQLiteDataReader sqliteDataReader;
            SQLiteCommand sqliteCommand;
            sqliteCommand = sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM codingTracker";
            sqliteDataReader = sqliteCommand.ExecuteReader();

            if (sqliteDataReader.HasRows)
            {
                while (sqliteDataReader.Read())
                {
                    codingRecords.Add(
                        new CodingRecord
                        {
                            Id = sqliteDataReader.GetInt32(0),
                            Date = DateTime.ParseExact(sqliteDataReader.GetString(1), "mm-dd-yy", new CultureInfo("en-US")),
                            Hours = sqliteDataReader.GetInt32(2),
                        });
                }
            }
            else
            {
                Console.WriteLine("No data.");
            }

            // Loop through the collection of records and display them to the console.
            Console.WriteLine("------------All Records------------");

            foreach (CodingRecord record in codingRecords)
            {
                Console.WriteLine($"{record.Id} - {record.Date.ToString("mm-dd-yy")} - Hours: {record.Hours}");
            }
            Console.WriteLine("-----------------------------------");
            Console.ReadLine();
        }

        private int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();

            while (!int.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("Invalid number. Try again.");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }
        // Move this a separate class
        private void GetUserInput()
        {
            bool closeApp = false;

            while (closeApp == false)
            {
                Console.Clear();
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to close the application.");
                Console.WriteLine("Type 1 to View All Records");
                Console.WriteLine("Type 2 to Insert Record");
                Console.WriteLine("Type 3 to Delete Record");
                Console.WriteLine("Type 4 to Update Record");
                Console.WriteLine("-----------------------------------");

                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!");
                        CloseConnection();
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        ShowAllDBRecords();
                        break;
                    case "2":
                        InsertDBRecord();
                        break;
                    case "3":
                        DeleteDBRecord();
                        break;
                    case "4":
                        UpdateDBRecord();
                        break;
                    default:
                        Console.WriteLine("Invalid Command. Please type a number from 0-4");
                        break;
                }
            }
        }

        private string GetDataInput()
        {
            Console.WriteLine("\nPlease enter the date: (Format: mm-dd-yy). Type 0 to return to main menu.");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput, "mm-dd-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid date. (Format: mm-dd-yy). Type 0 to return to Main Menu or try again.");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }
    }
}
