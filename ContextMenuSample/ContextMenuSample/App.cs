using System;
using Xamarin.Forms;
using ContextMenu;
using System.Linq;

namespace ContextMenuSample
{
	public class App : Application
	{
		public App()
		{
			MainPage = new SamplePage();
		}
	}

	public class SamplePage : ContentPage
	{
		public SamplePage()
		{
			var list = new ListView(Device.RuntimePlatform == Device.Android 
			                        ? ListViewCachingStrategy.RecycleElement 
			                        : ListViewCachingStrategy.RetainElement)
			{
				ItemTemplate = new DataTemplate(typeof(SampleCell)),
				ItemsSource = Enumerable.Range(0, 300),
				SeparatorVisibility = SeparatorVisibility.None,
				RowHeight = 60,
			};

			Content = list;
		}
	}

	public class SampleCell : ContextMenuViewCell
	{
		public SampleCell()
		{
			var r = new Random();
			var l = new Label
			{
				TextColor = Color.FromRgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)),
				FontSize = 40,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			l.SetBinding(Label.TextProperty, ".");

			Content = new ContentView
			{
				Margin = new Thickness(0, 5, 0, 0),
				BackgroundColor = Color.FromRgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)),
				Content = l
			};

			ContextTemplate = new DataTemplate(() => new StackLayout
			{
				WidthRequest = 252,
				Margin = new Thickness(0, 5, 0, 0),
				Orientation = StackOrientation.Horizontal,
				Children = {
					new Button
					{
						WidthRequest = 80,
						BackgroundColor = Color.Red,
						TextColor = Color.Black,
						Text = "Red",
						CommandParameter = "Red",
						Command = new Command((p) => {
							Application.Current.MainPage.DisplayAlert(p.ToString(), null, "Ok");
						})
					},
					new Button
					{
						WidthRequest = 80,
						BackgroundColor = Color.Yellow,
						TextColor = Color.Black,
						Text = "Yellow",
						CommandParameter = "Yellow",
						Command = new Command((p) => {
							Application.Current.MainPage.DisplayAlert(p.ToString(), null, "Ok");
						})
					},
					new Button
					{
						WidthRequest = 80,
						BackgroundColor = Color.Green,
						TextColor = Color.Black,
						Text = "Green",
						CommandParameter = "Green",
						Command = new Command((p) => {
							Application.Current.MainPage.DisplayAlert(p.ToString(), null, "Ok");
						})
					}
				}
			});
		}

		protected override void OnAppearing() // setting content width according to Parent width
		{
			base.OnAppearing();
			var p = Parent as View;
			if (p != null)
			{
				Content.WidthRequest = p.Width;
			}
		}
	}
}
