using System.Collections;
using UnityEngine;

public class CatTray : MonoBehaviour
{
    [SerializeField]
    int levelXP = 500;

    MapObject CatTrayObject;

    [SerializeField]
    PTS_Controller WinEffect_Controller;

    private void Start()
    {
        WinEffect_Controller = GetComponentInChildren<PTS_Controller>();
        CatTrayObject = GetComponent<MapObject>();
        CatTrayObject.Init_MapObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Kitten")
        {
            GameObject Kitten_GO = collision.gameObject;
            bool foundController = false;

            if(Kitten_GO != null)
            {
                KittenController attachedKittenController = Kitten_GO.GetComponent<KittenController>();

                if (attachedKittenController != null)
                    foundController = true;

                if (foundController)
                {
                    if (!attachedKittenController.isMovingRandomly)
                    {
                        AddTrainedKitten(Kitten_GO);
                        WinEffect_Controller.Activate_PTS();
                    }

                    else
                    {
                        attachedKittenController.ActFrightened();
                        attachedKittenController.InvertDirections();
                    }
                }

                else
                {
                    AddTrainedKitten(Kitten_GO);
                    WinEffect_Controller.Activate_PTS();
                }
            }
        }
    }

    private int CalculateXP()
    {
        return levelXP * TheGame.GameControl.KittensOnScene.Count;
    }

    private void AddTrainedKitten(GameObject trainedKitten)
    {
        TheGame.GameControl.trainedKittens++;
        CheckNumberOfTrainedKittens();
        
        TheGame.GameControl.RemoveKittenFromScene(trainedKitten);
    }

    private void CheckNumberOfTrainedKittens()
    {
        if (TheGame.GameControl.trainedKittens == TheGame.GameControl.kittensToTrain)
        {
            TheGame.GameControl.AddXP(CalculateXP());
            TheGame.GameControl.levelGoalAccomplished = true;
            
            string winNotification = TheGame.GameControl.GameNotification.gameNotesDB.FindNotification("Win");
            TheGame.GameControl.GameNotification.SendPopMessage(winNotification);
            
            StartCoroutine(WaitForWinAnimation());
        }
    }

    IEnumerator WaitForWinAnimation()
    {
        
        yield return new WaitForSecondsRealtime(0.01f);
        
        if (!WinEffect_Controller.GetPlayState())
            TheGame.GameControl.newLevelIsLoading = true;
        
        else
            StartCoroutine(WaitForWinAnimation());
    }

}
