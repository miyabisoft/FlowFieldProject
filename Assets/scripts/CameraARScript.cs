using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;

public class CameraARScript : MonoBehaviour {
	private CameraARLib cameraObj = null;
	private Binding bindingObject = new Binding ();

	// Use this for initialization
	void Start () 
	{
		cameraObj = GameObject.Find("ArCamera").GetComponent<CameraARLib>();
#if UNITY_ANDROID
		this.GetComponent<Renderer>().material.mainTexture = cameraObj.GetCameraTexture();
#endif
#if UNITY_IPHONE
		log("start");
#endif
	}

	// Update is called once per frame
	void Update () {
#if UNITY_ANDROID
		this.GetComponent<Renderer>().material.mainTexture = cameraObj.GetCameraTexture();
#endif
#if UNITY_IPHONE
		this.GetComponent<Renderer>().material.mainTexture = cameraObj.GetCameraTexture();
#endif
	}

	// ログの記録
	private static List<string> logMsg = new List<string>();
	public static void log(string msg) {
		logMsg.Add(msg);
		// 直近の5件のみ保存する
		if ( logMsg.Count > 50 ) {
			logMsg.RemoveAt( 0 );
		}
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
