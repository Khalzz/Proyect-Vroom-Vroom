using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] GameObject chaseCamera;
    [SerializeField] GameObject povCamera;
    public int camera;

    // Start is called before the first frame update
    void Start()
    {
        povCamera.SetActive(false);
        chaseCamera.SetActive(true);
        camera = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ChangeCamera") && camera == 1)
        {
            chaseCamera.SetActive(false);
            povCamera.SetActive(true);
            camera = 2;
        }
        else if (Input.GetButtonDown("ChangeCamera") && camera == 2)
        {
            chaseCamera.SetActive(true);
            povCamera.SetActive(false);
            camera = 1;
        }
    }
}
