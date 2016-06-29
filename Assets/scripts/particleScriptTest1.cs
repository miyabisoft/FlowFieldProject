
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Callback = System.Action<string>;

[RequireComponent(typeof(ParticleSystem))]
public class particleScriptTest1 : MonoBehaviour {
	GameObject VectorFields;
	List<VectorFieldInfo> vectorfields;
	VectorFields vectorfieldsObj = null;
	ParticleSystem m_System;
	ParticleSystem.Particle[] m_Particles;
	Vector3 point = new Vector3(0, 0, 0);
	Vector3 spoint = new Vector3(0, 0, 0);
	Vector3 fvector = new Vector3(0, 0, 0);
	Matrix4x4 m;
	List<VectorFieldInfo> workVectorfields = new List<VectorFieldInfo>();
	Vector3 translation = new Vector3 (1.0f, 1.0f, 0.0f);
	Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	Vector3 scale = new Vector3 (1f, 1f, 1f);
	int NOMAX = -999;
	int NOMIN = 999;

	float xmax = 2.0f;
	float xmin = 0.0f;
	float ymax = 2.0f;
	float ymin = 0.0f;
	// 刻み値（飛び飛びの値になることもある）
	float deltx = 0.1f;
	float delty = 0.1f;
	float xRange = 0;
	float yRange = 0;
	bool chkframe = false;

	// ログの記録
	private static List<string> logMsg = new List<string>();
	public static void log(string msg) {
		logMsg.Add(msg);
		// 直近の5件のみ保存する
		if ( logMsg.Count > 50 ) {
			logMsg.RemoveAt( 0 );
		}
	}

	// Use this for initialization
	void Start () {
		vectorfieldsObj = GameObject.Find("ArCamera").GetComponent<VectorFields>();
		vectorfields = vectorfieldsObj.getVectorFields ();
		xRange = (xmax - xmin) / deltx;
		yRange = (ymax - ymin) / delty;
		m = Matrix4x4.TRS(translation, rotation, scale);

		for (int i = 0; i < vectorfields.Count; i++) {
			workVectorfields.Add (new VectorFieldInfo ());
			workVectorfields[i].setPostion (vectorfields[i].getPostion().x, 
				vectorfields[i].getPostion().y, vectorfields[i].getPostion().z);
			workVectorfields[i].setVectorField (vectorfields[i].getVectorField().x,
				vectorfields[i].getVectorField().y, vectorfields[i].getVectorField().z);
		}
		//log ("workVectorfields.Count = " + workVectorfields.Count.ToString ());
		Application.targetFrameRate = 15;
	}

	void InitializeIfNeeded()
	{
		if (m_System == null)
			m_System = GetComponent<ParticleSystem>();

		if (m_Particles == null || m_Particles.Length < m_System.maxParticles) {
			if (m_Particles == null) {
				m_Particles = new ParticleSystem.Particle[300];
			} else {
//				log ("m_Particles.Count1 : " + m_Particles.Length.ToString ());
				m_Particles = new ParticleSystem.Particle[m_Particles.Length];
			}
		}
	}

	private void LateUpdate()
	{
		int no;
		float xmin = 9999.0f;
		float xmax = -9999.0f;
		float ymin = 9999.0f;
		float ymax = -9999.0f;
		float sx = 0f;
		float sy = 0f;
		float wx = 0f;
		float wy = 0f;
		String strx;
		String stry;

		InitializeIfNeeded();

		int numParticlesAlive = m_System.GetParticles(m_Particles);
/*
		for (int i = 0; i < numParticlesAlive; i++) {
			point = m.MultiplyPoint3x4 (m_Particles [i].position);
			if (point.x < xmin)
				xmin = point.x;
			if (point.x > xmax)
				xmax = point.x;
			
			if (point.y < ymin)
				ymin = point.y;
			if (point.y > ymax)
				ymax = point.y;
		}

		log ("xmin : " + String.Format ("{0:f2}", xmin) + " xmax : " + String.Format ("{0:f2}", xmax));
		log ("ymin : " + String.Format ("{0:f2}", ymin) + " ymax : " + String.Format ("{0:f2}", ymax));
*/


		for (int i = 0; i < numParticlesAlive; i++) {
			point = m.MultiplyPoint3x4 (m_Particles [i].position);
			//point = m_Particles [i].position;
			strx = String.Format ("{0:f2}", point.x);
			stry = String.Format ("{0:f2}", point.y);

			wx = Convert.ToSingle (strx);
			wy = Convert.ToSingle (stry);
			//log ("WX : " + wx + " WY : " + wy);

			//sx = point.x - (point.y * 0.1f);
			//sy = point.y + (point.x * 0.1f);
			//point.x = sx;
			//point.y = sy;

			no = getfvector(wx, wy);
			if (no >= workVectorfields.Count)
				continue;
			if (no < 0)
				continue;

			if (no > NOMAX)
				NOMAX = no;
			if (no < NOMIN)
				NOMIN = no;
			//log ("NOMAX : " + NOMAX.ToString () + " NOMIN : " + NOMIN.ToString());
			point.x = m_Particles [i].position.x + (workVectorfields [no].getVectorField ().x * 2f);
			point.y = m_Particles [i].position.y + (workVectorfields [no].getVectorField ().y * 2f);

/*
			if (getSelectVectorX(wx, wy) < 0)
				continue;
			point.x = m_Particles [i].position.x + getSelectVectorX(wx, wy)*2f;
			if (getSelectVectorY(wx, wy) < 0)
				continue;
			point.y = m_Particles [i].position.y + getSelectVectorY(wx, wy)*2f;

			for (int j = 0; j < 50; j++) {				
				log ("dx1 : dx2 " + point.x.ToString() + " " + sx.ToString () );
				log ("dy1 : dy2 " + point.y.ToString () + " " + sy.ToString ());
			}
*/
			point.z = 0.5f;
			m_Particles [i].position = point;
		}
		m_System.SetParticles(m_Particles, numParticlesAlive);

	}

