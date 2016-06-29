using UnityEngine;
using System.Collections;

public class WebTexture001 : MonoBehaviour {
	private WebCamTexture webcamTexture;
	private Texture2D texture = null;

	// Use this for initialization
	void Start()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		if (devices.Length > 0)
		{
			Debug.Log("CAMARA");
			webcamTexture = new WebCamTexture(devices[0].name, 320, 240, 10);
			this.GetComponent<Renderer>().material.mainTexture = webcamTexture;
			webcamTexture.Play();
		}
	}

	// Update is called once per frame
	void OnGUI()
	{
	}

	// Update is called once per frame
	void Update()
	{
/*		
		Color32[] pixels = webcamTexture.GetPixels32();
		//		GCHandle pixelsHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
		//		UpdateTexture(pixelsHandle.AddrOfPinnedObject(), webcamTexture.width, webcamTexture.height);
		if (texture) Destroy(texture);
		//Debug.Log("WIDTH = " + webcamTexture.width);
		//Debug.Log("HIGHT = " + webcamTexture.height);
		texture = new Texture2D(webcamTexture.width, webcamTexture.height);
		texture.SetPixels32(pixels);
		texture.Apply();
		//		pixelsHandle.Free();
		this.GetComponent<Renderer>().material.mainTexture = texture;
*/		
	}
}
