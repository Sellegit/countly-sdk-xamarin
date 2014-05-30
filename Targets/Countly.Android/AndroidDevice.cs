using System;
using System.Globalization;
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
			var telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);

			UDID = telephonyManager.DeviceId;
			DeviceName = Build.Model;
			OS = "Android";
			OSVersion = Build.VERSION.Release;
			Carrier = telephonyManager.NetworkOperatorName;

			var windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>(); 
			var display = windowManager.DefaultDisplay;

			var displayMetrics = new DisplayMetrics();
			display.GetMetrics(displayMetrics);

			Resulution = string.Format("{0}x{1}", displayMetrics.WidthPixels, displayMetrics.HeightPixels);

			Local = RegionInfo.CurrentRegion.Name;
			AppVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;

			Metrics = getMetrics ();
		}
	}
}

