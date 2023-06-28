using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNotesDB : MonoBehaviour
{
    public List<GameNotification> gameNotifications;

    public string FindNotification(string notificationType)
    {
        string notification = "emptyMessage";

        GameNotification notificationsOfType = gameNotifications.Find(notificationElements => notificationElements.messageType == notificationType);

        if (notificationsOfType != null)
        {
            int randomIndex = Random.Range(0, notificationsOfType.messages.Length);
            notification = notificationsOfType.messages[randomIndex];
            return notification;
        }

        else
        {
            Debug.Log("Notification type unknown.");
            return notification;
        }
    }
}
