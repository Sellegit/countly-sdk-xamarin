using System;
using MonoTouch.UIKit;

namespace Sample.iOS
{
	public class MainViewController : UIViewController
	{
		public MainViewController()
		{
			var button = new UIButton(UIButtonType.RoundedRect);
			button.SetTitle("Test Button", UIControlState.Normal);
			button.SizeToFit();

			button.TouchUpInside += (object sender, EventArgs e) => {
				var t = int.Parse(Title) + 1;
				this.NavigationController.PushViewController(new MainViewController() {
					Title = t.ToString(),
				}, true);
			};

			this.Title = "0";
			this.View.AddSubview(button);
		}
	}

	public class MainView : UIView
	{
		public UIButton button;
		public UILabel UILabel;

		public MainView()
		{

		}
	}
}