	private int getfvector(float x, float y) {
		float x1 = Mathf.Floor (x * 10f) / 10f; // 切り捨て
		float y1 = Mathf.Floor (y * 10f) / 10f; // 切り捨て
		int nx = (int)(x1 / deltx);
		int ny = (int)(y1 / delty);

		//log ("nx : " + String.Format ("{0:f2}", nx) + " ny : " + String.Format ("{0:f2}", ny));
		int no = (int)(nx + ny * (xRange + 1));
		return no;
	}

	private float getSelectVectorX(float x, float y) {
		if ((x == 0) && (y == 0))
			return 0;
		
		float x1 = Mathf.Floor (x * 10f) / 10f; // 切り捨て
		float y1 = Mathf.Floor (y * 10f) / 10f; // 切り捨て
		if ((x1 < 0) || (x1 >= xmax))
			return -1;
		if ((y1 < 0) || (y1 >= ymax))
			return -1;
		
		float x2 = x1 + 0.1f;
		float y2 = y1 + 0.1f;
		float dx = x - x1;
		float dy = y - y1;

		int no = checkVector (x1, y1);
		if (no >= workVectorfields.Count)
			return -1;
		float d1 = workVectorfields [no].getVectorField ().x;
		no = checkVector (x2, y1);
		if (no >= workVectorfields.Count)
			return -1;
		float d2 = workVectorfields [no].getVectorField ().x;
		no = checkVector (x1, y2);
		if (no >= workVectorfields.Count)
			return -1;
		float d3 = workVectorfields [no].getVectorField ().x;
		no = checkVector (x2, y2);
		if (no >= workVectorfields.Count)
			return -1;
		float d4 = workVectorfields [no].getVectorField ().x;

		float z1 = linear(dx, d1, d2);
		float z2 = linear(dx, d3, d4);
		float z3 = linear(dy, z1, z2);
		return z3;
	}

	private float getSelectVectorY(float x, float y) {
		if ((x == 0) && (y == 0))
			return 0;
		
		float x1 = Mathf.Floor (x * 10f) / 10f; // 切り捨て
		float y1 = Mathf.Floor (y * 10f) / 10f; // 切り捨て
		if ((x1 < 0) || (x1 >= xmax))
			return 0;
		if ((y1 < 0) || (y1 >= ymax))
			return 0;

		float x2 = x1 + 0.1f;
		float y2 = y1 + 0.1f;
		float dx = x - x1;
		float dy = y - y1;

		int no = checkVector (x1, y1);
		if (no >= workVectorfields.Count)
			return -1;		
		float d1 = workVectorfields [no].getVectorField ().y;
		no = checkVector (x2, y1);
		if (no >= workVectorfields.Count)
			return -1;
		float d2 = workVectorfields [no].getVectorField ().y;
		no = checkVector (x1, y2);
		if (no >= workVectorfields.Count)
			return -1;
		float d3 = workVectorfields [no].getVectorField ().y;
		no = checkVector (x2, y2);
		if (no >= workVectorfields.Count)
			return -1;
		float d4 = workVectorfields [no].getVectorField ().y;

		float z1 = linear(dx, d1, d2);
		float z2 = linear(dx, d3, d4);
		float z3 = linear(dy, z1, z2);
		return z3;
	}

	private int checkVector(float rx, float ry) {
		int nx = (int)Mathf.Abs(((xmin - rx) / deltx));
		int ny = (int)Mathf.Abs(((ymin - ry) / delty));
		int no = (int)(nx + ny * (xRange + 1));
		return no;
	}

	private float linear(float p, float d1, float d2) {
		return d1 * (0.1f - p) + d2 * p;
	}

	// Update is called once per frame
	void Update () {
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
