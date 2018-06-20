using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace KommeOgGaa
{
    public class Person
    {
        public long Ticks { get; set; }
        public int Index { get; private set; }
        public int RelationID { get; set; }
        public string PicturesLocation { get; set; }
        public bool IsCheckIn { get; set; }
        public string Category { get; set; }
        public bool IsLate { get; set; }
        public string Time { get { return new DateTime(Ticks).ToLongTimeString();  } }
        
            
        public string PicturesLocationFull { get { return Directory.GetCurrentDirectory() + PicturesLocation; } }
        public long Date
        {
            get
            {
                var dt = new DateTime(Ticks);
                return new DateTime(dt.Year, dt.Month, dt.Day).Ticks;
            }
        }
                
        public Person Relation { get; private set; }

        public void Insert()
        {
            var key = SQLite_DB_LIB.Database.Insert("Persons",
                new string[] { "RelationID", "PicturesLocation", "Ticks", "Category", "IsCheckIn", "IsLate" },
                new object[] { -1, PicturesLocation, Ticks, Category, Convert.ToInt32(IsCheckIn), Convert.ToInt32(IsLate) });

            Index = Convert.ToInt32(key);
        }
        public void Update()
        {
            SQLite_DB_LIB.Database.Update("Persons", 
                new Dictionary<string, object>()
                {
                    { "RelationID", RelationID },
                    { "PicturesLocation", PicturesLocation },
                    { "Ticks", Ticks },
                    { "Category", Category },
                    { "IsCheckIn", Convert.ToInt32(IsCheckIn) },
                    { "IsLate", Convert.ToInt32(IsLate) },
                }, Index);
        }

        private static Person DatabaseDataToPerson(Dictionary<int,object> row)
        {
            var person = new Person();
            person.Index            = Convert.ToInt32(row[0]);
            person.RelationID       = Convert.ToInt32(row[1]);
            person.PicturesLocation = Convert.ToString(row[2]);
            person.Ticks            = Convert.ToInt64(row[3]);
            person.Category         = Convert.ToString(row[4]);
            person.IsCheckIn        = Convert.ToBoolean(row[5]);
            person.IsLate           = Convert.ToBoolean(row[6]);

            return person;
        }

        public static List<Person> GetAllCheckInPersons()
        {
            var checkInRows = SQLite_DB_LIB.Database.GetRows<int>("Persons", new string[] { "Index", "RelationID", "PicturesLocation", "Ticks", "Category", "IsCheckIn", "IsLate" }, "WHERE `IsCheckIn` = 1");

            return ConvertDatabaseDataToList(checkInRows);
        }

        public static List<Person> GetAllCheckInToday()
        {
            long start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0,0,0).Ticks;
            long end = new DateTime(start).AddDays(1).Ticks;
            var checkInRows = SQLite_DB_LIB.Database.GetRows<int>("Persons", new string[] { "Index", "RelationID", "PicturesLocation", "Ticks", "Category", "IsCheckIn", "IsLate" }, "WHERE `IsCheckIn` = 1 AND `RelationID` = -1 AND `Ticks` BETWEEN "+ start +" AND "+ end);

            
            return ConvertDatabaseDataToList(checkInRows);
        }

        private static List<Person> ConvertDatabaseDataToList(List<Dictionary<int, object>> rows)
        {
            var persons = new List<Person>();
            if (rows != null)
            {
                foreach (var checkIn in rows)
                {
                    var personIn = DatabaseDataToPerson(checkIn);

                    if (personIn.RelationID != -1)
                    {
                        var checkOut = SQLite_DB_LIB.Database.GetRow<int>("Persons", new string[] { "Index", "RelationID", "PicturesLocation", "Ticks", "Category", "IsCheckIn", "IsLate" }, "WHERE `Index` = " + personIn.RelationID);
                        var personOut = DatabaseDataToPerson(checkOut);

                        personIn.Relation = personOut;
                        personOut.Relation = personIn;
                    }


                    persons.Add(personIn);
                }
            }
            return persons;
        }

        public static void DeleteOldRecords(int days)
        {
            long time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(-days).Ticks;
            var rows = SQLite_DB_LIB.Database.GetRows<int>("Persons", new string[] { "PicturesLocation" }, " WHERE `Ticks` < " + time);
            List<string> errors = new List<string>();
            if (rows != null)
            {
                foreach (var item in rows)
                {
                    string filename = Directory.GetCurrentDirectory() + item[0];
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (Exception)
                    {
                        errors.Add(filename);
                    }
                }
                
            }

            SQLite_DB_LIB.Database.Delete("Persons", " WHERE `Ticks` < " + time);

            if (errors.Count > 0)
            {
                string logfolder = Directory.GetCurrentDirectory() + @"\Logs\";
                string logFile = Path.Combine(logfolder, DateTime.Now.ToShortDateString() + ".txt");

                if (!Directory.Exists(logfolder))
                {
                    Directory.CreateDirectory(logfolder);
                }

                using (FileStream stream = File.Create(logFile))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(DateTime.Now.ToShortTimeString()+ " Error: Billeder Ikke Slettet.\n");

                        foreach (var item in errors)
                        {

                            writer.WriteLine("  File: "+ item + "\n");
                        }
                        writer.WriteLine("\n\n");
                    }
                }

                System.Windows.MessageBox.Show("Ikke alle billeder er blevet slettet.\n Tjek " + logFile + " for mere info.", "Billeder Ikke Slettet!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
