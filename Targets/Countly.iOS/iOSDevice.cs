using System;
using MonoTouch.UIKit;
using MonoTouch.AdSupport;
using MonoTouch.CoreTelephony;
using MonoTouch.Foundation;
using Constants = MonoTouch.Constants;
using System.Runtime.InteropServices;

namespace Countly
{
	public partial class DeviceInfo
	{
		public void Init()
		{
			DeviceName = DeviceHardware.DeviceVersion;
			OS = "iOS";
			OSVersion = UIDevice.CurrentDevice.SystemVersion;
			var prov = new CTTelephonyNetworkInfo().SubscriberCellularProvider;
			Carrier = prov == null ? "Unknown" : prov.CarrierName;
			UDID = GetUid();

			var bounds = UIScreen.MainScreen.Bounds;
			var scale = UIScreen.MainScreen.Scale;
			bounds.Width *= scale;
			bounds.Height *= scale;
			Resulution = string.Format("{0}x{1}", bounds.Width, bounds.Height);

			Local = NSLocale.CurrentLocale.LocaleIdentifier;
			AppVersion = NSBundle.MainBundle.InfoDictionary.ObjectForKey(new NSString("CFBundleVersion")).ToString();

			Metrics = getMetrics();
		}

		string GetUid()
		{
			var verson = Version.Parse(UIDevice.CurrentDevice.SystemVersion);
			if (verson.Major >= 6)
				return (ASIdentifierManager.SharedManager.AdvertisingIdentifier as NSUuid).AsString();

			try {
				return OpenUDID.Value;
			} 
			catch (Exception ex) {
				return "";
			}
		}
	}

	/// <summary>
	/// This code source is:
	/// http://snippets.dzone.com/user/zachgris
	/// 
	/// Detail descriptions how to determine what iPhone hardware is:
	/// http://www.drobnik.com/touch/2009/07/determining-the-hardware-model/    
	/// </summary>
	public class DeviceHardware
	{
		// make sure to add a 'using System.Runtime.InteropServices;' line to your file
		public const string HardwareProperty = "hw.machine";

		[DllImport(Constants.SystemLibrary)]
		internal static extern int sysctlbyname(
			[MarshalAs(UnmanagedType.LPStr)] string property, // name of the property
            IntPtr output, // output
            IntPtr oldLen, // IntPtr.Zero
            IntPtr newp, // IntPtr.Zero
            uint newlen // 0
		);

		public static string DeviceVersion {
			get {
				// get the length of the string that will be returned
				var pLen = Marshal.AllocHGlobal(sizeof(int));
				sysctlbyname(DeviceHardware.HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

				var length = Marshal.ReadInt32(pLen);

				// check to see if we got a length
				if (length == 0) {
					Marshal.FreeHGlobal(pLen);
					return "Unknown";
				}

				// get the hardware string
				var pStr = Marshal.AllocHGlobal(length);
				sysctlbyname(DeviceHardware.HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

				// convert the native string into a C# string
				var hardwareStr = Marshal.PtrToStringAnsi(pStr);
				
				Marshal.FreeHGlobal(pLen);
				Marshal.FreeHGlobal(pStr);
				return hardwareStr;
			}
		}
	}
}
