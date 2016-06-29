using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Callback = System.Action<string>;

public class CameraARLib : MonoBehaviour {
	// CameraAR Object
	private Binding bindingObject = new Binding();
	private CameraAR CameraARObject = new CameraAR();
	private int status;
	private Color32[] dataIn;
	private GCHandle pixelsHandleIn;
	private IntPtr pixels_ptr = IntPtr.Zero;
	private int ready = 0;

	private int texWidth = 1024;
	private int texHeight = 1024;
	private Texture2D CameraTex = null;
	private System.IntPtr textureid;

#if UNITY_IPHONE
	private Texture2D NetiveTex = null;
#endif

	// ログの記録
	private static List<string> logMsg = new List<string>();
	public static void log(string msg) {
		logMsg.Add(msg);
		// 直近の5件のみ保存する
		if ( logMsg.Count > 20 ) {
			logMsg.RemoveAt( 0 );
		}
	}

	void Awake() {
		status = 0;
		status = CameraARObject.open();
	}

	// Use this for initialization
	void Start () {
		if (status == 1)
		{
			StartCoroutine(videoplay());
		}
				
	}

	IEnumerator videoplay() {
		yield return new WaitForSeconds(3);
		createCameratexture ();
	}

	// Update is called once per frame
	void Update () {
#if UNITY_ANDROID		
		if (ready == 0)
			return;
		CameraARObject.UpdateBufferImage(pixels_ptr);
		//log("a5: " + dataIn[0].a + " b: " + dataIn[0].b + " g: " + dataIn[0].g + " r: " + dataIn[0].r);
		log("width = " + CameraTex.width + " height = " + CameraTex.height);
		bindingObject.HybridSetTextureOfCam1(pixels_ptr, CameraTex.width, CameraTex.height);
		CameraTex.SetPixels32(dataIn);
		CameraTex.Apply();
		pixelsHandleIn.Free();
#endif
#if UNITY_IPHONE
		if (ready == 0)
			return;

		CameraARObject.setTextureImage(pixels_ptr);
		log("width2 = " + CameraTex.width + " height2 = " + CameraTex.height);
		bindingObject.HybridSetTextureOfCam1(pixels_ptr, CameraTex.width, CameraTex.height);
		CameraTex.SetPixels32(dataIn);
		CameraTex.Apply();
		pixelsHandleIn.Free();

//		textureid = CameraARObject.GetTexturePtr();
//		CameraTex.UpdateExternalTexture(textureid);
#endif
	}

	// カメラ用のテクスチャー作成
	void createCameratexture()
	{

#if UNITY_ANDROID
	texWidth = CameraARObject.getwidth();
	texHeight = CameraARObject.getheight();
	CameraTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
	CameraTex.filterMode = FilterMode.Point;
	dataIn = CameraTex.GetPixels32();
	pixelsHandleIn = GCHandle.Alloc(dataIn, GCHandleType.Pinned);
	pixels_ptr = pixelsHandleIn.AddrOfPinnedObject();
	ready = 1;
#endif
#if UNITY_IPHONE

	CameraTex = new Texture2D(640, 852, TextureFormat.ARGB32, false);
	CameraTex.filterMode = FilterMode.Point;
	dataIn = CameraTex.GetPixels32();
	pixelsHandleIn = GCHandle.Alloc(dataIn, GCHandleType.Pinned);
	pixels_ptr = pixelsHandleIn.AddrOfPinnedObject();
	ready = 1;
	
/*
	CameraTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
	CameraTex.filterMode = FilterMode.Point;
	CameraTex.wrapMode = TextureWrapMode.Clamp;
	CameraTex.Apply();

	textureid = CameraARObject.GetTexturePtr(); // IOSからテクスチャーID取得
	NetiveTex = Texture2D.CreateExternalTexture(texWidth, texHeight, TextureFormat.BGRA32, false, false, textureid);
	CameraTex.UpdateExternalTexture(NetiveTex.GetNativeTexturePtr());
	ready = 1;
*/
#endif
	}

	// get property
	public Texture2D GetCameraTexture()
	{
		return CameraTex;
	}
	public Color32[] GetCameraColor32()
	{
		//CameraTex.
		return CameraTex.GetPixels32();
	}

	void OnGUI()
	{
		Rect rect = new Rect( 0, 300, Screen.width - 20, Screen.height - 20 );
		GUIStyle style = new GUIStyle();
		style.fontSize = 30;
		style.fontStyle = FontStyle.Normal;
		style.normal.textColor = Color.blue;
		// 出力された文字列を改行でつなぐ
		string outMessage = "";
		foreach( string msg in logMsg ) {
			outMessage += msg + System.Environment.NewLine;
		}
		// 改行でつないだログメッセージを画面に出す
		GUI.Label(rect, outMessage, style);
	}

}
