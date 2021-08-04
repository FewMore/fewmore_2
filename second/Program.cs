using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;


namespace WeatherApp
{

    class Program
    {
        const string key = "528ee0cd30451ccddd22c47235ae7185";
        const string language = "ru";
        
        
        static void Main(string[] args)
        {

            
            string filename = DateTime.Now.ToString("ddMMyyyyhhmm") + ".txt";

            while (true)
            {
                try
                {
                    while (true)
                    {

                        Console.Write("Введите название города для получения данных о текущей погоде в нем.\n" +
                           "Введите Q для завершения работы программы.\n\nВвод: ");
                        var name_of_city = Console.ReadLine();
                        if (name_of_city == "Q")
                            Environment.Exit(0);
                        Console.WriteLine();
                        Console.WriteLine(GetWeather(name_of_city));
                        using (StreamWriter stream = new StreamWriter(filename))
                            stream.WriteLine(GetWeather(name_of_city));
                        Console.ReadKey();
                        Console.Clear();

                    }
                }


                catch (Exception)
                {
                    Console.WriteLine("Не получилось отобразить запрашиваемый город."
                    + "\nВозможные причины: \n" + "Неправильно указано название города\n" + "Нет доступа к интернету\n");
                }
            }
           

            static string GetWeather(string cityName)
            {

                using (var client = new HttpClient()) //system.Net.Http;
                {
                    client.BaseAddress = new Uri("http://api.openweathermap.org/");
                    client.DefaultRequestHeaders.Accept.Clear();


                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //using system.net.http.headers

                    HttpResponseMessage response = client.GetAsync("data/2.5/weather?q=" + cityName + ",&appid=" + key + "&lang=" + language).Result;

                    RootObject data = JsonConvert.DeserializeObject<RootObject>(response.Content.ReadAsStringAsync().Result);

                   

                    string currentTemp = Math.Round((data.main.temp - 273.15)).ToString() + "°"; //преобразоание в градусы из кельвинов
                    string name = data.name;
                    string humidity = data.main.humidity.ToString();

                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(1000);

                    string sunrise = dateTimeOffset.AddSeconds(data.sys.sunrise).ToLocalTime().ToString();
                    string sunset = dateTimeOffset.AddSeconds(data.sys.sunset).ToLocalTime().ToString();

                    string result = $"Погода для города {name}:\n" +
                                   $"Температура: {currentTemp}\n" +
                                   $"Влажность:{humidity}%\n" +
                                   $"Восход солнца:{sunrise}%\n" +
                                   $"Закат солнца:{sunset}%\n";



                    return result;

                }
            }
           
        }

    }

}