using System;
using Xamarin.Forms;
using ContextMenu;
using System.Linq;
using ContextMenuSample.Pages;
using ContextMenuSample.ViewModels;

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
                                Text = "SIDE MENU BAR",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new SideMenuPage()))
                            },
                            new Button
                            {
                                Text = "MOVE TO DELETE",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new MoveToDeletePage()))
                            },
                            new Button
                            {
                                Text = "MOVE TO MUTE (AUTOCLOSING)",
                                Command = new Command(() => MainPage.Navigation.PushAsync(new MoveToUpdatePage()))
                            }
                        }
                    }
                }
            });
        }
    }
}
