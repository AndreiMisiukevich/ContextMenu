using System;
using System.Linq;
using ContextMenu;
using Xamarin.Forms;

namespace ContextMenuSample
{
    public partial class SideActionBarSamplePage : BaseContentPage
    {
        public SideActionBarSamplePage()
        {
            InitializeComponent();
            SampleList.ItemsSource = Enumerable.Range(0, 300);
        }

        private void OnClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            DisplayAlert($"{button.CommandParameter} clicked", null, "OK");

            Device.BeginInvokeOnMainThread(() => GetParent<SideContextMenuView>(button).ForceClose());
        }

        private void OnOpenClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            Device.BeginInvokeOnMainThread(() => GetParent<SideContextMenuView>(button).ForceOpen());
        }
    }
}
