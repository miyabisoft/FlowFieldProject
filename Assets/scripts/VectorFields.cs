using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Callback = System.Action<string>;

public class VectorFields : MonoBehaviour {
	float xmax = 1.0f;
	float xmin = -1.0f;
	float ymax = 1.0f;
	float ymin = -1.0f;
	// １目盛りの値
	float delx = 0.1f;
	float dely = 0.1f;
	// 刻み値（飛び飛びの値になることもある）
	float deltx = 0.1f;
	float delty = 0.1f;
	float xRange = 0;

	private List<VectorFieldInfo> vectorfields = new List<VectorFieldInfo>();

	Vector3 translation = new Vector3 (1.0f, 1.0f, 0.0f);
	Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	Vector3 scale = new Vector3 (1f, 1f, 1f);

	public List<VectorFieldInfo> getVectorFields() {
		return vectorfields;
	}

	// ログの記録
	private static List<string> logMsg = new List<string>();
	public static void log(string msg) {
		logMsg.Add(msg);
		// 直近の5件のみ保存する
		if ( logMsg.Count > 100 ) {
			logMsg.RemoveAt( 0 );
		}
	}

	// Use this for initialization
	void Start () {
		int count = 0;
		float sx = 0;
		float sy = 0;
		Vector3 v = new Vector3();

		xRange = (xmax - xmin) / deltx;
		// カメラ座標にする x:0~1 y:0~1 ベクトル場を表示して確認するため
		Matrix4x4 m = Matrix4x4.TRS(translation, rotation, scale);

		// Create VectorField's Patter x:-1~1 y:-1~1
		for (float y = -1; y < 1; y = y+delty) {
			for (float x = -1; x < 1; x = x+deltx) {
				sx = x - (y * 0.1f);
				sy = y + (x * 0.1f);
				//sx = x + (y * 0.1f);	// xがsxへ移動
				//sy = y + (x * 0.1f);	// yがsyへ移動
				vectorfields.Add (new VectorFieldInfo ());
				// 位置
				vectorfields [count].setPostion (x, y, 0);

				v = m.MultiplyPoint3x4 (vectorfields [count].getPostion ());
				if (v.x < 0)
					v.x = 0f;
				if (v.y < 0)
					v.y = 0f;
				vectorfields [count].setPostion (v.x, v.y , 0);
				vectorfields [count].setVectorField (sx - x, sy - y, 0);
/*
				if ( (count > 0) && (count < 100) ) {
					log ("px : py  " + vectorfields[count].getPostion().x.ToString() + " , " + 
						vectorfields[count].getPostion().y.ToString());
					log ("vx : vy  " + vectorfields[count].getVectorField().x.ToString() + " , " + 
						vectorfields[count].getVectorField().y.ToString());
				}
*/
				count++;
			}
		}
		//log ("Count = " + count.ToString ());

	}

	void OnPostRender () {
		
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.Color(Color.red);
		for (int i = 0; i < vectorfields.Count; i++) {
			GL.Begin (GL.LINES);
			// 始点
			GL.Vertex3 (vectorfields [i].getPostion ().x/(xmax-xmin), vectorfields [i].getPostion ().y/(xmax-xmin), 0);
			// 終点 = 始点 + ベクトル値
			GL.Vertex3 ((vectorfields [i].getPostion ().x/(xmax-xmin)) + vectorfields[i].getVectorField().x, 
				(vectorfields [i].getPostion ().y/(xmax-xmin)) + vectorfields[i].getVectorField().y, 0);
			GL.End ();
		}
		GL.PopMatrix();

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
