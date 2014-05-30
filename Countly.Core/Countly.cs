using System;
using System.Net;
using System.Web;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// Please see http://support.count.ly/kb/reference/countly-server-api-reference

#if __ANDROID__
using Android.Content;
#endif

namespace Countly
{
    public partial class Countly
    {
        private static Countly sharedInstance;
        private Timer timer;
        private ConnectionQueue queue;
        private bool isVisible;
        private double unsentSessionLength;
        private double previousTime;
        private DateTime startTime = DateTime.Now;
        private List<CountlyEvent> eventqueue;

        private Timer EventTimer;

        public static Countly SharedInstance
        {
			get{ return sharedInstance ?? (sharedInstance = new Countly ()); }
        }

        private Countly()
        {
			queue = new ConnectionQueue();
            eventqueue = new List<CountlyEvent>();
            timer = new Timer( new TimerCallback( ( object o ) =>
                {
                    OnTimer();
                }), 
                null,
                120 * 1000,
                120 * 1000);
            isVisible = false;
            unsentSessionLength = 0;

            EventTimer = new Timer(new TimerCallback((o) =>
                {
                    if (eventqueue.Count != 0)
                    {
                        queue.QueueEvents(eventqueue);
                        eventqueue.Clear();
                    }
                }),
                null,
                Timeout.Infinite,
                Timeout.Infinite);
        }

		#if __ANDROID__
		public void init(Context context, string serverURL, string appKey)
		{
			queue.setServerURL(serverURL);
			queue.setAppKey(appKey);
			queue.setDeviceInfo(context);
		}
		#endif

		#if __IOS__
        public void init(string serverURL, string appKey)
        {
            queue.setServerURL(serverURL);
            queue.setAppKey(appKey);
			queue.setDeviceInfo();
        }
		#endif

        public void OnStart()
        {
            previousTime = (DateTime.Now - startTime).TotalMilliseconds / 1000;

            queue.beginSession();

            isVisible = true;
        }

        public void OnStop()
        {
            isVisible = false;

            double currentTime = (DateTime.Now - startTime).TotalMilliseconds / 1000;
            unsentSessionLength += currentTime - previousTime;

            int duration = (int)unsentSessionLength;
            queue.endSession(duration);
            unsentSessionLength -= duration;
        }

        public void PostEvent(CountlyEvent Event, int TimeoutTime = 300)
        {
            if (Event == null)
            {
                throw new ArgumentNullException("Event");
            }

            eventqueue.Add(Event);

            if (eventqueue.Count >= 5)
            {
                queue.QueueEvents(eventqueue);
                eventqueue.Clear();
                EventTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else
            {
                EventTimer.Change(TimeoutTime, Timeout.Infinite);
            }
        }

        private void OnTimer()
        {
            if (isVisible == false)
            {
                return;
            }

            double currentTime = (DateTime.Now - startTime).TotalMilliseconds / 1000;
            unsentSessionLength += currentTime - previousTime;
            previousTime = currentTime;

            int duration = (int)unsentSessionLength;
            queue.updateSession(duration);
            unsentSessionLength -= duration;
        }

        public class CountlyEvent
        {
            public CountlyEvent()
            {
                Key = "";
				Count = 1;
                //UsingSum = false;
                //UsingSegmentation = false;
            }

            /// <summary>
            /// String describing the event that has occured. (Mandatory)
            /// </summary>
            public string   Key     { get; set; }
            /// <summary>
            /// The number of times this event has occured. (Mandatory)
            /// </summary>
            public Int32    Count   { get; set; }

            /// <summary>
            /// Flags if Sum will be used in the event call. Automatically set
            /// when Sum is modified.
            /// </summary>
            //public bool     UsingSum    { get; set; }
            /// <summary>
            /// Value used in the summation of similar event. For example the price
            /// of an in app purchase, so total revenue can be monitored. (Optional)
            /// </summary>
            public double?   Sum     { get; set; }         // { get; set { UsingSum = true; Sum = value; } }

            /// <summary>
            /// Flags if Segmentation will be used in the event call. Automatically
            /// set when Segmentation is modified.
            /// </summary>
            //public bool                         UsingSegmentation   { get; set; }
            /// <summary>
            /// Used to define characteristics of the event which can be filtered by. (Optional)
            /// </summary>
            public Dictionary<String, String>   Segmentation    { get; set; }        // { get; set { UsingSegmentation = true; Segmentation = value; } }
        }
    }

    public class ConnectionQueue
    {
		private volatile PersistantQueue<string> queue = new PersistantQueue<string>();
        public Thread thread;
        //private volatile bool StopThread = false;
        private string AppKey;
        private string ServerURL;

