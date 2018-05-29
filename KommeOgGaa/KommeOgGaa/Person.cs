using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace KommeOgGaa
{
    class Person
    {
        public long Ticks { get; set; }
        public int Index { get; set; }
        public int Link { get; set; }
        public string PicturesLocation { get; set; }

        public string Time { get { return new DateTime(Ticks).ToLongTimeString();  } }
        public long Date
        {
            get
            {
                var dt = new DateTime(Ticks);
                return new DateTime(dt.Year, dt.Month, dt.Day).Ticks;
            }
        }
                
        public Person Relation { get; set; }

        public static List<Person> GetAllCheckInPersons()
        {
            var persons = new List<Person>();

            string[] images = { };// Directory.GetFiles(Directory.GetCurrentDirectory()+@"\Billeder til dansk");

            Random r = new Random();
            foreach (var path in images)
            {

                int date = r.Next(20, 23);

                var checkIn = new Person()
                {
                    PicturesLocation = path,
                };
                int id = r.Next(0, 3);
                switch (id)
                {
                    case 0: checkIn.Ticks = new DateTime(2018,05,22, 8,0,0).Ticks;  break;
                    case 1: checkIn.Ticks = new DateTime(2018, 04, 21, 9, 0, 0).Ticks; break;
                    case 2: checkIn.Ticks = new DateTime(2018, 05, 20, 7, 0, 0).Ticks; break;
                    case 3: checkIn.Ticks = new DateTime(2017, 05, 22, 8, 0, 0).Ticks; break;
                }


                var checkOut = new Person()
                {
                    PicturesLocation = path,
                    Relation = checkIn
                };
                switch (id)
                {
                    case 0: checkOut.Ticks = new DateTime(2018, 05, 22, 14, 0, 0).Ticks; break;
                    case 1: checkOut.Ticks = new DateTime(2018, 04, 21, 15, 0, 0).Ticks; break;
                    case 2: checkOut.Ticks = new DateTime(2018, 05, 20, 13, 0, 0).Ticks; break;
                    case 3: checkOut.Ticks = new DateTime(2017, 05, 22, 13, 0, 0).Ticks; break;
                }
                checkIn.Relation = checkOut;

                persons.Add(checkIn);
            }

            return persons;
        } 
    }
}
