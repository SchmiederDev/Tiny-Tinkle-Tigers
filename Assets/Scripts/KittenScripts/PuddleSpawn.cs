using System.Collections;
using UnityEngine;

public class PuddleSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject PeePuddle;

    [SerializeField]
    float initialPuddleSpawnRange = 10f;

    [SerializeField]
    float puddleSpawnRate = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        float momentOfFirstPuddleSpawn = Random.Range(1, initialPuddleSpawnRange);
        StartCoroutine(SpawnFirstPuddle(momentOfFirstPuddleSpawn));        
    }

    IEnumerator SpawnFirstPuddle(float momentOfFirstPuddleSpawn)
    {
        yield return new WaitForSeconds(momentOfFirstPuddleSpawn);
        
        GameObject PeePuddleInstance = Instantiate(PeePuddle, gameObject.transform.position, Quaternion.identity);
        TheGame.GameControl.PuddlesOnScene.Add(PeePuddleInstance);

        StartCoroutine(SpawnPuddle());
    }

    IEnumerator SpawnPuddle()
    {
        yield return new WaitForSeconds(puddleSpawnRate);
        
        GameObject PeePuddleInstance = Instantiate(PeePuddle, gameObject.transform.position, Quaternion.identity);
        TheGame.GameControl.PuddlesOnScene.Add(PeePuddleInstance);

        StartCoroutine(SpawnPuddle());
    }
}
