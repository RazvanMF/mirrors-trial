///SCRIPT THAT DEALS UI CHANGES
///HANDLES: LIVES, SECONDS, LEVELS, SCORE AND HIGHSCORE (UI)
///         RESTARTS (IF DEAD) (SHOULD BE CHANGED)
///GETS: PLAYER STATE, CANVASES

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] PlayerState _playerState;
    [SerializeField] Canvas[] _uiStates;
    
    int _currentState = 0;
    GameManager _gameManager;

    TextMeshProUGUI lifeText, secondsText, levelText, scoreText, powerText, gameOverScoreText;

    void Start()
    {
        //state 0: play
        //state 1: reset
        _uiStates[0].gameObject.SetActive(true);
        _uiStates[1].gameObject.SetActive(false);

        _gameManager = GetComponent<GameManager>();

        lifeText = _uiStates[0].transform.Find("LifeText").GetComponent<TextMeshProUGUI>();
        secondsText = _uiStates[0].transform.Find("SecondsText").GetComponent<TextMeshProUGUI>();
        levelText = _uiStates[0].transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        scoreText = _uiStates[0].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        powerText = _uiStates[0].transform.Find("PowerText").GetComponent<TextMeshProUGUI>();

        gameOverScoreText = _uiStates[1].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        CanvasStateHandler();
        if (_currentState == 0)
            PlayStateHandler();
        else if (_currentState == 1)
        {
            GameOverStateHandler();

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.level = 1;

                PlayerState.score = 0;

                PowerupState.shotgunEnabled = false;
                PowerupState.fastBullet = false;
                PowerupState.ricochet = false;
                PowerupState.fastFirerate = false;
                PowerupState.oneShot = false;

                PlayerMovement._positionRetainerVector = Vector2.zero;

                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }

    }

    void CanvasStateHandler()
    {
        if (_playerState.health == 0)
        {
            _currentState = 1;
            _uiStates[0].gameObject.SetActive(false);
            _uiStates[1].gameObject.SetActive(true);
        }
    }

    void PlayStateHandler()
    {
        lifeText.text = "Lives: " + _playerState.health.ToString();

        if (_playerState.insideRound)
            secondsText.text = "-Seconds-\n" + _playerState.currentRoundSeconds.ToString() + "/" + _playerState.maxRoundSeconds.ToString();
        else
            secondsText.text = "-Seconds-\n" + _playerState.currentCooldownSeconds.ToString() + "/" + _playerState.maxCooldownSeconds.ToString();

        levelText.text = "Level: " + GameManager.level.ToString() + "/6";

        scoreText.text = "-Score-\n" + PlayerState.score.ToString();

        powerText.text = $"Shotgun: {(PowerupState.shotgunEnabled == true ? "T" : "F")}\n" +
            $"FastBullet: {(PowerupState.fastBullet == true ? "T" : "F")}\n" +
            $"Ricochet: {(PowerupState.ricochet == true ? "T" : "F")}\n" +
            $"Firerate: {(PowerupState.fastFirerate == true ? "T" : "F")}\n" +
            $"OneShot: {(PowerupState.oneShot == true ? "T" : "F")}";

    }

    void GameOverStateHandler()
    {
        int highscore = PlayerPrefs.GetInt("High Score");
        if (highscore == PlayerState.score)
            gameOverScoreText.color = Color.green;
        else
            gameOverScoreText.color = Color.red;
        gameOverScoreText.text = "Score: " + PlayerState.score.ToString() + "\nHigh Score: " + highscore.ToString();
    }
}
