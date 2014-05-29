using System;
using Newtonsoft.Json;

namespace Countly
{
	public partial class DeviceInfo
	{
		static DeviceInfo shared;
		public static DeviceInfo Shared { get{ return shared ?? (shared = new DeviceInfo()); }}


		public DeviceInfo()
		{
			Init ();
		}

		public string UDID {get;private set;}

		public string OS { get; private set;}

		public string OSVersion {get;private set;}

		public string DeviceName {get;set;}

		public string Resulution {get;set;}

		public string Carrier {get;set;}

		public string Local {get;set;}

		public string Metrics {get;set;}

		public string AppVersion {get;set;}

		
		private class MetricsClass
		{
			public String _device { get; set; }
			public String _os { get; set; }
			public String _os_version { get; set; }
			public String _carrier { get; set; }
			public String _resolution { get; set; }
			public String _local { get; set; }
			public String _app_version { get; set; }

			public MetricsClass(DeviceInfo info)
			{
				_device = info.DeviceName;
				_os = info.OS;
				_os_version = info.OSVersion;
				_carrier = info.Carrier;
				_resolution = info.Resulution;
				_local = info.Local;
				_app_version = info.AppVersion;
			}
		}
		string getMetrics()
		{
			string output;

			//output = "{";
			//output +=       "\"" + "_device" +     "\"" + ":" + "\"" + getDevice() +       "\"";
			//output += "," + "\"" + "_os" +          "\"" + ":" + "\"" + getOS() +           "\"";
			//output += "," + "\"" + "_os_version" +  "\"" + ":" + "\"" + getOSVersion() +    "\"";
			//output += "," + "\"" + "_carrier" +     "\"" + ":" + "\"" + getCarrier() +      "\"";
			//output += "," + "\"" + "_resolution" +  "\"" + ":" + "\"" + getResolution() +   "\"";
			//output += "," + "\"" + "_local" +       "\"" + ":" + "\"" + getLocal() +        "\"";
			//output += "}";

			output = JsonConvert.SerializeObject(new MetricsClass(this), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

			//output = HttpUtility.UrlEncode(output);

			return output;
		}

	}
}

