using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private GameObject _hideObjects;
    [SerializeField] private TMP_Dropdown _location;
    [SerializeField] private  TextMeshProUGUI _city;
    [SerializeField] private  TextMeshProUGUI _currentTemperature;
    [SerializeField] private  TextMeshProUGUI _maxTemperature;
    [SerializeField] private  TextMeshProUGUI _minTemperature;
    [SerializeField] private  TextMeshProUGUI _description;    
    [SerializeField] private  TextMeshProUGUI _clockText;
    [SerializeField] private  TextMeshProUGUI _errorText;
    [SerializeField] private  GameObject _weatherIcon;
    [SerializeField] private GameObject _clearSkyPrefab;
    [SerializeField] private GameObject _fewCloudsPrefab;
    [SerializeField] private GameObject _scatteredCloudsPrefab;
    [SerializeField] private GameObject _brokenCloudsPrefab;
    [SerializeField] private GameObject _showerRainPrefab;
    [SerializeField] private GameObject _rainPrefab;
    [SerializeField] private GameObject _thunderstormPrefab;
    [SerializeField] private GameObject _snowPrefab;
    [SerializeField] private GameObject _mistPrefab;
    [SerializeField] private GameObject _errorPopup;
    // API key to use for accessing weather data
    private const string API_KEY = "d14443c053f8af72324b73b898e4dbbb"; 
    
    //Animation names for reference
    private const string SHOWER_ANIM = "ShowerRainAnimation"; 
    private const string RAIN_ANIM = "RainAnimation"; 

    // Animators for some of the weather icons
    private Animator _showerRainAnimator;
    private Animator _rainAnimator;


    void Start()
    {        
        StartCoroutine(UpdateClockCoroutine());
    }

    private IEnumerator UpdateClockCoroutine()
    {
        while (true)
        {
            UpdateClock();
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void UpdateClock()
    {
        if (_clockText != null) 
        {
            //Convert Time to dd-MM-yy HH:mm:ss format
            _clockText.text = DateTime.Now.ToString("dd-MM-yy HH:mm:ss");
        }
    }

    // Method called when the location dropdown is changed
    public void OnLocationDropdownChanged()
    {
        // Get the city ID for the selected location
        int cityId = GetCityId(_location.value);

        // If a valid city ID was found, start the coroutine to get weather data for that city
        if(cityId!= 0)StartCoroutine(GetWeatherData(cityId));
    }

    // Method to get the city ID for a selected dropdown value
    private int GetCityId(int dropdownValue)
    {
        int cityId = 0;

        // Use a switch statement to set the city ID based on the dropdown value
        switch (dropdownValue)
        {
            case 0:
                cityId = 0; // None
                break;
            case 1:
                cityId = 2267056; // Lisbon
                break;
            case 2:
                cityId = 2267094; // Leiria
                break;
            case 3:
                cityId = 2740636; // Coimbra
                break;
            case 4:
                cityId = 2735941; // Porto
                break;
            case 5:
                cityId = 2268337; //
                break;
        }
        return cityId;
    }

    /// <summary>
    /// Retrieves weather data for a given city from the OpenWeatherMap API.
    /// </summary>
    /// <param name="cityId"> The name of the city to retrieve weather data for.</param>
    /// <returns></returns>
    IEnumerator GetWeatherData(int cityId)
    {
        // Set the API endpoint URL with the provided API key and the city name parameter
        string url = "http://api.openweathermap.org/data/2.5/weather?id=" + cityId + "&units=metric&appid=" + API_KEY;
        HttpWebRequest request = null;
        HttpWebResponse response = null;

        try
        {
            // Create a new HTTP request and set the timeout to 5 seconds
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000; // 5 seconds
             // Get the HTTP response from the API
            response = (HttpWebResponse)request.GetResponse();
            
        }
        catch (WebException ex)
        {
            // If an exception is thrown, check if it's a timeout error
            if (ex.Status == WebExceptionStatus.Timeout)
            {
                 // Handle timeout error by logging a message and stopping the coroutine
                HandleWeatherAPIError("The request to the weather API timed out. Please try again later.");
                yield break;
            }
            else
            {
                // If it's not a timeout error, handle other errors by logging the message
                HandleWeatherAPIError("There was an error connecting to the weather API. Please try again later." + ex.Message);
            }
        }

         // If the response was successful, read the response stream and parse the JSON data
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        JObject data = JObject.Parse(jsonResponse);
        float temperature = (float)data["main"]["temp"];
        string cityName = (string)data["name"]; 
        float tempMin = (float)data["main"]["temp_min"];
        float tempMax = (float)data["main"]["temp_max"];
        string description = (string)data["weather"][0]["description"];
        string iconCode = (string)data["weather"][0]["icon"];

        // Activate the hidden game objects
        if(!_hideObjects.activeSelf)_hideObjects.SetActive(true);

        // Update the UI with the retrieved weather data
        _city.text = "Weather for: " + cityName;
        _currentTemperature.text = "Current Weather: " + temperature.ToString("0") + "°C";
        _maxTemperature.text = "Maximum Weather: " + tempMax.ToString("0") + "°C";
        _minTemperature.text = "Minimum Weather: " + tempMin.ToString("0") + "°C";
        _description.text = description;

        // Load the weather icon for the current weather condition
        LoadIcon(iconCode);

        // Close the HTTP response and return null to end the coroutine
        response.Close();
        yield return null;
    }

    /// <summary>
    ///  loads the appropriate weather icon prefab
    /// </summary>
    /// <param name="iconCode"></param>
    private void LoadIcon(string iconCode)
    {
        // Declare a variable to hold the prefab to be instantiated
        GameObject prefab = null;

        // Use a switch statement to match the iconCode to the appropriate prefab
        switch(iconCode)
        {
            case "01d":
            case "01n":
                prefab = _clearSkyPrefab;
                break;
            case "02d":
            case "02n":
                prefab = _fewCloudsPrefab;
                break;
            case "03d":
            case "03n":
                prefab = _scatteredCloudsPrefab;
                break;
            case "04d":
            case "04n":
                prefab = _brokenCloudsPrefab;
                break;
            case "09d":
            case "09n":
                prefab = _showerRainPrefab;
                
                break;
            case "10d":
            case "10n":
                prefab = _rainPrefab;
                break;
            case "11d":
            case "11n":
                prefab = _thunderstormPrefab;
                break;
            case "13d":
            case "13n":
                prefab = _snowPrefab;
                break;
            case "50d":
            case "50n":
                prefab = _mistPrefab;
                break;
            default:
                // Handle unknown icon codes
                break;
        }

        // Instantiate the prefab if it's not null
        if (prefab != null)
        {
            // Instantiate the prefab and destroy the old one
            GameObject instantiatedPrefab = Instantiate(prefab, _weatherIcon.transform.position, Quaternion.identity, _weatherIcon.transform.parent);
            Destroy(_weatherIcon);
            _weatherIcon = instantiatedPrefab;
        }

        // Handle animations for certain weather conditions
        if(prefab == _showerRainPrefab)
        {
            _showerRainAnimator = _weatherIcon.GetComponent<Animator>();        
            _showerRainAnimator.Play(SHOWER_ANIM);
        }
        if(prefab == _rainPrefab)
        {
            _rainAnimator = _weatherIcon.GetComponent<Animator>();
            _rainAnimator.Play(RAIN_ANIM);
        }
    }

    public void HandleWeatherAPIError(string errorMessage)
    {
        // Display the error message to the user
        Debug.LogError(errorMessage);
        _errorPopup.SetActive(true);
        _errorText.text = errorMessage;
    }

    public void HideErrorPopup()
    {
        _errorPopup.SetActive(false);
    }
}