		DeviceInfo deviceInfo;

        public void setAppKey(string input)
        {
            AppKey = input;
        }

        public void setServerURL(string input)
        {
            ServerURL = input.TrimEnd('/');
        }

		#if __ANDROID__
		public void setDeviceInfo(Context context)
		{
			deviceInfo = new DeviceInfo(context);
		}
		#endif

		#if __IOS__
		public void setDeviceInfo()
		{
			deviceInfo = new DeviceInfo();
		}
		#endif

        public void beginSession()
        {
            string data;
            data = "app_key=" + AppKey;
			data += "&" + "device_id=" + deviceInfo.UDID;
            data += "&" + "sdk_version=" + "1.0";
            data += "&" + "begin_session=" + "1";
			data += "&" + "metrics=" + deviceInfo.Metrics;

            queue.Enqueue(data);

            Tick();
        }

        public void updateSession(int duration)
        {
            string data;
            data = "app_key=" + AppKey;
			data += "&" + "device_id=" + deviceInfo.UDID;
            data += "&" + "session_duration=" + duration;

            queue.Enqueue(data);

            Tick();
        }

        public void endSession(int duration)
        {
            string data;
            data = "app_key=" + AppKey;
			data += "&" + "device_id=" + deviceInfo.UDID;
            data += "&" + "end_session=" + "1";
            data += "&" + "session_duration=" + duration;

            queue.Enqueue(data);
            Tick();

            //ManualResetEvent Restart = new ManualResetEvent(false);

            //ThreadPool.QueueUserWorkItem((o) =>
            //    {
            //        HttpWebRequest Request = WebRequest.CreateHttp(ServerURL + "/i?" + data);

            //        Request.BeginGetResponse(new AsyncCallback((Async) =>
            //            {
            //                HttpWebResponse Responce = (HttpWebResponse)((HttpWebRequest)Async.AsyncState).EndGetResponse(Async);
            //                String s = (new StreamReader(Responce.GetResponseStream())).ReadToEnd();
            //                Restart.Set();
            //            }), Request);
            //    });
            //Restart.WaitOne();

            //Tick();
            //thread.Join();
            /*
            StopThread = true;
            if (null != thread && ThreadState.Unstarted != thread.ThreadState)
            {
                thread.Join();
            }

            ManualResetEvent Continue = new ManualResetEvent(false);

            foreach (String CurrentQuery in queue)
            {
                try
                {
                    WebClient Downloader = new WebClient();
                    Downloader.OpenReadCompleted += new OpenReadCompletedEventHandler((object sender, OpenReadCompletedEventArgs args) =>
                        {
                            try
                            {
                                if (null != args.Error)
                                {
                                    throw args.Error;
                                }

#if (DEBUG)
                                Debug.WriteLine("Countly:\t" + "ok -> " + CurrentQuery);
#endif
                            }
                            catch (Exception E)
                            {
                            }
                            finally
                            {
                                if (null != args.Result)
                                {
                                    args.Result.Close();
                                }

                                Continue.Set();
                            }
                        });
                    Downloader.OpenReadAsync(new Uri(ServerURL + "/i?" + CurrentQuery, UriKind.Absolute));
                    Continue.WaitOne();
                }
                catch (Exception E)
                {
#if (DEBUG)
#endif
                }
                finally
                {
                    Continue.Reset();
                }
            }
            */
        }

        public void QueueEvents(List<Countly.CountlyEvent> Events)
        {
            string data = "";
            data += "app_key=" + AppKey;
			data += "&" + "device_id=" + deviceInfo.UDID;
            data += "&" + "events=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(Events, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() }));

            //bool First1 = true;
            //foreach(Countly.CountlyEvent CurrentEvent in Events)
            //{
            //    if (First1)
            //    {
            //        First1 = false;
            //    }
            //    else
            //    {
            //        data += ",";
            //    }
            //    data += "{";
            //    data += "\"" + "key" + "\"" + ":" + "\"" + CurrentEvent.Key + "\"" + ",";
            //    data += "\"" + "count" + "\"" + ":" + CurrentEvent.Count;
            //    if (CurrentEvent.UsingSum)
            //    {
            //        data += ",";
            //        data += "\"" + "sum" + "\"" + ":" + CurrentEvent.Sum;
            //    }
            //    if (CurrentEvent.UsingSegmentation)
            //    {
            //        data += ",";
            //        data += "\"" + "segmentation" + "\"" + ":" + "{";

            //        bool First2 = true;
            //        foreach (String CurrentKey in CurrentEvent.Segmentation.Keys)
            //        {
            //            if (First2)
            //            {
            //                First2 = false;
            //            }
            //            else
            //            {
            //                data += ",";
            //            }
            //            data += "\"" + CurrentKey + "\"" + ":" + "\"" + CurrentEvent.Segmentation[CurrentKey] + "\"";
            //        }

            //        data += "}";
            //    }
            //    data += "}";
            //}

            //data += "]";

            queue.Enqueue(data);
        }

