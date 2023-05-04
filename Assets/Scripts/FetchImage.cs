using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FetchImage : MonoBehaviour
{
    public GameObject _imagePlane;
    private string _imageUrl = "http://138.16.161.65:5030/api/image";

    // Start is called before the first frame update
    async void Start()
    {
        Renderer rend = _imagePlane.GetComponent<Renderer>();
        Texture2D texture = await GetRemoteTexture(_imageUrl);
        rend.material.mainTexture = texture;

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
            if( www.result!=UnityWebRequest.Result.Success ) // for Unity >= 2020.1
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
