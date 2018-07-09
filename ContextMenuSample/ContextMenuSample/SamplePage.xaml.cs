using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using ContextMenu;

namespace ContextMenuSample
{
	public partial class SamplePage : ContentPage
	{
		public SamplePage()
		{
			InitializeComponent();
			SampleList.ItemsSource = Enumerable.Range(0, 300);
		}

		private void OnClicked(object sender, EventArgs e)
		{
			var button = sender as Button;
			DisplayAlert($"{button.CommandParameter} clicked", null, "OK");

			(button.Parent.Parent.Parent.Parent as ContextViewCell).ForceClose();
		}
	}
}
