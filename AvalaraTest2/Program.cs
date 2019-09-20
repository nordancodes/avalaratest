using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Application
{
    class predictPrecipitation
    {
        
        static void Main(string[] args)
        {

            /*
             * Use SteamReader class to open local csv file. While
             * there are remaining lines in the dataset, split data and assign
             * relative fields within the List of precipDays.
             */
            
            using (StreamReader reader = new StreamReader(File.OpenRead(@"27612-precipitation-data.csv")))
            {
                List<precipDay> days = new List<precipDay>();
                while (!reader.EndOfStream)
                {
                    precipDay Day = new precipDay();
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    //Throw out inconsistently structured data in the set.
                    if(values.Length == 8)
                    {
                        Day.Station = values[0];
                        Day.Name = values[1] + values[2];
                        Day.Latitude = values[3];
                        Day.Longitude = values[4];
                        Day.Elevation = values[5];
                        Day.Date = values[6];
                        Double number = 0.0;
                        Boolean isDouble = Double.TryParse(values[7], out number);
                        if(isDouble)
                        {
                           Day.precipitation = number;
                        }
                        else
                        {
                            Day.precipitation = 0.0;
                        }

                        days.Add(Day);
                    }
                   

                }

                //Output user instructions to console.
                Console.WriteLine("What date would you like to predict precipitation for?(Format: M/D/YYYY)");
                Console.WriteLine("For current date prediction, simply press 'Enter'");

                string dateneeded = Console.ReadLine();


                //If no date entered, use current date as prediction data.
                if(dateneeded == "")
                {
                    dateneeded = DateTime.Today.ToString("d");
                }

                string monthneeded = dateneeded.Split('/')[0];
                double result;
                double monthtotal = 0.0;
                double samples = 0;
                



                /* Loop through data to find predicted average. Due to missing days
                 * in the dataset, we find averages for the predicted month since at least all
                 * months have data.
                 */
                
                foreach (precipDay day in days)
                {
                    if (day.Date.Split('/')[0] == monthneeded) 
                    {
                        monthtotal += day.precipitation;
                        samples++;
                    }
                  
                }
                result = monthtotal / samples;

                /* Print results to console. If result is astray, we can assume
                 * requested date was incorrectly formatted.
                 */

                if(result.Equals(0) || double.IsNaN(result))
                {
                    Console.WriteLine("Date entered may have been incorrect format. Please enter MM/DD/YYYY");
                }

                predictAnswer answer = new predictAnswer(dateneeded, result);
                Console.WriteLine("Prediction for : " + answer.Date + " : " + answer.precip);

                //Json response of predicted rainfall.
                string json = JsonConvert.SerializeObject(answer);
            }





            }


       
    }

    //Class to hold data for each field in the dataset.
    class precipDay
    {
        public string Station { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Elevation { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public double precipitation { get; set; } = 0.0;



        public precipDay()
        {
            
        }
    }

    //Class to use for answer to make for easy json conversion.
    class predictAnswer
    {
        public string Date { get; set; } = string.Empty;
        public double precip { get; set; } = 0.0;
        public predictAnswer(string Date, double precip)
        {
            this.Date = Date;
            this.precip = precip;
        }
    }
}

