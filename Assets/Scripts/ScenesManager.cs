using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private const string METEOROLOGY = "MeteorologyScene";
    private const string MEDIA  = "MediaScene";
    private const string CUBE = "SortCubeScene";
    private const string MENU = "MenuScene";

    public void LoadMeteorologyScene()
    {
        SceneManager.LoadScene(METEOROLOGY);
    }

    public void LoadMediaScene()
    {
        SceneManager.LoadScene(MEDIA);
    }

    public void LoadCubeSorterScene()
    {
        SceneManager.LoadScene(CUBE);
    }
    
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(MENU);
    }
}
