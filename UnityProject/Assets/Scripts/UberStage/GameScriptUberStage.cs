using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScriptUberStage : MonoBehaviour, IGameController
{
    private Transform[] temp = { null, null, null, null };
    private CarEngine player;
    private bool paused = false;
    private AudioManager audioManager;
    private float gameTime = 0;
    private float score = 0;
    private bool finished = false;

    public Transform[] DestinationObjects { get { return temp; } }
    public float GameTimeLimit { get { return 1; } }
    public float KillPoints { get { return 0; } }

    [Header("HUD")]
    public GameObject gameHUD;
    public Text gameTimeText;
    public Text scoreText;
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    [Header("Finish Menu")]
    public GameObject finishMenu;
    public GameObject scoreboardPanel;
    public GameObject submitNamePanel;
    public Button submitNameButton;
    public Text endScoreText;
    public Text endTimeText;
    public Text namesText;
    public Text scoresText;
    public Text crashNamesText;
    public Text crashScoresText;
    [Header("Crash Menu")]
    public GameObject endGameMenu;


    private void Awake()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        player = GameObject.FindWithTag("Player").GetComponent<CarEngine>();
    }

    private void Update()
    {
        //pause
        if (Input.GetButtonDown("Cancel"))
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadScene("UberStage");
        }

        // game clock
        if (!finished)
        {
            gameTime += Time.deltaTime;
            float seconds = gameTime % 60f;
            float minutes = gameTime / 60;
            gameTimeText.text = "Time: " + string.Format("{0:00}:{1:00}", (int)minutes, (int)seconds);

            if (player.Drifting)
            {
                AddPoints(Time.deltaTime);
            }
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

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        audioManager.paused(true);
        Time.timeScale = 0;
        paused = true;
        Debug.Log("Paused");
    }

    public void AddPoints(float points)
    {
        score += points;
        scoreText.text = "Drifting score: " + ((int)score).ToString("F0");
    }

    public void ChangeNavigatorStyle()
    {
        return;
    }

    public void EndGame()
    {
        finished = true;
        gameHUD.SetActive(false);
        crashScoresText.text = Scores.GetUberStageTimes();
        crashNamesText.text = Scores.GetNamesUberStage();
        endGameMenu.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnInputValueChange(InputField input)
    {
        if (input.text.Length > 0)
        {
            submitNameButton.interactable = true;
        }
        else
        {
            submitNameButton.interactable = false;
        }
    }

    public void SpawnCustomer(int spawnIndex)
    {
        return;
    }

    public void spawnDestination()
    {
        return;
    }

    public void SubmitName(InputField input)
    {
        submitNamePanel.SetActive(false);

        Scores.AddPlayerScoreUberStage(input.text, gameTime);
        scoresText.text = Scores.GetUberStageTimes();
        namesText.text = Scores.GetNamesUberStage();

        scoreboardPanel.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CarController>().enabled = false;
            Finish();
        }
    }

    private void Finish()
    {
        finished = true;
        gameHUD.SetActive(false);
        finishMenu.SetActive(true);
        endScoreText.text = "Drifting score: " + ((int)score).ToString("F0");
        float seconds = gameTime % 60f;
        float minutes = gameTime / 60;
        endTimeText.text = "Time: " + string.Format("{0:00}:{1:00}", (int)minutes, (int)seconds);
    }
}
