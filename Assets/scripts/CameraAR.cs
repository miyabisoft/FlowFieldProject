using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

public class CameraAR {
	#if UNITY_ANDROID
		private AndroidJavaObject cameraARObj;
		[DllImport ("cameralib")]
		private static extern int getWidth();
		[DllImport ("cameralib")]
		private static extern int getHeight();
		[DllImport ("cameralib")]
		private static extern int getCheckData();
		[DllImport ("cameralib")]
		private static extern void setCheckData(int val);
		[DllImport ("cameralib")]
		private static extern void setImadeDatatoUnity(IntPtr data);
		[DllImport ("cameralib")]
		private static extern void SetTextureFromUnity(System.IntPtr texture);
		[DllImport ("cameralib")]
		private static extern IntPtr GetRenderEventFunc();
	#endif
	#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void startGLCamera();
		[DllImport("__Internal")]
		private static extern int getTexturePtr();
		[DllImport("__Internal")]
		private static extern void setImageBuffer(System.IntPtr data);
	#endif

	// Class
	public CameraAR() {
	}

	// Open Camera
	public int open() {
		int ret = 0;
		#if UNITY_ANDROID
			cameraARObj = new AndroidJavaObject("miyabi.com.camerapreview311.CameraPluginModule");
			ret = cameraARObj.Call<int>("startCamera", "");
			return(ret);
		#endif
		#if UNITY_IPHONE
			startGLCamera();
			return(1);
		#endif
		#if UNITY_EDITOR
			return(1);
		#endif
	}

	public int getwidth() {
		#if UNITY_ANDROID
			return(getWidth());
		#endif
		#if UNITY_IPHONE || UNITY_EDITOR
			return(0);
		#endif
	}

	public int getheight() {
		#if UNITY_ANDROID
			return(getHeight());
		#endif
		#if UNITY_IPHONE || UNITY_EDITOR
			return(0);
		#endif
	}

	// Update Camera Image
	public void UpdateBufferImage(System.IntPtr data ) {
		#if UNITY_ANDROID
			setImadeDatatoUnity(data);
		#endif
	}

	// Update Texture
	public void UpdateImage() {
		#if UNITY_ANDROID
			GL.IssuePluginEvent(GetRenderEventFunc(), 1);
		#endif
	}

	// Set Unity Texcure Pointer
	public void SetTexturePtr(System.IntPtr ptr) {
		#if UNITY_ANDROID
			SetTextureFromUnity(ptr);
		#endif
	}

	public System.IntPtr GetTexturePtr() {
		#if UNITY_IPHONE
			return((System.IntPtr)getTexturePtr());
		#endif
		return((System.IntPtr)0);
	}

	public void setTextureImage(System.IntPtr ptr) {
		#if UNITY_IPHONE || UNITY_EDITOR
			setImageBuffer(ptr);
		#endif
	}

	public void SetData(int val) {
		#if UNITY_ANDROID
			setCheckData(val);	
		#endif
	}

	public int getData() {
		#if UNITY_ANDROID
			return getCheckData();
		#endif
		#if UNITY_IPHONE || UNITY_EDITOR
			return(0);
		#endif
	}
}
