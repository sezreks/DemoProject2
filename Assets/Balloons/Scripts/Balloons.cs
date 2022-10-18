using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Balloons : MonoBehaviour
{
    //public Transform cube, sphere;
    //public Vector3 vec;


    public Rigidbody ConnectedRigidbody;
    public bool DetermineDistanceOnStart = true;
    public float Distance;
    public float Spring = -0.1f;
    public float Damper = -5f;
    protected Rigidbody Rigidbody;
    [SerializeField]private bool connected = false;

  


    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (DetermineDistanceOnStart)
            Distance = Vector3.Distance(Rigidbody.position, ConnectedRigidbody.position);

    }


    void Update()
    {
       

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit = new RaycastHit();
        ////if (Input.GetMouseButton(0))
        ////{
        ////    if (Physics.Raycast(ray, out hit))
        ////    {
        ////        //hit.transform.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward.normalized, ForceMode.Impulse);
        ////        print(hit.collider.name);

        ////        hit.transform.GetComponent<Rigidbody>().AddExplosionForce(5, hit.point, 5);
        ////    }
        ////}

    


    }


    void FixedUpdate()
    {
        
            transform.LookAt(ConnectedRigidbody.transform);
            transform.eulerAngles -= new Vector3(90, 0, 0);

        if (!connected)
        {
            Rigidbody.position = Vector3.Slerp(Rigidbody.position, ConnectedRigidbody.position + new Vector3(0, Distance*2, 0), Time.fixedDeltaTime * 1.5f);
            if (Vector3.Distance(Rigidbody.position, ConnectedRigidbody.position) >= Distance)
            {
                connected = true;
            }
        }
        else
        {
            var connection = Rigidbody.position - ConnectedRigidbody.position;
            var distanceDiscrepancy = Distance - connection.magnitude;
            Rigidbody.position += distanceDiscrepancy * connection.normalized;
            var velocityTarget = connection + (Rigidbody.velocity + Physics.gravity * Spring);
            var projectOnConnection = Vector3.Project(velocityTarget, connection);
            Rigidbody.velocity = (velocityTarget - projectOnConnection) / (1 + Damper + Time.fixedDeltaTime);


            var rope = transform.GetComponent<LineRenderer>();

            rope.SetPosition(0, transform.position); //+ (Vector3.down / 3));
            rope.SetPosition(1, ConnectedRigidbody.position + (Vector3.up / 2));
        }


        
       




    }



}

