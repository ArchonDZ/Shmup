using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Row
{
    public GameObject[] gameObjects;
}

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave", order = 51)]
public class Wave : ScriptableObject, IObservable, IObserver
{
    [Header("Bezier")]
    [SerializeField] private Bezier bezier;
    [SerializeField] private float moveTime = 3.0f;
    [SerializeField] private float placeTime = 1.5f;
    [Header("or from the point")]
    [SerializeField] private Vector3 posPoint;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float speed;
    [SerializeField] private BoundType boundType = BoundType.down;
    [Header("Common")]
    [SerializeField] private float delaySecSpawn = 0.5f;
    [SerializeField] private bool dontWaitEnd;
    [SerializeField] private Row[] rowsEnemies;

    public IObservable Observable { get => level; set => level = value as Level; }
    public int Score { get; private set; }
    public bool DontWaitEnd { get => dontWaitEnd; }

    private int enemyCount;
    private Level level;

    public void RegisterObserver(IObserver observer)
    {
        observer.Observable = this;
    }

    public void RemoveObserver(IObserver observer)
    {
        observer.Observable = null;
        enemyCount--;

        if (!dontWaitEnd && enemyCount <= 0)
            End();
    }

    private void End()
    {
        Observable?.RemoveObserver(this);
    }

    public IEnumerator Init()
    {
        ScreenPlay.screenPlay?.InitWave(bezier, moveTime, placeTime);

        if (!dontWaitEnd)
        {
            enemyCount = 0;
            for (int i = 0; i < rowsEnemies.Length; i++)
                for (int j = 0; j < rowsEnemies[i].gameObjects.Length; j++)
                    if (rowsEnemies[i].gameObjects[j] != null)
                        if (rowsEnemies[i].gameObjects[j].TryGetComponent(out Enemy enemy))
                            enemyCount++;
        }

        Score = 0;
        for (int i = 0; i < rowsEnemies.Length; i++)
            for (int j = 0; j < rowsEnemies[i].gameObjects.Length; j++)
                if (rowsEnemies[i].gameObjects[j] != null)
                    if (rowsEnemies[i].gameObjects[j].TryGetComponent(out Enemy enemy))
                    {
                        RegisterObserver(enemy);

                        if (posPoint != Vector3.zero)
                            ScreenPlay.screenPlay?.SpawnObjectInPoint(rowsEnemies[i].gameObjects[j], posPoint, i, j, direction, rotation, speed, boundType);
                        else
                            ScreenPlay.screenPlay?.SpawnObjectInPoint(rowsEnemies[i].gameObjects[j], bezier.GetFirstPoint(), i, j);

                        Score += enemy.score;

                        yield return new WaitForSeconds(delaySecSpawn);
                    }

        yield break;
    }
}
