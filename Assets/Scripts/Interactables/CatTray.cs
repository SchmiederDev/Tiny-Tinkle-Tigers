using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTray : MonoBehaviour
{
    [SerializeField]
    int levelXP = 500;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Kitten")
        {            
            TheGame.GameControl.trainedKittens++;
            GameObject TrainedKitten = collision.gameObject;

            if (TheGame.GameControl.trainedKittens == TheGame.GameControl.kittensToTrain)
            {
                TheGame.GameControl.AddXP(CalculateXP());
                TheGame.GameControl.levelGoalAccomplished = true;
                TheGame.GameControl.gameCanStart = false;
            }

            TheGame.GameControl.RemoveKittenFromScene(TrainedKitten);

        }
    }

    private int CalculateXP()
    {
        return levelXP * TheGame.GameControl.KittensOnScene.Count;
    }
}
