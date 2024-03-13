///SCRIPT THAT DEALS WITH THE GAME ITSELF
///HANDLES: ENEMY SPAWNING, ITEM SPAWNING, ROUND TIMER CALCULATION, LEVEL SWITCH (LOADS NEW SCENE), HIGH SCORE SAVING
///GETS: ENEMY PREFAB, TRANSFORM FOR SPAWNPOINTS, PLAYER STATE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform[] _spawnpointTransform;

    GameObject _enemyContainer;

    public static int level = 1;
    [SerializeField] int _currentLevel = 1;

    [SerializeField] PlayerState _playerState;
    [SerializeField] PowerupState _powerupState;
    int _spawnState = 0;

    void Start()
    {
        _enemyContainer = this.transform.Find("EnemyContainer").gameObject;
        StartCoroutine(RoundTimer());
    }

    // Update is called once per frame
    void Update()
    {
        _currentLevel = level;
        _currentLevel = Mathf.Clamp(_currentLevel, 1, 6);
        if (_spawnState == 1 && _enemyContainer.transform.childCount == 0)
        {
            StartCoroutine(EnemyLevelSpawnCoRoutine(_currentLevel));
        }

        if (_spawnState == 1 && _playerState.health == 0)
        {
            Destroy(this._enemyContainer.gameObject);
            _spawnState = 0;

            if (PlayerPrefs.HasKey("High Score"))
            {
                if (PlayerPrefs.GetInt("High Score") < PlayerState.score)
                    PlayerPrefs.SetInt("High Score", PlayerState.score);
            }
            else
                PlayerPrefs.SetInt("High Score", PlayerState.score);
            PlayerPrefs.Save();

            StopAllCoroutines();
        }
    }

    private IEnumerator EnemyLevelSpawnCoRoutine(int cLevel)
    {
        int index = 0;
        int[] usedSpawn = new int[6] { 0, 0, 0, 0, 0, 0 };
        Transform[] chosenSpawnpoint = new Transform[cLevel];
        for (int i = 0; i < cLevel; i++) 
        {
            int randomSpawnIndex = Random.Range(0, 6);
            while (usedSpawn[randomSpawnIndex] != 0)
            {
                randomSpawnIndex = Random.Range(0, 6);
            }
            usedSpawn[randomSpawnIndex] = 1;
            chosenSpawnpoint[index++] = _spawnpointTransform[randomSpawnIndex];
        }

        for (int i = 0; i < index; i++) 
        {
            Instantiate(_enemyPrefab, chosenSpawnpoint[i].position, Quaternion.identity, _enemyContainer.transform);
        }

        yield return null;
    }

    private IEnumerator RoundTimer()
    {
        while(_playerState.currentRoundSeconds < _playerState.maxRoundSeconds)
        {
            _playerState.currentRoundSeconds++;
            yield return new WaitForSeconds(1);
        }
        _playerState.insideRound = false;
        PlayerState.score += 10;
        _playerState.canBeHit = false;
        StartCoroutine(CooldownTimer());

    }

    private IEnumerator CooldownTimer()
    {
        _spawnState = 0;
        Destroy(this._enemyContainer.gameObject);
        while(_playerState.currentCooldownSeconds < _playerState.maxCooldownSeconds)
        {
            _playerState.currentCooldownSeconds++;
            yield return new WaitForSeconds(1);
        }

        if (level == 5)
            PowerupState.shotgunEnabled = true;
        else if (level == 3)
            PowerupState.fastBullet = true;
        else if (level == 2)
            PowerupState.ricochet = true;
        else if (level == 4)
            PowerupState.fastFirerate = true;
        else if (level == 1)
            PowerupState.oneShot = true;

        level++;
        _playerState.canBeHit = true;

        

        SceneManager.LoadScene("Game", LoadSceneMode.Single);

    }
}
