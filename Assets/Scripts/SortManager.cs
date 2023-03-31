using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _redColumnPositions;
    [SerializeField] private GameObject[] _greenColumnPositions;
    [SerializeField] private GameObject[] _blueColumnPositions;

    [SerializeField] private GameObject _redColumn;
    [SerializeField] private GameObject _greenColumn;
    [SerializeField] private GameObject _blueColumn;

    // Reference to the CubeSpawn script and lists to hold the unsorted and sorted cubes
    [SerializeField] private CubeSpawn _cubeSpawn;
    private List<GameObject> _unsortedCubes;
    private List<GameObject> _sortedCubes;

    // Method called to start the sorting process
    public void StartSort(int i)
    {
        // Populate the unsortedCubes list
        _unsortedCubes = _cubeSpawn.unsortedCubes;
        _sortedCubes = _cubeSpawn.sortedCubes;

        // Start the sorting coroutine
        StartCoroutine(SortCubes(i));
    }

    // Coroutine that handles the sorting process
    private IEnumerator  SortCubes(int algorithm)
    {
        // Loop through the unsorted list of cubes
        for (int i = _unsortedCubes.Count -1; i >= 0; i--)
        {
            // Get a reference to the current cube
            GameObject cube = _unsortedCubes[i];           

            // Determine the color of the cube and move it to the corresponding column
            int colorIndex = GetColorIndex(cube);
            if (colorIndex == 0) // Red
            {
                yield return StartCoroutine(PrepareMovement(_redColumnPositions, cube, "red"));
            }
            else if (colorIndex == 1) // Green
            {
                yield return StartCoroutine(PrepareMovement(_greenColumnPositions, cube, "green"));
            }
            else if (colorIndex == 2) // Blue
            {
                yield return StartCoroutine(PrepareMovement(_blueColumnPositions, cube, "blue"));
            }
            // Wait for the cube movement to finish before continuing to the next cube
            yield return new WaitForSeconds(0.5f);
        }

         // Call the appropriate sorting algorithm based on the input
        switch(algorithm)
        {
            case 1:
            StartCoroutine(StartBubbleSort());
            break;

            case 2:
            StartCoroutine(StartSelectionSort());
            break;
        } 
    }

    // Coroutine that handles the selection sort algorithm
    private IEnumerator StartSelectionSort()
    {
         // Sort the Sorted column by color using Selection Sort
        for (int i = 0; i < _sortedCubes.Count - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < _sortedCubes.Count; j++)
            {
                if (GetColorIndex(_sortedCubes[j]) < GetColorIndex(_sortedCubes[minIndex]))
                {
                    minIndex = j;
                }
            }
            if (minIndex != i)
            {
                // Swap the positions of the cubes in the Sorted column
                GameObject temp = _sortedCubes[i];
                _sortedCubes[i] = _sortedCubes[minIndex];
                _sortedCubes[minIndex] = temp;

                var firstPosition = _sortedCubes[i].transform.position;
                var SecondPosition = _sortedCubes[minIndex].transform.position;

                // Move the cubes in the scene to their new positions
                yield return StartCoroutine(MoveCube(_sortedCubes[i].transform, SecondPosition, 0.1f));
                yield return StartCoroutine(MoveCube(_sortedCubes[minIndex].transform, firstPosition, 0.1f));
            }
        }
    }

    // Coroutine that handles the bubble sort algorithm
    private IEnumerator StartBubbleSort()
    {
        // Sort the Sorted column by color using Bubble Sort
        for (int i = 0; i < _sortedCubes.Count - 1; i++)
        {
            for (int j = 0; j < _sortedCubes.Count - i - 1; j++)
            {
                if (GetColorIndex(_sortedCubes[j]) > GetColorIndex(_sortedCubes[j + 1]))
                {
                    // Swap the positions of the cubes in the Sorted column
                    GameObject temp = _sortedCubes[j];
                    _sortedCubes[j] = _sortedCubes[j + 1];
                    _sortedCubes[j + 1] = temp;

                    var firstPosition = _sortedCubes[j].transform.position;
                    var secondPosition = _sortedCubes[j + 1].transform.position;

                    // Move the cubes in the scene to their new positions
                    yield return StartCoroutine(MoveCube(_sortedCubes[j].transform, secondPosition, 0.1f));
                    yield return StartCoroutine(MoveCube(_sortedCubes[j + 1].transform, firstPosition, 0.1f));
                }
            }
        }
    }

    /// <summary>
    /// Updates the positions of the target columns
    /// </summary>
    /// <param name="colorList">an array of GameObject representing the positions of cubes of a specific color, </param>
    /// <param name="cube">the cube to be moved</param>
    /// <param name="color"> color of the cube </param>
    /// <returns></returns>
    private IEnumerator PrepareMovement(GameObject[] colorList, GameObject cube, string color)
    {
        // Wait until the cube has finished moving to the first position in the colorList
        yield return StartCoroutine(MoveCube(cube.transform, colorList[0].transform.position, 0.2f));

        // Create a new array with one less element than colorList and copy the remaining elements into it
        GameObject[] newArr = new GameObject[colorList.Length - 1];
        Array.Copy(colorList, 1, newArr, 0, newArr.Length);

        // Update the appropriate column position array based on the color of the cube
        switch(color)
        {
            case "red":
            _redColumnPositions = newArr;
            cube.transform.SetParent(_redColumn.transform);
            break;

            case "green":
            _greenColumnPositions = newArr;            
            cube.transform.SetParent(_greenColumn.transform);
            break;

            case "blue":
            _blueColumnPositions = newArr;            
            cube.transform.SetParent(_blueColumn.transform);
            break;
        }       
    }

    /// <summary>
    /// a coroutine function that moves a cube's transform to a target position over a duration of time
    /// </summary>
    /// <param name="cubeTransform"> the Transform component of the cube GameObject</param>
    /// <param name="targetPosition">the target position where the cube should move towards</param>
    /// <param name="duration">the duration of time over which the cube should mov</param>
    /// <returns></returns>
    private IEnumerator MoveCube(Transform cubeTransform, Vector3 targetPosition, float duration)
    {
        // Initialize elapsed time and start position
        float elapsedTime = 0;
        Vector3 startPosition = cubeTransform.position;
        
        // While the elapsed time is less than the duration, move the cube towards the target position using Lerp
        while (elapsedTime < duration)
        {
            cubeTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }        
        cubeTransform.position = targetPosition;
    }   

    /// <summary>
    /// returns the index of the color of the cube in the colorList array
    /// </summary>
    /// <param name="cube">the cube to be moved</param>
    /// <returns>index of the color of the cube</returns>
    private int GetColorIndex(GameObject cube)
    {
        // Get the color of the cube as a Color32 object
        Color32 color = cube.GetComponent<Image>().color;
        // Check if the color is red, green, or blue and return the corresponding index
        if (color.Equals(new Color32(255, 0, 0, 255))) // Red
        {
            return 0;
        }
        else if (color.Equals(new Color32(0, 255, 0, 255))) // Green
        {
            return 1;
        }
        else if (color.Equals(new Color32(0, 0, 255, 255))) // Blue
        {
            return 2;
        }
        else // Unknown color
        {
            return -1;
        }
    }
}