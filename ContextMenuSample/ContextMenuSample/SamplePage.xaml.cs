using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ContextMenuSample
{
	public partial class SamplePage : ContentPage
	{
		public SamplePage()
		{
			InitializeComponent();
			SampleList.ItemsSource = Enumerable.Range(0, 300);
		}
	}
}
