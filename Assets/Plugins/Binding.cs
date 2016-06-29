using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

public class Binding {

#if UNITY_IPHONE || UNITY_XBOX360
		[DllImport("__Internal")]
		private static extern string CurrentLanguage ();
		[DllImport("__Internal")]
		private static extern void UpdateTexture(System.IntPtr data, int width, int height);
		[DllImport("__Internal")]
		private static extern void DetectHsv(System.IntPtr src, System.IntPtr dest, int width, int height);
		[DllImport("__Internal")]
		private static extern void convertToCannyTextureA(System.IntPtr src, 
	                           int width, int height, float threshold1, float threshold2);
		[DllImport("__Internal")]
		private static extern void OpticalFlow1(System.IntPtr src, System.IntPtr pre, int width, int height);
		[DllImport("__Internal")]
		private static extern void OpticalFlow2(System.IntPtr src, int width, int height);
		[DllImport("__Internal")]
		private static extern void OpticalFlow3(System.IntPtr src, System.IntPtr pre, int width, int height);
		[DllImport("__Internal")]
		private static extern void createCheckTexture(System.IntPtr data, int w, int h, int ch);
		[DllImport("__Internal")]
		private static extern int SetTextureOfCam1(System.IntPtr data, int w, int h);
#endif
#if UNITY_ANDROID
	[DllImport("ImageModulePlugin")]
		private static extern string CurrentLanguage ();
		[DllImport("ImageModulePlugin")]
		private static extern void UpdateTexture(System.IntPtr data, int width, int height);
		[DllImport("ImageModulePlugin")]
		private static extern void DetectHsv(System.IntPtr src, System.IntPtr dest, int width, int height);
		[DllImport("ImageModulePlugin")]
		private static extern void convertToCannyTextureA(System.IntPtr src, 
	                           int width, int height, float threshold1, float threshold2);
		[DllImport("ImageModulePlugin")]
		private static extern void OpticalFlow1(System.IntPtr src, System.IntPtr pre, int width, int height);
		[DllImport("ImageModulePlugin")]
		private static extern void OpticalFlow2(System.IntPtr src, int width, int height);
		[DllImport("ImageModulePlugin")]
		private static extern void OpticalFlow3(System.IntPtr src, System.IntPtr pre, int width, int height);
		[DllImport("ImageModulePlugin")]
		private static extern void createCheckTexture(System.IntPtr data, int w, int h, int ch);
		[DllImport("ImageModulePlugin")]
		private static extern int SetTextureOfCam1(System.IntPtr data, int w, int h);
#endif


	public static string HybridCurrentLanguage () {
		Debug.Log("HybridCurrentLanguage!");
		#if UNITY_IPHONE
		if (Application.platform != RuntimePlatform.OSXEditor) {
			return CurrentLanguage();
		}
		#endif
		#if UNITY_ANDROID
		Debug.Log("UNITY_ANDROID1");
		if (Application.platform == RuntimePlatform.Android) {
			Debug.Log("UNITY_ANDROID2");
			return CurrentLanguage();
		}
		#endif
		return "Not Founf Unity Type";
	}

	public void HybridUpdateTexture(System.IntPtr data, int width, int height) {
		UpdateTexture(data, width, height);
	}

	public void HybridDetectHsv(System.IntPtr src, System.IntPtr dest, int width, int height) {
		DetectHsv(src, dest, width, height);
	}
	public void HybridconvertToCannyTextureA(System.IntPtr src, int width, int height,
	                                               float threshold1, float threshold2) {
		convertToCannyTextureA(src, width, height, threshold1, threshold2);
	}

	public void HybridOpticalFlow1(System.IntPtr src, System.IntPtr pre, int width, int height)
	{
		OpticalFlow1(src, pre, width, height);
	}

	public void HybridOpticalFlow2(System.IntPtr src, int width, int height)
	{
		OpticalFlow2(src, width, height);
	}

	public void HybridOpticalFlow3(System.IntPtr src, System.IntPtr pre, int width, int height)
	{
		OpticalFlow3(src, pre, width, height);
	}

	public void HybridCreateCheckTexture(System.IntPtr data, int w, int h, int ch)
	{
		createCheckTexture(data, w, h, ch);
	}

	public int HybridSetTextureOfCam1(System.IntPtr data, int w, int h)
	{
		return SetTextureOfCam1(data, w, h);
	}

}
