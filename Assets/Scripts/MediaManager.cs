using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.IO;

public class MediaManager : MonoBehaviour
{
    [SerializeField] private string _imageUrl;
    [SerializeField] private string _videoUrl;
    [SerializeField] private RawImage _imageDisplay;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private GameObject _videoLoadingPanel;
    [SerializeField] private GameObject _imageLoadingPanel;

    public void StartDownload()
    {
        StartCoroutine(DownloadImage());
        StartCoroutine(DownloadVideo());
    }

    /// <summary>
    /// Downloads an image from the specified URL and displays it in a RawImage component
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadImage()
    {
        // Set the loading panel to active to show that the image is being downloaded
        _imageLoadingPanel.SetActive(true);

        // Send a web request to the specified URL to get the image texture
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(_imageUrl))
        {
            yield return request.SendWebRequest();// Wait until the request is completed

             // Check if the request was successful and handle any errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // Get the downloaded texture and set it as the texture for the RawImage component
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                _imageDisplay.texture = texture;
                _imageDisplay.gameObject.SetActive(true);

                // Set the alpha value of the RawImage to 1 to make it fully opaque
                Color color = _imageDisplay.color;
                color.a = 1f; 
                _imageDisplay.color = color;
            }
        }

        // Set the loading panel to inactive to hide it once the image is downloaded and displayed
        _imageLoadingPanel.SetActive(false);
    }

    /// <summary>
    /// Coroutine that downloads a video file and saves it to the device's persistent data path
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadVideo()
    {
        // Show loading panel while the video is being downloaded
        _videoLoadingPanel.SetActive(true);

        // Create a UnityWebRequest to download the video from the specified URL
        UnityWebRequest _videoRequest = UnityWebRequest.Get(_videoUrl);

        // Send the request and wait for it to finish
        yield return _videoRequest.SendWebRequest();

        // Check if the request has finished successfully
        if (_videoRequest.isDone == false || _videoRequest.error != null)
        {
            Debug.Log("Request = " + _videoRequest.error);
        }

        // Get the downloaded video as a byte array
        byte[] _videoBytes = _videoRequest.downloadHandler.data;

        // Set the path to save the video to
        string _pathToFile = Path.Combine(Application.persistentDataPath, "movie.mp4");

        // Write the video bytes to the specified path on the device        
        File.WriteAllBytes(_pathToFile, _videoBytes);

        // Start playing the downloaded video
        StartCoroutine(this.playThisURLInVideo(_pathToFile));


        // Hide the loading panel now that the video is downloaded and playing  
        _videoLoadingPanel.SetActive(false);
    }

    /// <summary>
    /// Coroutine to play a video given its URL
    /// </summary>
    /// <param name="_url"></param>
    /// <returns></returns>
    private IEnumerator playThisURLInVideo(string _url)
    {
        // Set the source and URL of the video player to the given URL
        _videoPlayer.source = UnityEngine.Video.VideoSource.Url;
        _videoPlayer.url = _url;
        _videoPlayer.Prepare();

        // Wait until the video player is prepared before starting playback
        while (_videoPlayer.isPrepared == false)
        {
            yield return null;
        }

        // Once the video player is prepared, set it to loop and start playing the video
        _videoPlayer.isLooping = true;
        _videoPlayer.Play();
    }
}
