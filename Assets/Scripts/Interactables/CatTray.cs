using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTray : MonoBehaviour
{
    [SerializeField]
    int levelXP = 500;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Kitten")
        {
            TheGame.GameControl.trainedKittens++;

            if (TheGame.GameControl.trainedKittens == TheGame.GameControl.KittensOnScene.Count)
                TheGame.GameControl.AddXP(CalculateXP());
        }
    }

    private int CalculateXP()
    {
        return levelXP * TheGame.GameControl.KittensOnScene.Count;
    }
}
