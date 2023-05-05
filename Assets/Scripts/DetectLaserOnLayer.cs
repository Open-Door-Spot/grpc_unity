using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;


public class DetectLaserOnLayer : MonoBehaviour
{ //put this on the plane

	public Camera mainCamera;
	public GameObject raycastCanvas; //where output hitpoints
	float handRight; //a float that stores the value for buttonpress
	public Plane plane;
    public GameObject _imageCube;

    public GameObject _capsule;
	private Renderer _capsuleRend;
	public GameObject _laserPoint;

	int handleX, handleY, hingeX, hingeY;

    private float commandStart = 0f;
    private float triggerCooldown = 2f; // 5 = five seconds

    // Start is called before the first frame update
    void Start()
	{
		raycastCanvas = GameObject.FindGameObjectWithTag("Plane");
		handleX = handleY = hingeX = hingeY = -1;
        _capsule = GameObject.FindGameObjectWithTag("Capsule");
		_capsuleRend = _capsule.GetComponent<Renderer>();
		_capsuleRend.material.color = Color.black;

        Renderer laserPointRenderer = _laserPoint.GetComponent<Renderer>();
        laserPointRenderer.material.color = Color.red;
    }

    // Update is called once per frame
    async void Update(){
		//handright stores a variable. if right hand is not clicked == 0, if clicked == 1
		handRight = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
		bool buttonA = OVRInput.Get(OVRInput.Button.One); //button A

        //if the hand trigger is pressed
        if (handRight > 0.9){
			if (CustomLaserPointer.instance.LaserHitPlane()){
			int x, y;
			if (handleX == -1 && handleY == -1 && hingeX == -1 && hingeY == -1 && _capsuleRend.material.color == Color.cyan)
			{
				(x, y) = pinpointLocation();
                handleX = x;
				handleY = y;
				commandStart = Time.time;
			}
			else if (handleX != -1 && handleY != -1 && hingeX == -1 && hingeY == -1 && Time.time > commandStart + triggerCooldown)
			{
                (x, y) = pinpointLocation();
                hingeX = x;
				hingeY = y;
				commandStart = Time.time;
				if (handleX != -1 && handleY != -1 && hingeX != -1 && hingeY != -1)
				{
					_capsuleRend.material.color = Color.green;
				}
			}
		}
		}

		//if both buttonA and the hand trigger is pressed
		if (handRight > 0.9 && buttonA && _capsuleRend.material.color == Color.black)
		{
			if (CustomLaserPointer.instance.LaserHitCapsule())
			{
				_capsuleRend.material.color = Color.cyan;
				await initiateSpotImageTransfer();
			}
		}
		else if (handRight > 0.9 && buttonA && _capsuleRend.material.color == Color.green)
		{
			if (CustomLaserPointer.instance.LaserHitCapsule())
			{
				if (handleX != -1 && handleY != -1 && hingeX != -1 && hingeY != -1)
				{
					_capsuleRend.material.color = Color.gray;
					await sendBackCoordinates();
				}
			}
		}
	}
	//Corountine important as laser detects too quickly, give time for the player

	private (int x, int y) pinpointLocation()
	{
		//grab laser location on layer
		RaycastHit hit = CustomLaserPointer.instance.getHit(); //where is getHit?
		// the older code here
		var localHit = transform.InverseTransformPoint(hit.point); //world coordinate

		//Do something with the local coordinate
		UnityEngine.Debug.Log("Plain Local Hit coordinate: " + localHit);
		var (x, y, z) = (localHit.x, localHit.y, localHit.z);

		GameObject mark = Instantiate(_laserPoint);
		mark.transform.position = hit.point;

		int WIDTH = 960;
		int HEIGHT = 640;
		int pixelX = Math.Min(WIDTH, (int)((Mathf.Abs(-8 - x) / 12) * 960));
		int pixelY = Math.Min(HEIGHT, (int)((Mathf.Abs(6 - y) / 8) * 640));

		return (pixelX, pixelY);
	}

	async Task<bool> initiateSpotImageTransfer()
	{
		string initiateSpotEndpoint = "http://138.16.161.57:5030/api/image";

        Renderer rend = _imageCube.GetComponent<Renderer>();
        Texture2D texture = await GetRemoteTexture(initiateSpotEndpoint);
        rend.material.mainTexture = texture;
		return true;
	}

	async Task<bool> sendBackCoordinates()
	{
		string sendBackSpotEndpoint = "http://138.16.161.57:5030/api/params";

        var values = new Dictionary<string, string>
                {
                    { "handle_x", handleX.ToString() },
                    { "handle_y", handleY.ToString() },
					{"hinge_x", hingeX.ToString() },
					{"hinge_y", hingeY.ToString() }
                };

        var content = new FormUrlEncodedContent(values);

        using (var httpClient = new HttpClient())
		{
            await httpClient.PostAsync(sendBackSpotEndpoint, content);
        }
        return true;
	}

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();

            // await until it's done: 
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            // read results:
            if (www.result != UnityWebRequest.Result.Success) // for Unity >= 2020.1
            {
                // log error:
#if DEBUG
                            Debug.Log( $"{www.error}, URL:{www.url}" );
#endif

                // nothing to return on error:
                return null;
            }
            else
            {
                // return valid results:
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}

