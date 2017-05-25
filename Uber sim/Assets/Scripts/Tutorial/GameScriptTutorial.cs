using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScriptTutorial : MonoBehaviour, IGameController {
    
    public float gameTimeLimit = 1;
    public GameObject[] aiCars;

    [Header("Spawnpoint")]
    public int customerCount = 3;
    public GameObject spawnpoint; //Spawnpoint parent GameObject
    private Transform[] spawnpoints;
    private int[] usedSpawnpoints;

    [Header("HUD")]
    public GameObject gameHUD;
    public GameObject[] navCameras;
    public GameObject pauseMenu;

    [Header("Customer")]
    public Transform customerPrefab;
    public Transform destinationPrefab;
    public float killPoints = 0;

    private Vector3 spawnPosition;
    private Vector3 destination;
    private bool paused = false;
    private int navTemp = 0;
    private AudioManager audioManager;
    private int tempSpawn = -1;
    private int customerType;

    public Transform[] DestinationObjects { get; private set; }
    public float GameTimeLimit
    {
        get
        {
            return gameTimeLimit;
        }
    }
    public float KillPoints { get { return killPoints; } }

    private void Awake()
    {
        //get spawnpoints
        spawnpoints = new Transform[spawnpoint.transform.childCount];
        usedSpawnpoints = new int[customerCount + 1];
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        for (int i = 0; i < spawnpoint.transform.childCount; i++)
        {
            spawnpoints[i] = spawnpoint.transform.GetChild(i);
        }
        Debug.Log("Spawnpoints: " + spawnpoints.Length);
    }

    private void Start()
    {
        DestinationObjects = new Transform[] { null, null, null, null };
        for (int i = 0; i < usedSpawnpoints.Length; i++)
        {
            usedSpawnpoints[i] = -1;
        }
        ResumeGame();
        for (int i = 0; i < customerCount; i++)
        {
            SpawnCustomer(-1); // spawn first customers
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        audioManager.paused(false);
        Time.timeScale = 1;
        paused = false;
        Debug.Log("Unpaused");
    }

    private void Update()
    {
        //pause
        if (Input.GetButtonDown("Cancel")) //default escape
        {
            if (!paused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        audioManager.paused(true);
        Time.timeScale = 0;
        paused = true;
        Debug.Log("Paused");
    }

    public void EndGame()
    {
        return;
    }

    public void SubmitName(InputField input)
    {
        return;
    }

    public void OnInputValueChange(InputField input)
    {
        return;
    }

    private void ClearSpawnpoint(int index)
    {
        tempSpawn = usedSpawnpoints[index];
        usedSpawnpoints[index] = -1;
    }

    public void SpawnCustomer(int spawnIndex)
    {
        // check if empty spawn
        bool full = true;
        int spawnListIndex = -1;
        for (int i = 0; i < usedSpawnpoints.Length - 1; i++)
        {
            if (usedSpawnpoints[i] == spawnIndex)
            {
                spawnListIndex = i;
                ClearSpawnpoint(spawnListIndex);
                full = false;
                break;
            }
        }
        if (full)
        {
            Debug.Log("No empty spawnpoints");
            return;
        }

        // Get empty spawnpoint
        int random;
        do
        {
            random = UnityEngine.Random.Range(0, spawnpoints.Length);
        } while (ArrayContains(usedSpawnpoints, random));

        //spawn
        Vector3 position = spawnpoints[random].transform.position;
        Transform customer = Instantiate(customerPrefab, position, Quaternion.identity);
        customer.GetComponent<CustomerScript>().InitCustomer((CustomerType)customerType, random);
        NextCustomerType();

        //set used spawn
        usedSpawnpoints[spawnListIndex] = random;
        DestinationObjects[spawnListIndex] = customer;
    }

    private void NextCustomerType()
    {
        customerType++;
        if (customerType > 3) { customerType = 0; }
    }

    public void spawnDestination()
    {
        // Get empty spawnpoint
        int random;
        do
        {
            random = UnityEngine.Random.Range(0, spawnpoints.Length);
        } while (ArrayContains(usedSpawnpoints, random));

        //spawn
        Vector3 position = spawnpoints[random].transform.position;
        Transform destination = Instantiate(destinationPrefab, position, Quaternion.identity);

        //set used spawn
        usedSpawnpoints[customerCount] = random;
        DestinationObjects[customerCount] = destination;
    }

    private bool ArrayContains(int[] array, int number)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == number)
            {
                return true;
            }
        }
        if (tempSpawn == number) { return true; }
        return false;
    }

    public void AddPoints(float points)
    {
        return;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeNavigatorStyle()
    {
        navCameras[navTemp].SetActive(false);
        navTemp++;
        if (navTemp > navCameras.Length - 1)
        {
            navTemp = 0;
        }
        Debug.Log(navTemp);
        navCameras[navTemp].SetActive(true);
    }

    public void ToggleAICar(int index)
    {
        aiCars[index].SetActive(!aiCars[index].activeSelf);
    }
}
