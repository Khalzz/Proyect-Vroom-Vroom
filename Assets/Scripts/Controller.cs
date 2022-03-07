using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Slider throttleSlider;
    public Slider brakeSlider;
    public Slider steerSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        throttleSlider.value = Input.GetAxis("Throttle");
        brakeSlider.value = Input.GetAxis("Brake");
        steerSlider.value = Input.GetAxis("Steer");
    }
}
