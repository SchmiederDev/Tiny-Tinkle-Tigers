using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField]
    private Furniture furnitureObject;
    public string ObjectName { get; private set; }

    public Vector2 ObjectOrigin { get; private set; }
    public Vector2 ObjectScale { get; private set; }

    [SerializeField]
    private float minimalDistance = 1f;

    public float effectiveDistance { get; private set; }


    [SerializeField]
    private BoxCollider2D ObjectCollider;

    // Start is called before the first frame update
    public void Init_MapObject()
    {
        Get_FurnitureObject_and_Set_Scale();
        CalculateEffectiveDistance();
        CalculateOrigin();
        Add_and_FetchCollider();
    }

    private void Get_FurnitureObject_and_Set_Scale()
    {        
        ObjectName = furnitureObject.ObjectName;
        ObjectScale = new Vector2(furnitureObject.X_Extension, furnitureObject.Y_Extension);
    }

    private void CalculateEffectiveDistance()
    {
        effectiveDistance = ObjectScale.x + minimalDistance;
    }

    private void CalculateOrigin()
    {
        float xPos = gameObject.transform.position.x - ObjectScale.x/2f;
        float yPos = gameObject.transform.position.y + ObjectScale.y/2f;
        ObjectOrigin = new Vector2(xPos, yPos);
    }

    private void Add_and_FetchCollider()
    {
        gameObject.AddComponent<BoxCollider2D>();
        ObjectCollider = GetComponent<BoxCollider2D>();
    }

}
