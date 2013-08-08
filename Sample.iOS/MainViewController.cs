using System;
using MonoTouch.UIKit;

namespace Sample.iOS
{
	public class MainViewController : UIViewController
	{
		public MainViewController ()
		{
		}
		public override void LoadView ()
		{
			View = new MainView ();
		}
	}

	public class MainView : UIView
	{
		public UIButton button;
		public UILabel UILabel;
		public MainView()
		{
			button = new UIButton (UIButtonType.RoundedRect);
			button.SetTitle ("Test Button", UIControlState.Normal);
			button.SizeToFit ();
			this.AddSubview (button);
		}
	}
}

