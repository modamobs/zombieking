﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidShareUsing : MonoBehaviour
{
	private const string subject = "쌉 좀비 나이트 키우기!~";
	private const string body = "https://play.google.com/store/apps/details?id=com.two.people.zombie&hl=ko&ah=DMVucFo2qsRMMUDFz1pQPdIWrgY";

	public void Share()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent")) 
		using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent")) {
			intentObject.Call<AndroidJavaObject>("setAction", intentObject.GetStatic<string>("ACTION_SEND"));
			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
			intentObject.Call<AndroidJavaObject>("putExtra", intentObject.GetStatic<string>("EXTRA_SUBJECT"), subject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentObject.GetStatic<string>("EXTRA_TEXT"), body);
			using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity")) 
			using (AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via"))
			currentActivity.Call("startActivity", jChooser);
		}
#endif
	}
}