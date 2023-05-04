using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;

public class DetectLaserOnLayer : MonoBehaviour
{ //put this on the plane

    public Camera mainCamera;
    public GameObject raycastCanvas; //where output hitpoints
    float handRight; //a float that stores the value for buttonpress
    public Plane plane;
    private string endpoint = "http://138.16.161.65:5030/api/params";


    // Start is called before the first frame update
    async void Start()
    {
        //grab objects in scene
        // but do we really need a tag to do this?
        // I have no freaking idea about this part
        //appearantly we don't have image now but the image is a mesh?
        raycastCanvas = GameObject.FindGameObjectWithTag("Plane");
        await sendBackResult("STARTED", "YES");

        /*
         * Trying to find the vertices...
        try {
            FindAllCorners FAC = new FindAllCorners();
            List<Vector3> CornerPoints = FAC.CalculateCornerPoints();
            await sendBackResult("The length of corner points", CornerPoints.Count.ToString());
            //await sendBackResult("The left upper corner is", CornerPoints[0].ToString());
            //await sendBackResult("The right upper corner is", CornerPoints[1].ToString());
            //await sendBackResult("The left lower corner is", CornerPoints[2].ToString());
            //await sendBackResult("The right lower corner is", CornerPoints[3].ToString());
        }
        catch(Exception e)
        {
            await sendBackResult("ERRORRRR", e.ToString());
        }
        */
    }

    // Update is called once per frame
    void Update(){
        //handright stores a variable. if right hand is not clicked == 0, if clicked == 1
        handRight = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        //if the hand trigger is pressed
        if(handRight > 0.9){
            //await sendBackResult("TRIGGER IS PRESSED", "YES");

            //if the laser is pointed at the layer
            if (CustomLaserPointer.instance.LaserHit()){
                pinpointLocation();
            }

        }    
    }
    //Corountine important as laser detects too quickly, give time for the player

    async Task<bool> sendBackResult(string first, string second)
    {
        var values = new Dictionary<string, string>
                {
                    { "pixel_x", first },
                    { "pixel_y", second }
                };

        var content = new FormUrlEncodedContent(values);

        using (var httpClient = new HttpClient())
        {
            await httpClient.PostAsync(endpoint, content);
        }
        return true;
    }

    async void pinpointLocation(){
        //grab laser location on layer
        RaycastHit hit = CustomLaserPointer.instance.getHit(); //where is getHit?

        // the older code here
        var localHit = transform.InverseTransformPoint(hit.point); //world coordinate

        //Do something with the local coordinate
        UnityEngine.Debug.Log("Plain Local Hit coordinate: " + localHit);
        var (x, y, z) = (localHit.x, localHit.y, localHit.z);

        int pixelX = (int)(1000-((Mathf.Abs(3 - x) / 10) * 1000));
        int pixelY = (int)((Mathf.Abs(7 - y) / 10) * 874);
        UnityEngine.Debug.Log("Pixel X, Y: (" + pixelX + ", " + pixelY + ")");

        //await sendBackResult(pixelX, pixelY);
        await sendBackResult(pixelX.ToString(), pixelY.ToString());
    }
}

