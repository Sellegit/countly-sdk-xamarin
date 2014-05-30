using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Sample.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			NSString appClass = new NSString (@"MyUIApp");
			NSString delegateClass = new NSString (@"AppDelegate");
			UIApplication.Main (args, appClass, delegateClass);
		}
	}

	[Register("MyUIApp")]
	public class MyUIApp : UIApplication
	{
		public override void SendEvent (UIEvent theEvent)
		{
//			Console.WriteLine (theEvent);
//			
//			Console.WriteLine (theEvent.Subtype);
//			if (theEvent.Type == UIEventType.RemoteControl) {
//
//				Console.WriteLine (theEvent.Subtype);
//				switch (theEvent.Subtype) {
//
//					case UIEventSubtype.RemoteControlPause:
//					case UIEventSubtype.RemoteControlPlay:
//					case UIEventSubtype.RemoteControlTogglePlayPause:
//
//					break;
//					case UIEventSubtype.RemoteControlPreviousTrack:
//					//Util.Previous ();
//					break;
//
//					case UIEventSubtype.RemoteControlBeginSeekingForward:
//					case UIEventSubtype.RemoteControlNextTrack:
//
//					break;
//
//					default:
//					break;
//				}
//			} else 
			base.SendEvent (theEvent);
		}
	}
}
