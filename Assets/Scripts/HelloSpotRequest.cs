using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.Net.Http;

public class HelloSpotRequest : MonoBehaviour
{

    private float commandStart = 0f;
    private float commandCooldown = 120f; // 5 = five seconds
    private string endpoint = "http://138.16.161.97:8888/hello_spot";

    void Start()
    {
        Debug.Log("Hello Spot Request is Triggered");
    }

    async void Update()
    {
        //float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        bool buttonX = OVRInput.Get(OVRInput.Button.Three);

        if (buttonX == true)
        {
            if (Time.time > commandStart + commandCooldown)
            {
                commandStart = Time.time;
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(endpoint);
                    Debug.Log(response);
                }
            }
        }
    }
}
