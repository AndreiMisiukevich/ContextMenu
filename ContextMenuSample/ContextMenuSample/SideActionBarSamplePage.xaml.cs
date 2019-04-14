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
            Device.BeginInvokeOnMainThread(() => GetParent<SideActionBarCell>(sender as View, (sender as View).Parent).ForceClose());
        }

        private void OnOpenClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            Device.BeginInvokeOnMainThread(() => GetParent<SideActionBarCell>(button, button.Parent).ForceOpen());
        }
    }
}
