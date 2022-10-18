using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonsController : MonoBehaviour
{
    private List<GameObject> balloons= new List<GameObject>();
    [SerializeField]private Rigidbody connectedRigidbody;
    [SerializeField]private float spawnRate;
    private float t;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            SpawnBalloon();

        }

         t += Time.deltaTime;

        if (spawnRate > 0.5f && t >= spawnRate )
        {
            SpawnBalloon();
            t = 0;
        }
    }
 
    public void SpawnBalloon()
    {

        GameObject balloon = ObjectPoolManager.Instance.GetObject("Balloon");
        
        balloons.Add(balloon);
        balloon.GetComponent<Balloons>().ConnectedRigidbody = connectedRigidbody;
        balloon.GetComponent<Balloons>().Distance =  Random.Range(4.8f, 5.2f);
        balloon.transform.position = connectedRigidbody.position + Vector3.up/2;
        balloon.transform.localScale = Vector3.zero;
        balloon.SetActive(true);
    }

}
