using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour, IObservable
{
    [SerializeField] public float enemyHealthGain;
    [SerializeField] private Wave[] waves;

    [HideInInspector] public static Level level;
    
    private bool endGame = true;
    private SafeInt allScore;
    private int needScore;
    private bool waveEnd;
    private List<IObserver> observers = new List<IObserver>();

    public bool EndGameStatus { get => endGame; }

    void Awake()
    {
        level = this;
    }

    void Start()
    {
        StartCoroutine(LevelCoroutine());
    }

    public void AddScore(int score, float superPowerScore)
    {
        allScore = new SafeInt(allScore.GetValue() + score);
        Player.player.armament.superPower.LoadSuperPower(superPowerScore);
        GUIGameUpdater.guiGameUpdater.SetScore(allScore.GetValue());
    }

    public void RegisterObserver(IObserver observer)
    {
        observer.Observable = this;
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observer.Observable = null;
        observers.Remove(observer);
        waveEnd = true;
    }

    public bool LevelPerfectPassed()
    {
        return allScore.GetValue() >= needScore;
    }

    IEnumerator LevelCoroutine()
    {
        GUIGameUpdater.guiGameUpdater.SetWave(1, waves.Length);

        yield return null;
        yield return SplashCoroutine(-25f, 30f);
        yield return new WaitForSeconds(0.5f);

        Player.player.boundsCheck.keepOnScreen = true;
        endGame = false;

        for (int i = 0; i < waves.Length; i++)
        {
            GUIGameUpdater.guiGameUpdater.SetWave(i + 1, waves.Length);

            if (waves[i].DontWaitEnd)
            {
                StartCoroutine(InitCoroutine(waves[i]));
            }
            else
            {
                waveEnd = false;
                yield return StartCoroutine(InitCoroutine(waves[i]));

                while (!waveEnd)
                    yield return null;
                
                Debug.Log("Конец волны");
            }      
        }

        Debug.Log("Конец уровня");
        Debug.Log("Очков " + allScore.GetValue() + "/" + needScore);
        EndGame(true);
        yield break;
    }

    IEnumerator SplashCoroutine(float posY, float speed)
    {
        Transform playerTransform = Player.player.transform;
        Vector3 tmpPos = playerTransform.position;

        while (tmpPos.y < posY)
        {
            tmpPos.y += speed * Time.deltaTime;
            playerTransform.position = tmpPos;
            yield return null;
        }

        playerTransform.position = new Vector3(0, posY, 0);
        yield break;
    }

    IEnumerator InitCoroutine(Wave wave)
    {
        RegisterObserver(wave);
        yield return StartCoroutine(wave.Init());
        needScore += wave.Score;

        yield break;
    }

    public void EndGame(bool win)
    {
        StartCoroutine(EndGameCoroutine(win));
    }

    private IEnumerator EndGameCoroutine(bool win)
    {
        endGame = true;
        yield return new WaitForSeconds(1.5f);

        if (win)
        {
            Player.player.boundsCheck.keepOnScreen = false;
            EffectController.effectController.splashEnd?.SetActive(true);

            yield return SplashCoroutine(50f, 100f);
            yield return new WaitForSeconds(1f);
        }

        UIController.uiController.ShowResult(win);

        yield break;
    }
}
