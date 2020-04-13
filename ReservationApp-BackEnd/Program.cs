using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ReservationApp_BackEnd
{
    public class TakenSeats
    {
        public DateTime takenTime { get; set; }
        public int takenSeat { get; set; }
        public int restaurantId { get; set; }
        public TakenSeats(DateTime takentime, int takenseat, int restaurantid)
        {
            takenTime = takentime;
            takenSeat = takenseat;
            restaurantId = restaurantid;
        }
    }

    class Program
    {
        SqlConnection connection = new SqlConnection("Data Source=luxefood.database.windows.net;Initial Catalog=LuxeFoods;User ID=Klees;Password=Johnny69;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        static void Main(string[] args)
        {
            string val;
            int a = 0;
            Console.WriteLine("Welcome to the Back-End API of LuxeFoods reservation system!\n\n");

            int userId = 0;

            Console.WriteLine("\nEnter your Email: ");
            string userEmail = Console.ReadLine();
            Console.WriteLine("\nEnter your Password: ");
            string userPassword = Console.ReadLine();

            if (userEmail == "" || userPassword == "")
            {
                Console.WriteLine("Vul alle velden in.");
            }
            else
            {
                string q = $"SELECT DISTINCT * FROM [user] WHERE email='{userEmail}'";
                string que = $"SELECT id from [user] where email='{userEmail}'";
                string password = "";
                string typedPassword = EncryptPassword(userPassword);
                try
                {
                    SqlConnection con = new SqlConnection("Data Source=luxefood.database.windows.net;Initial Catalog=LuxeFoods;User ID=Klees;Password=Johnny69;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand cmd = new SqlCommand(q, con);
                        SqlCommand cmnd = new SqlCommand(que, con);
                        cmd.ExecuteNonQuery();
                        

                        using (SqlDataReader reader = cmnd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userId = reader.GetInt32(0);
                            }
                        }

                        con.Close();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        foreach (DataRow dr in dt.Rows)
                        {
                            password = dr["password"].ToString();
                        }
                        if (typedPassword == password)
                        {
                            Console.WriteLine("Je bent nu ingelogd.");
                        }
                        else
                        {
                            Console.WriteLine("deze gebruiker bestaat niet, of het wachtwoord is verkeerd ingevuld.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            bool stopLoop = false;
            while(!stopLoop)
            {
                Console.WriteLine("\n\nWhat do you want to do?");
                Console.WriteLine("1-Make Reservation 2-Check Reservation 3-Change Reservation 4-Exit");
                val = Console.ReadLine();
                
                try
                {
                    a = Convert.ToInt32(val);
                }
                catch
                {
                    Console.WriteLine("Please Enter a Number");
                }

                if (a == 1)
                {
                    Program program = new Program();
                    program.makeReservation(userId);
                }
                else if (a == 2)
                {
                    checkReservation(userId);
                }
                else if (a == 3)
                {
                    changeReservation(userId);
                }
                else if (a == 4)
                {
                    stopLoop = true;
                    Console.WriteLine("Exiting the program...");
                }
                else
                {
                    Console.WriteLine("U can only choose between 1, 2 and 3");
              }
            }
            Console.WriteLine("Escaped the while loop!");
        }

        static string EncryptPassword(string text)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(text));
                return Convert.ToBase64String(data);
            }
        }

        void makeReservation(int userId)
        {
            Console.WriteLine("You can now go and make reservation!\n");
            int restaurantId = 0;
            string restaurantIdString;

            List<string> RestaurantNames = new List<string>();

            SqlCommand read = new SqlCommand("select * from restaurant", connection);
            connection.Open();
            using (SqlDataReader reader = read.ExecuteReader())
            {
                while (reader.Read())
                {
                    RestaurantNames.Add(reader.GetString(1));

                }
                connection.Close();
            }

            Console.WriteLine("Which restaurant do you want to make a reservation for?");
            string restaurantChoice = "";
            int incr = 1;
            foreach (string x in RestaurantNames)
            {
                restaurantChoice += incr + "-" + x + " ";
                incr++;
            }
            Console.WriteLine(restaurantChoice);
            restaurantIdString = Console.ReadLine();
            try
            {
                restaurantId = Convert.ToInt32(restaurantIdString);
            }
            catch
            {
                Console.WriteLine("Enter a number!");
            }

            Console.WriteLine("When do you want to make the reservation?");
            Console.WriteLine("Write it in this format YYYY-MM-DD");
            string date;
            date = Console.ReadLine();

            DateTime parsedDate = DateTime.Parse(date);
      
            Console.WriteLine("\nThis is your date: " + parsedDate);

            List<DateTime> availableTimes = new List<DateTime>() {
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 16, 00, 00),
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 17, 00, 00),
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 18, 00, 00),
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 19, 00, 00),
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 20, 00, 00),
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 21, 00, 00),
                new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 22, 00, 00)
            };
            List<TakenSeats> takenTimesWithTables = new List<TakenSeats>();
            List<TakenSeats> availableTimesWithTables = new List<TakenSeats>();

            for (int y = 1; y < RestaurantNames.Count+1; y++)
            {
                for (int i = 0; i < availableTimes.Count; i++)
                {
                    for (int x = 1; x <= 55; x++)
                    {
                        availableTimesWithTables.Add(new TakenSeats(availableTimes[i], x, y));
                    }
                }
            }
            
            SqlCommand readCommand = new SqlCommand("select datum, tafelNummer, restaurantId from reservering where datum between '" + parsedDate.Month + "/" + parsedDate.Day + "/" + parsedDate.Year + "' and '" + parsedDate.Month + "/" + parsedDate.Day + "/" + parsedDate.Year + " 23:59:59'", connection);
            connection.Open();
            using (SqlDataReader reader = readCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    TakenSeats p1 = new TakenSeats(reader.GetDateTime(0), reader.GetInt32(1), reader.GetInt32(2));
                    takenTimesWithTables.Add(p1);
                }
                connection.Close();
            }

            Console.WriteLine("Succesfully finished getting all the available times");

            foreach (TakenSeats x in takenTimesWithTables)
            {
                foreach (TakenSeats i in availableTimesWithTables.ToList())
                {
                    if (i.restaurantId != restaurantId)
                    {
                        availableTimesWithTables.Remove(i);
                    }
                    if ((x.takenSeat == i.takenSeat) && (x.takenTime == i.takenTime) && (x.restaurantId == restaurantId))
                    {
                        availableTimesWithTables.Remove(i);
                    }
                }
            }

            Console.WriteLine("Those are all the available times for " + parsedDate.Year + "-" + parsedDate.Month + "-" + parsedDate.Day + ": ");
            foreach (TakenSeats x in availableTimesWithTables)
            {
                Console.WriteLine(x.takenTime + " and seat nr. " + x.takenSeat);
            }

            Console.WriteLine("Please choose at what hour you want to make reservation: ");

            int hour = 0;   
            while (hour < 16 || hour > 23)
            {
                string time = Console.ReadLine();

                hour = Convert.ToInt32(time);
            }
            
            TimeSpan ts = new TimeSpan(hour, 0, 0);
            date += " " + ts;
            parsedDate = DateTime.Parse(date);

            int tableNumber = 0;
           
            Console.WriteLine("\nWhich table do you want to reserve? Enter a number: ");
            string tableNumberString = Console.ReadLine();
            try
            {
                tableNumber = Convert.ToInt32(tableNumberString);
            }
            catch
            {
                Console.WriteLine("Please enter a number");
            }

            Console.WriteLine("Your reservation is Finished!");
            Console.WriteLine("This is how you reservation looks like!");
            Console.WriteLine("UserId: " + userId + ", RestaurantId: " + restaurantId + ", Date: " + parsedDate + ", Table Number: " + tableNumber);
            bool submitChecked = false;
            while(!submitChecked)
            {
                Console.WriteLine("\nDo you want to submit the reservation? (1-yes/2-no)");
                string answer = Console.ReadLine();
                int answer32 = Convert.ToInt32(answer);
                if (answer32 == 1)
                {
                    bool trueData = true;
                    foreach (TakenSeats x in takenTimesWithTables)
                    {
                        if (x.takenTime == parsedDate && x.takenSeat == tableNumber && x.restaurantId == restaurantId)
                        {
                            Console.WriteLine("There is already an reservation made for this time and table! Please choose another time or table.");
                            trueData = false;
                            break;
                        }
                    }
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open && trueData)
                    {
                        string q = $"INSERT INTO [reservering] (restaurantId, klantId, datum, tafelNummer) VALUES  ('{restaurantId}', '{userId}', '{date}', '{tableNumber}')";

                        try
                        {
                            SqlCommand cmd = new SqlCommand(q, connection);
                            cmd.ExecuteNonQuery();

                            
                            Console.WriteLine("Reservation Has been Succesfully Sumbited");
                            submitChecked = !submitChecked;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    connection.Close();
                } else if (answer32 == 2)
                {
                    Console.WriteLine("Canceling Reservation....");
                    submitChecked = !submitChecked;
                } else
                {
                    Console.WriteLine("Please Enter either y or n!");
                }
            }
        }

        class reservation
        {
            public int Id { get; set; }
            public int restaurantId { get; set; }
            public int klantId { get; set; }
            public DateTime Date { get; set; }
            public int tableNr { get; set; }
            
            public reservation(int id, int restaurantid, int klantid, DateTime date, int tablenr)
            {
                Id = id;
                restaurantId = restaurantid;
                klantId = klantid;
                Date = date;
                tableNr = tablenr;
            }
        }

        class restaurants
        {
            public int Id { get; set; }
            public string Naam { get; set; }
            public int menuId { get; set; }
            public int amountTables { get; set; }

            public restaurants(int id, string naam, int amounttables, int menuid = 0)
            {
                Id = id;
                Naam = naam;
                menuId = menuid;
                amountTables = amounttables;
            }
        }


        static void checkReservation(int userId)
        {
            Console.WriteLine("These are your current reservation:\n");

            List<reservation> userReservations = new List<reservation>();

            SqlConnection connection = new SqlConnection("Data Source=luxefood.database.windows.net;Initial Catalog=LuxeFoods;User ID=Klees;Password=Johnny69;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            SqlCommand readCommand = new SqlCommand("select * from reservering where klantId='" + userId + "'", connection);
            connection.Open();
            using (SqlDataReader reader = readCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    reservation _ = new reservation(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetDateTime(3), reader.GetInt32(4));
                    userReservations.Add(_);
                }
                connection.Close();
            }

            List<restaurants> allRestaurants = new List<restaurants>();
            
            SqlCommand restReadCommand = new SqlCommand("select * from restaurant", connection);
            connection.Open();
            using (SqlDataReader reader = restReadCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    restaurants _ = new restaurants(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(3));
                    allRestaurants.Add(_);
                }
                connection.Close();
            }

            foreach (reservation x in userReservations)
            {
                Console.WriteLine("id: " + x.Id + " " + allRestaurants[x.restaurantId-1].Naam + " " + x.Date + " Gereserveerde Tafels: " + x.tableNr);
            }
        }

        static void changeReservation(int userId)
        {
            Console.WriteLine("Enter your reservation id");
            int searchId = Convert.ToInt32(Console.ReadLine());

            reservation foundReservation = new reservation(0,0,0,new DateTime(),0);

            SqlConnection connection = new SqlConnection("Data Source=luxefood.database.windows.net;Initial Catalog=LuxeFoods;User ID=Klees;Password=Johnny69;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            SqlCommand readCommand = new SqlCommand("select * from reservering where Id='" + searchId + "'", connection);
            connection.Open();
            using (SqlDataReader reader = readCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    foundReservation = new reservation(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetDateTime(3), reader.GetInt32(4));
                }
                connection.Close();
            }

            List<restaurants> allRestaurants = new List<restaurants>();

            SqlCommand restReadCommand = new SqlCommand("select * from restaurant", connection);
            connection.Open();
            using (SqlDataReader reader = restReadCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    restaurants _ = new restaurants(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(3));
                    allRestaurants.Add(_);
                }
                connection.Close();
            }

            

            bool check = false;
            while (!check)
            {
                Console.WriteLine("Is this your reservation? (y/n) " + foundReservation.Id + " " + allRestaurants[foundReservation.restaurantId - 1].Naam + " " + foundReservation.Date + " tafel nr. " + foundReservation.tableNr);
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    check = !check;
                }
                else if (answer == "n")
                {
                    Console.WriteLine("Leaving...");
                    check = !check;
                }
                else
                {
                    Console.WriteLine("Please Enter y or n");
                }
            }

            check = false;
            while (!check)
            {
                Console.WriteLine("What do you want to change?");
                Console.WriteLine("1-Datum");
                Console.WriteLine("2-Tijd");
                Console.WriteLine("3-Tafel Nummer");
                Console.WriteLine("4-Restaurant Id");
                int changeOption = Convert.ToInt32(Console.ReadLine());
                if (changeOption == 1)
                {
                    bool correctData = true;
                    check = !check;
                    string newDate = "";
                    Console.WriteLine("Changing Datum");
                    Console.WriteLine("Your Current Date is: " + foundReservation.Date);

                    check = false;
                    while(!check)
                    {
                        Console.WriteLine("Which date do you want it to switch to?");
                        newDate = Console.ReadLine();
                        DateTime newDateTime = DateTime.Parse(newDate);

                        
                        if (newDateTime != foundReservation.Date.Date)
                        {
                            newDate += " " + foundReservation.Date.TimeOfDay;

                            string q = $"SELECT * FROM [reservering] WHERE datum='{newDate}'";

                            List<int> restaurantIds = new List<int>();
                            List<int> tafelNummers = new List<int>();
                            List<DateTime> datums = new List<DateTime>();

                            try
                            {
                                SqlConnection con = new SqlConnection("Data Source=luxefood.database.windows.net;Initial Catalog=LuxeFoods;User ID=Klees;Password=Johnny69;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                                con.Open();
                                if (con.State == System.Data.ConnectionState.Open)
                                {

                                    SqlCommand cmd = new SqlCommand(q, con);

                                    cmd.ExecuteNonQuery();


                                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                                    DataTable dt = new DataTable();
                                    da.Fill(dt);
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        datums.Add(Convert.ToDateTime(dr["datum"].ToString()));
                                        restaurantIds.Add(Convert.ToInt32(dr["restaurantId"].ToString()));
                                        tafelNummers.Add(Convert.ToInt32(dr["tafelNummer"].ToString()));
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            int count = 0;

                            foreach (int x in restaurantIds)
                            {
                                if (x == foundReservation.restaurantId && tafelNummers[count] == foundReservation.tableNr && foundReservation.Date.TimeOfDay == datums[count].TimeOfDay)
                                {
                                    Console.WriteLine("This place has already been taken");
                                    correctData = false;
                                }
                                count++;
                            }
                            check = !check;
                        }
                        else
                        {
                            Console.WriteLine("You entered the same date!");
                        }
                    }

                    if (correctData)
                    {
                        try
                        {
                            SqlCommand Command = new SqlCommand("UPDATE reservering SET datum='" + newDate + "' WHERE Id='" + foundReservation.Id + "'", connection);
                            connection.Open();
                            Command.ExecuteNonQuery();

                            connection.Close();
                            Console.WriteLine("Succesfully Changed the date to " + newDate);
                        } catch (Exception error)
                        {
                            Console.WriteLine(error);
                        }   
                    }
                }
                else if (changeOption == 2)
                {
                    bool correctData = true;
                    check = !check;
                    string newTime = "";
                    string dateString = "";
                    Console.WriteLine("Changing Tijd");
                    Console.WriteLine("Your Current DateTime is: " + foundReservation.Date);

                    check = false;
                    while (!check)
                    {
                        Console.WriteLine("Which time do you want it to switch to? just put in the Hour");
                        int hour = 0;
                        while (hour < 16 || hour > 23)
                        {
                            newTime = Console.ReadLine();

                            hour = Convert.ToInt32(newTime);
                        }

                        TimeSpan newTimeSpan = new TimeSpan(hour, 0, 0);


                        //date += " " + ts;
                        //parsedDate = DateTime.Parse(date);


                        if (newTimeSpan != foundReservation.Date.TimeOfDay)
                        {
                            dateString = foundReservation.Date.Year + "-" + foundReservation.Date.Month + "-" + foundReservation.Date.Day;
                            dateString += " " + newTimeSpan;

                            DateTime dateDateTime = DateTime.Parse(dateString);
                            

                            string q = $"SELECT * FROM [reservering] WHERE datum='{dateString}'";

                            List<int> restaurantIds = new List<int>();
                            List<int> tafelNummers = new List<int>();
                            List<DateTime> datums = new List<DateTime>();

                            try
                            {
                                SqlConnection con = new SqlConnection("Data Source=luxefood.database.windows.net;Initial Catalog=LuxeFoods;User ID=Klees;Password=Johnny69;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                                con.Open();
                                if (con.State == System.Data.ConnectionState.Open)
                                {

                                    SqlCommand cmd = new SqlCommand(q, con);

                                    cmd.ExecuteNonQuery();


                                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                                    DataTable dt = new DataTable();
                                    da.Fill(dt);
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        datums.Add(Convert.ToDateTime(dr["datum"].ToString()));
                                        restaurantIds.Add(Convert.ToInt32(dr["restaurantId"].ToString()));
                                        tafelNummers.Add(Convert.ToInt32(dr["tafelNummer"].ToString()));
                                    }
                                }
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            int count = 0;

                            foreach (int x in restaurantIds)
                            {
                                if (x == foundReservation.restaurantId && tafelNummers[count] == foundReservation.tableNr && dateDateTime.Date.TimeOfDay == datums[count].TimeOfDay)
                                {
                                    Console.WriteLine("This place has already been taken");
                                    correctData = false;
                                }
                                count++;
                            }
                            check = !check;
                        }
                        else
                        {
                            Console.WriteLine("You entered the same date!");
                        }
                    }

                    if (correctData)
                    {
                        try
                        {
                            SqlCommand Command = new SqlCommand("UPDATE reservering SET datum='" + dateString + "' WHERE Id='" + foundReservation.Id + "'", connection);
                            connection.Open();
                            Command.ExecuteNonQuery();

                            connection.Close();
                            Console.WriteLine("Succesfully Changed the date to " + dateString);
                        }
                        catch (Exception error)
                        {
                            Console.WriteLine(error);
                        }
                    }


                    //Stay at current restaurant id
                    //Stay at the same date
                    //Stay at the same table number
                }
                else if (changeOption == 3)
                {
                    Console.WriteLine("Changing Tafel Nummer");
                    //Stay at the same restaurant id
                    //Stay at the same time
                    //Stay at the same date
                }
                else if (changeOption == 4)
                {
                    Console.WriteLine("Changing Restaurant");
                    //Stay at the same time
                    //Stay at the same date
                    //Stay at the same table number
                }
                else
                {
                    Console.WriteLine("Enter a valid option");
                }
            }
        }
    }
}