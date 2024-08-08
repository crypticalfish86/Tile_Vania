using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionController : MonoBehaviour
{
    [Header("Object references")]
    [SerializeField] TextMeshProUGUI livesTextObject;
    [SerializeField] TextMeshProUGUI scoreTextObject;

    [Header("Numbers")]
    [SerializeField] int startingPlayerLives = 3;
        int currentPlayerLives;
    int currentScore;

    /*
        Ensure single game object persits on scene reloads and doesn't get deleted (destroy any objects after initlization of first one).

        Also Initialize all neccessary things at start of game like:
            -Player lives
    */
    private void Awake() {

        //Ensure single game object persits
        int numGameSessionObjects = FindObjectsOfType<GameSessionController>().Length;

        if (numGameSessionObjects > 1) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }

        //Initialize starting variables
        
        currentPlayerLives = startingPlayerLives;
        SetLivesInTextUI();
        
        currentScore = 0;
        SetScoreInTextUI();

    }
    
    //if players lives run out restart game, otherwise remove a life
    public void ProcessPlayerDeath() {
        if (currentPlayerLives < 1) {
            Debug.Log("GameObject reset");
            SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
        else {
            currentPlayerLives--;
            SetLivesInTextUI();
            Debug.Log("Player Lives: " + currentPlayerLives);
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentLevelIndex);
        }
    }

    //Lives
    private void SetLivesInTextUI() {
        livesTextObject.text = $"Lives x {currentPlayerLives}";
    }

    //Score
    public void addToScore(int scoreToAdd) {
        currentScore += scoreToAdd;
        SetScoreInTextUI();
    }
    private void SetScoreInTextUI() {
        scoreTextObject.text = $"Score: {currentScore}";
    }
}
