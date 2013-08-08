using System;
using Newtonsoft.Json;
using System.Reflection;

namespace Countly
{
	public partial class DeviceInfo
	{

		public void Init()
		{
			UDID = OpenUDID.value;
			OS = "Windows Phone";
			OSVersion = Environment.OSVersion.Version.ToString();
			DeviceName = DeviceStatus.DeviceName;
			Resulution = getResolution ();
			Carrier = DeviceNetworkInformation.CellularMobileOperator;
			Local = CultureInfo.CurrentCulture.Name;

			var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			AppVersion = versionAttrib.Version.ToString();   

			Metrics = getMetrics ();
		}

		public static string getResolution()
		{
			int ScaleFactor = 0;
			double LogicalWidth = 0.0;
			double LogicalHeight = 0.0;

			if (Environment.OSVersion.Version.Major < 8.0)
			{
				return "800x480";
			}

			if (Deployment.Current.Dispatcher.CheckAccess())
			{
				#if WP8
				ScaleFactor = Application.Current.Host.Content.ScaleFactor;
				#else
				ScaleFactor = (int)Application.Current.Host.Content.GetType().GetProperty("ScaleFactor").GetGetMethod().Invoke(Application.Current.Host.Content, null);
				#endif
			}
			else
			{
				ManualResetEvent Continue = new ManualResetEvent(false);
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				                                          {
					#if WP8
					ScaleFactor = Application.Current.Host.Content.ScaleFactor;
					#else
					ScaleFactor = (int)Application.Current.Host.Content.GetType().GetProperty("ScaleFactor").GetGetMethod().Invoke(Application.Current.Host.Content, null);
					#endif
					LogicalWidth = Application.Current.Host.Content.ActualWidth;
					LogicalHeight = Application.Current.Host.Content.ActualHeight;

					Continue.Set();
				});
				Continue.WaitOne();
			}

			double Scale = (double)ScaleFactor / 100;

			return (LogicalHeight * Scale).ToString("F0") + "x" + (LogicalWidth * Scale).ToString("F0");

			//switch (ScaleFactor)
			//{
			//    case 100:
			//        return "800x480";

			//    case 150:
			//        return "1280x720";

			//    case 160:
			//        return "1280x768";

			//    default:
			//        return "???";
			//}
		}
	
	}
}

