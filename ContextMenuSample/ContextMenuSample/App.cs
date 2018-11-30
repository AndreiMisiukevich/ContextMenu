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
            MainPage = new NavigationPage(new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new StackLayout
                    {
                        Children = {
                            new Button
                            {
                                Text = "Side Action Bar",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new SideActionBarSamplePage()))
                            },
                            new Button
                            {
                                Text = "Move To Action",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new MoveToActionSamplePage()))
                            },
                            new Button
                            {
                                Text = "Move To Action with AutoClose enabled",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new MoveToActionAutoCloseSamplePage()))
                            }
                        }
                    }
                }
            });
        }
    }
}
