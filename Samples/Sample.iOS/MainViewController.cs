using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace Sample.iOS
{
	public class MainViewController : UIViewController
	{
		public MainViewController()
		{
			var button = UIButton.FromType(UIButtonType.System);
			button.SetTitle("Click Me!", UIControlState.Normal);
			button.Frame = new RectangleF(0, 100, this.View.Frame.Width, 60);

			button.TouchUpInside += (object sender, EventArgs e) => {
				var t = int.Parse(Title) + 1;
				Countly.Countly.SharedInstance.PostEvent(new Countly.Countly.CountlyEvent {
					Key = "Button Click",
					Count = 1
				});
				this.NavigationController.PushViewController(new MainViewController() {
					Title = t.ToString(),
				}, true);
			};

			this.Title = "0";
			this.View.BackgroundColor = UIColor.White;
			this.View.AddSubview(button);
		}
	}
}

