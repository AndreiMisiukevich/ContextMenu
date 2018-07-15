using System;
using Xamarin.Forms;
using ContextMenu;
using System.Linq;

namespace ContextMenuSample
{
	public class CodeSamplePage : ContentPage
	{
		public CodeSamplePage()
		{
			var sampleList = new ListView
			{
				RowHeight = 80,
				BackgroundColor = Color.Black,
				SeparatorVisibility = SeparatorVisibility.None,
				ItemsSource = Enumerable.Range(0, 300)
			};

			sampleList.ItemTemplate = new DataTemplate(() => new ContextViewCell
			{
				Content = new ContentView
				{
					Margin = new Thickness(0, 5),
					BackgroundColor = Color.White,
					Content = new Label
					{
						TextColor = Color.Black,
						FontSize = 30,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						VerticalOptions = LayoutOptions.CenterAndExpand
					}.With(v => v.SetBinding(Label.TextProperty, "."))
				}.With(v => v.SetBinding(WidthRequestProperty, new Binding { Source = sampleList, Path = nameof(Width)})),
				ContextTemplate = new DataTemplate(() => new StackLayout {
					Spacing = 0,
					Margin = new Thickness(0, 5),
					HorizontalOptions = LayoutOptions.Fill,
					Orientation = StackOrientation.Horizontal,
					Children = {
						new Button
						{
							WidthRequest = 80,
							BackgroundColor = Color.Red,
							TextColor = Color.Black,
							Text = "Red",
							CommandParameter = "Red",
							Margin = new Thickness(5, 0, 0, 0)
						}.With(v => v.Clicked += OnClicked),
						new Button
						{
							WidthRequest = 80,
							BackgroundColor = Color.Yellow,
							TextColor = Color.Black,
							Text = "Yellow",
							CommandParameter = "Yellow",
							Margin = new Thickness(5, 0, 5, 0),
						}.With(v => v.Clicked += OnClicked),
						new Button
						{
							WidthRequest = 80,
							BackgroundColor = Color.Green,
							TextColor = Color.Black,
							Text = "Green",
							CommandParameter = "Green",
							Margin = new Thickness(0, 0, 5, 0)
						}.With(v => v.Clicked += OnClicked)
					}
				})
			});

			Content = sampleList;
		}

		private void OnClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			DisplayAlert($"{button.CommandParameter} clicked", null, "OK");
			(button.Parent.Parent.Parent.Parent as ContextViewCell).ForceClose();
		}
	}

	public static class BindingExtensions
	{
		public static TView With<TView>(this TView view, Action<TView> action) where TView : View
		{
			action?.Invoke(view);
			return view;
		}
	}
}
