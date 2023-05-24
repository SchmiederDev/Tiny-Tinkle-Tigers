using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Furniture Object", menuName ="MapObject/Furniture")]
public class Furniture : ScriptableObject
{
    public string ObjectName;
    public float X_Extension;
    public float Y_Extension;
}
