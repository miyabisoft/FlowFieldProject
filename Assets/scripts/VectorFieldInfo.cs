using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Callback = System.Action<string>;

public class VectorFieldInfo {
	Vector3 vectorFiled;
	Vector3 postion;

	public void setVectorField(float x, float y, float z) {
		vectorFiled.x = x;
		vectorFiled.y = y;
		vectorFiled.z = z;
	}
	public Vector3 getVectorField() {
		return vectorFiled;
	}

	public void setPostion(float x, float y, float z) {
		postion.x = x;
		postion.y = y;
		postion.z = z;
	}
	public Vector3 getPostion() {
		return postion;
	}
}
