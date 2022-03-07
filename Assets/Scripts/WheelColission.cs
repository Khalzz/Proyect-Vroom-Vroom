using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColission : MonoBehaviour
{
    public GameObject colissionPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnColissionEnter(Collision other)
    {
        ContactPoint[] contacts = new ContactPoint[10];
        int numContacts = other.GetContacts(contacts);

        Instantiate(colissionPoint, contacts[0].point, Quaternion.identity);
    }
}