        private ManualResetEvent WakeUpWorker = new ManualResetEvent(false);

        private void Tick()
        {
            if (queue.Count == 0)
            {
                return;
            }

            if (thread == null)
            {
                thread = new Thread(new ParameterizedThreadStart((o) =>
                    {
                        String Data = "";

                        while (true)
                        {
                            if (queue.Count != 0)
                            {
                                try
                                {
                                    Data = queue.Peek();

                                    ManualResetEvent WaitForResponce = new ManualResetEvent(false);
                                    HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(ServerURL + "/i?" + Data);
                                    Exception InnerException = null;

                                    Request.BeginGetResponse(new AsyncCallback((Async) =>
                                        {
                                            try
                                            {
                                                HttpWebResponse Responce = (HttpWebResponse)((HttpWebRequest)Async.AsyncState).EndGetResponse(Async);
#if (DEBUG)
                                                String ReturnedString = (new StreamReader(Responce.GetResponseStream())).ReadToEnd();
                                                Debug.WriteLine("Countly:\t" + "returned -> " + ReturnedString);
                                                Debug.WriteLine("Countly:\t" + "ok -> " + Data);
#endif
                                                queue.Dequeue();
                                            }
                                            catch (Exception E)
                                            {
                                                InnerException = E;
                                            }
                                            WaitForResponce.Set();
                                        }), Request);

                                    WaitForResponce.WaitOne();

                                    if (InnerException != null)
                                    {
                                        throw InnerException;
                                    }
                                }
                                catch (Exception E)
                                {
#if (DEBUG)
                                    Debug.WriteLine("Countly:\t" + E.ToString());
                                    Debug.WriteLine("Countly:\t" + "error -> " + Data);
#endif
                                    WakeUpWorker.WaitOne(5000);
                                    WakeUpWorker.Reset();
                                }
                            }
                            else
                            {
                                WakeUpWorker.WaitOne(5000);
                                WakeUpWorker.Reset();
                            }
                        }
                    }));

                thread.IsBackground = true;
#if (DEBUG)
                thread.Name = "Countly Worker Thread";
#endif
                //StopThread = false;

                thread.Start();
            }
            else
            {
                WakeUpWorker.Set();
            }

//            thread = new Thread(new ThreadStart(() =>
//                {
//                    string data = string.Empty;
//                    ManualResetEvent signal = new ManualResetEvent(false);

//                    while (!StopThread)
//                    {
//                        try
//                        {
//                            data = queue.Peek();

//                            WebClient client = new WebClient();
//                            client.OpenReadCompleted += new OpenReadCompletedEventHandler((object sender, OpenReadCompletedEventArgs args) =>
//                                {
//                                    try
//                                    {
//                                        if (args.Error != null)
//                                        {
//                                            throw args.Error;
//                                        }

//                                        args.Result.Close();
//#if (DEBUG)
//                                        Debug.WriteLine("Countly:\t" + "ok -> " + data);
//#endif
//                                        queue.Dequeue();
//                                    }
//                                    catch (Exception E)
//                                    {
//#if (DEBUG)
//                                        Debugger.Break();

//                                        if (E is WebException)
//                                        {
//                                            Debug.WriteLine("Countly:\t" + (E as WebException).Message + "\t" + (E as WebException).Status);
//                                        }
//#endif
//                                    }
//                                    finally
//                                    {
//                                        signal.Set(); // Allow working thread to continue
//                                    }
//                                });

//                            signal.Reset();

//                            client.OpenReadAsync(new Uri(ServerURL + "/i?" + data));

//                            signal.WaitOne(); // Block thread until 'OpenReadCompleted' method has completed
//                        }
//                        catch (Exception E)
//                        {
//                            if (E is ThreadAbortException)
//                            {
//#if (DEBUG)
//                                Debug.WriteLine("Countly:\t" + "Worker thread abort recieved and respected");
//#endif
//                                return;
//                            }

//                            if (E is InvalidOperationException)
//                            {
//                                //return;
//                            }

//#if (DEBUG)
//                            Debug.WriteLine("Countly:\t" + E.ToString());
//                            Debug.WriteLine("Countly:\t" + "error -> " + data);
//#endif
//                            break;
//                        }
//                        finally
//                        {
//                            data = string.Empty;
//                        }
//                    }
//                }));


        }
    }

}
