using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGameController
{
    Transform[] DestinationObjects { get; }
    float GameTimeLimit { get;}
    float KillPoints { get; }

    void EndGame();

    void SubmitName(InputField input);

    void OnInputValueChange(InputField input);

    void SpawnCustomer(int spawnIndex);

    void spawnDestination();

    void AddPoints(float points);

    void LoadScene(string sceneName);

    void ChangeNavigatorStyle();
}
