using System;
using Android.Content;
using Android.Telephony;
using Android.Views;
using Android.OS;
using Android.Util;
using Android.Runtime;

namespace Countly
{
	public partial class DeviceInfo
	{
		public void Init()
		{
			DeviceName = Build.Model;
			OS = "Android";
			OSVersion = Build.VERSION.Release;
			Resolution = GetResolution();
			Locale = Java.Util.Locale.Default.ToString();
			AppVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;

			var telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
			UDID = telephonyManager == null ? "Unknown" : telephonyManager.DeviceId;
			Carrier = telephonyManager == null ? "Unknown" : telephonyManager.NetworkOperatorName;

			Metrics = getMetrics ();
		}

		string GetResolution()
		{
			var windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>(); 
			if (windowManager == null)
				return "Unknown";

			var display = windowManager.DefaultDisplay;

			var displayMetrics = new DisplayMetrics();
			display.GetMetrics(displayMetrics);

			return string.Format("{0}x{1}", displayMetrics.WidthPixels, displayMetrics.HeightPixels);
		}
	}
}

