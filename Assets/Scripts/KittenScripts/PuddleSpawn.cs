using System.Collections;
using UnityEngine;

public class PuddleSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject PeePuddle;

    [SerializeField]
    float puddleSpawnRate = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPuddle());        
    }

    IEnumerator SpawnPuddle()
    {
        yield return new WaitForSeconds(puddleSpawnRate);
        GameObject PeePuddleInstance = Instantiate(PeePuddle, gameObject.transform.position, Quaternion.identity);
        StartCoroutine(SpawnPuddle());
    }
}
