using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Game Notification", menuName = "Messages/Game Notification")]
public class GameNotification : ScriptableObject
{
    public string messageType;
    public string[] messages;
}
