using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using System;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        
        private readonly string googleApiKey = "AIzaSyAP8kakZfEGQoq4Zc0QsFn8qoTP-lrUezM"; // Replace with your API key
        private readonly string apiKey = "a043be7af5mshca4a3945fd92576p19bccdjsnb13a7923e5ca";
        private string cityName;
        private bool isCelsius = true;
        private bool isFahrenheit = false;
        private WeatherData weatherData;
        private List<City> matchingCities;
        private string previousInput = "";
        private GoogleMapsService googleMapsService;

        public bool IsCelsius
        {
            get { return isCelsius; }
            set
            {
                if (value != isCelsius)
                {
                    isCelsius = value;
                    OnPropertyChanged(nameof(IsCelsius));
                    UpdateTemperatureDisplay();
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isCelsius = true;
            isFahrenheit = false;
            UpdateTemperatureDisplay();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            isCelsius = false;
            isFahrenheit = true;
            UpdateTemperatureDisplay();
        }



        private DateTime _now;
        public DateTime Now
        {
            get { return _now; }
            set
            {
                _now = value;
                OnPropertyChanged(nameof(Now));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            lblDigitalClock.Visibility = Visibility.Hidden;
            lblCityName.Visibility = Visibility.Hidden;
            lblCondition.Visibility = Visibility.Hidden;
            lblDigitalClock.Visibility = Visibility.Hidden;
            lblHumidity.Visibility = Visibility.Hidden;
            humidImage.Visibility = Visibility.Hidden;
            lblPressure_mb.Visibility = Visibility.Hidden;
            pressImage.Visibility = Visibility.Hidden;
            lblTemperature.Visibility = Visibility.Hidden;
            tempImage.Visibility = Visibility.Hidden;
            lblUV.Visibility = Visibility.Hidden;
            uvImage.Visibility = Visibility.Hidden;
            lblWindSpeed.Visibility = Visibility.Hidden;
            windImage.Visibility = Visibility.Hidden;
            midBorder.Visibility = Visibility.Hidden;

            // Initialize Google Maps Service with API key
            googleMapsService = new GoogleMapsService(googleApiKey);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task GetWeatherDataAsync(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))     //Step 1.
            {
                MessageBox.Show("Please enter a city name!");
                return;
            }

            // Get the weather data from the API and deserialize    //Step 2.
            string apiUrl = $"https://weatherapi-com.p.rapidapi.com/current.json?q={cityName}";     
            //Call the API
            using (var client = new HttpClient())           //Step 3.
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(apiUrl),
                };
               
                // Add the required headers
                request.Headers.Add("X-RapidAPI-Key", apiKey);
                request.Headers.Add("X-RapidAPI-Host", "weatherapi-com.p.rapidapi.com");

                try
                {
                    using (var response = await client.SendAsync(request))  //Step 4
                    {
                        if (response.IsSuccessStatusCode)       //Step 5
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonResponse); // Assign data to weatherData
                            DisplayWeatherData(weatherData);
                        }
                        else //Step 6
                        {
                            // Handle API request failure
                            string errorResponse = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("API request failed:\nLocation not available, try again!\n" +  errorResponse);
                        }
                    }
                }
                catch (HttpRequestException ex) 
                {
                    // Handle exception (e.g., network error)
                    MessageBox.Show("An error occurred: " + ex.Message);
                }

                lblDigitalClock.Visibility = Visibility.Visible;
                lblCityName.Visibility = Visibility.Visible;
                lblCondition.Visibility = Visibility.Visible;
                lblDigitalClock.Visibility = Visibility.Visible;
                lblHumidity.Visibility = Visibility.Visible;
                humidImage.Visibility = Visibility.Visible;
                lblPressure_mb.Visibility = Visibility.Visible;
                pressImage.Visibility = Visibility.Visible;
                lblTemperature.Visibility = Visibility.Visible;
                tempImage.Visibility = Visibility.Visible;
                lblUV.Visibility = Visibility.Visible;
                uvImage.Visibility = Visibility.Visible;
                lblWindSpeed.Visibility = Visibility.Visible;
                windImage.Visibility = Visibility.Visible;
                midBorder.Visibility = Visibility.Visible;
                txtCityName.Text = "";
            }
        }
        private async void btnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            cityName = txtCityName.Text.Trim();
            await GetWeatherDataAsync(cityName);
        }

        private void DisplayWeatherData(WeatherData weatherData)
        {
            //Link the data to the labels and display the data
            lblCityName.Content = weatherData.Location.Name;
            lblTemperature.Content = weatherData.Current.TempC + "°C";
            lblCondition.Content = weatherData.Current.Condition.Text;
            lblHumidity.Content = weatherData.Current.Humidity + "%";
            lblUV.Content = weatherData.Current.Uv + " UV";
            lblPressure_mb.Content = weatherData.Current.Pressure_mb + " mb";

            Now = weatherData.Location.LocalTime;

            BitmapImage weatherIcon = new BitmapImage();
            weatherIcon.BeginInit();
            weatherIcon.UriSource = new Uri("http:" + weatherData.Current.Condition.Icon);
            weatherIcon.EndInit();
            imgWeatherIcon.Source = weatherIcon;

            lblWindSpeed.Content = weatherData.Current.WindKph + " km/h";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private void UpdateTemperatureDisplay()
        {
            if (weatherData != null)
            {
                if (isCelsius)
                {
                    // Update temperature display in Celsius
                    lblTemperature.Content = $"{weatherData.Current.TempC} °C";
                    lblWindSpeed.Content = $"{weatherData.Current.WindKph} km/h";
                }
                else if (isFahrenheit)
                {
                    // Update temperature display in Fahrenheit
                    lblTemperature.Content = $"{weatherData.Current.TempF} °F";
                    lblWindSpeed.Content = $"{weatherData.Current.WindMph} mph";
                }
            }
        }

        private async void txtCitySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userInput = txtCityName.Text.Trim();

            // Check if the input length is at least 3 characters and has changed
            if (userInput.Length >= 3 && userInput != previousInput)
            {
                previousInput = userInput;

                // Delay for a short time (e.g., 300 milliseconds) to allow the user to finish typing
                await Task.Delay(300);

                // Check if the input hasn't changed while waiting
                if (userInput == txtCityName.Text.Trim())
                {
                    // Provide a valid language code here, such as "en" for English
                    string languageCode = "en";

                    List<string> suggestions = await googleMapsService.GetAddressSuggestionsAsync(userInput, languageCode);

                    if (suggestions.Count > 0)
                    {
                        cmbMatchingCities.ItemsSource = suggestions;
                        cmbMatchingCities.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        cmbMatchingCities.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private async void cmbMatchingCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMatchingCities.SelectedItem is string selectedSuggestion)
            {
                // Update the main weather display with data for the selected city.
                await GetWeatherDataAsync(selectedSuggestion);
                UpdateTemperatureDisplay();

                cmbMatchingCities.Visibility = Visibility.Collapsed;
            }
        }
    }
}
