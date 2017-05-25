using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour, IGameController {

    public float gameTimeLimit = 600;

    [Header("Spawnpoint")]
    public int customerCount = 3;
    public GameObject spawnpoint; //Spawnpoint parent GameObject
    private Transform[] spawnpoints;
    private int[] usedSpawnpoints;

    [Header("HUD")]
    public Text scoreText;
    public Text endScoreText;
    public Text gameTimeText;
    public GameObject gameHUD;
    public GameObject[] navCameras;
    public GameObject pauseMenu;
    public GameObject endGameMenu;
    public GameObject submitNamePanel;
    public GameObject scoreboardPanel;
    public Button submitNameButton;
    public Text scoresText;
    public Text namesText;

    [Header("Customer")]
    public Transform customerPrefab;
    public Transform destinationPrefab;
    public float killPoints = 10f;

    private Vector3 spawnPosition;
    private Vector3 destination;
    private float score = 0;
    private bool paused = false;
    private int navTemp = 0;
    private AudioManager audioManager;
    private int tempSpawn = -1;
    private int customerType = 0;
    private bool finished = false;

    public Transform[] DestinationObjects { get; private set; }

    public float GameTimeLimit { get { return gameTimeLimit; } }
    public float KillPoints { get { return killPoints; } }

    void Awake()
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

    void Start()
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

    void Update()
    {
        if (gameTimeLimit <= 0)
        {
            EndGame();
        }

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

        // game clock
        gameTimeLimit -= Time.deltaTime;
        float seconds = gameTimeLimit % 60f;
        float minutes = gameTimeLimit / 60;
        gameTimeText.text = "Time: " + string.Format("{0:00}:{1:00}", (int)minutes, (int)seconds);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        audioManager.paused(false);
        Time.timeScale = 1;
        paused = false;
        Debug.Log("Unpaused");
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
        finished = true;
        gameHUD.SetActive(false);
        endGameMenu.SetActive(true);
        endScoreText.text = "Score: " + ((int)score).ToString();
    }

    public void SubmitName(InputField input)
    {
        submitNamePanel.SetActive(false);

        Scores.AddPlayerScore(input.text, (int)score);
        scoresText.text = Scores.GetScores();
        namesText.text = Scores.GetNames();

        scoreboardPanel.SetActive(true);
    }

    public void OnInputValueChange(InputField input)
    {
        if(input.text.Length > 0)
        {
            submitNameButton.interactable = true;
        }
        else
        {
            submitNameButton.interactable = false;
        }
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
        if(customerType > 3) { customerType = 0; }
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
        if(tempSpawn == number) { return true; }
        return false;
    }

    public void AddPoints(float points)
    {
        if (!finished)
        {
            score += points;
            scoreText.text = "Score: " + ((int)score).ToString();
        }
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
}


