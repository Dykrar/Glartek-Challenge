using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CubeSpawn : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject[] unsortedColumnPositions;
    [SerializeField] private GameObject unsortedColumn;
    [SerializeField] private GameObject sortedColumn;
    [SerializeField] private GameObject[] sortedColumnPositions;
    public List<GameObject> unsortedCubes = new List<GameObject>();
    public List<GameObject> sortedCubes = new List<GameObject>();
    [SerializeField] private GameObject startSort;

    /// <summary>
    ///  This method is an IEnumerator which will execute as a coroutine and is called when the script first starts
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
        for (int i = 0; i < unsortedColumnPositions.Length; i++)
        {
            // Instantiate a new cubePrefab GameObject and set its color to a random color
            GameObject cube = Instantiate(cubePrefab,unsortedColumnPositions[i].transform);
            Color32 color = GetRandomColor();
            cube.GetComponent<Image>().color = color;

             // Set the scale and parent of the new cube, and add it to the list of unsortedCubes
            cube.transform.localScale =  new Vector3(1, 1, 1);
            cube.transform.SetParent(unsortedColumn.transform); 
            unsortedCubes.Add(cube);

            // Instantiate a new cubePrefab GameObject, set its color to the same color as the unsorted cube, and add it to the list of sortedCubes
            GameObject sortedCube = Instantiate(cubePrefab, sortedColumnPositions[i].transform);
            sortedCube.GetComponent<Image>().color = color;            
            sortedCube.transform.localScale =  new Vector3(1, 1, 1);
            sortedCube.transform.SetParent(sortedColumn.transform);
            sortedCubes.Add(sortedCube);

            // Wait one frame before continuing to the next iteration of the loop
            yield return null;
        }

        // Activate the startSorts button once all cubes have been instantiated
        startSort.SetActive(true);
    }

    /// <summary>
    ///  Returns a random Color Red, Gree or Blue
    /// </summary>
    /// <returns>A Color32 object representing a random color.</returns>
    private Color32 GetRandomColor()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                return new Color32(255, 0, 0, 255); // Red
            case 1:
                return new Color32(0, 255, 0, 255); // Green
            case 2:
                return new Color32(0, 0, 255, 255); // Blue
            default:
                return new Color32(255, 255, 255, 255); // White
        }
    }
    
}