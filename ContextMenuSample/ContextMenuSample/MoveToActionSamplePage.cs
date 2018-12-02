using System;
using Xamarin.Forms;
using ContextMenu;
using System.Linq;
using System.Collections.ObjectModel;

namespace ContextMenuSample
{
    public class MoveToActionSamplePage : BaseContentPage
    {
        public MoveToActionSamplePage()
        {
            var items = new ObservableCollection<int>(Enumerable.Range(0, 300));

            var sampleList = new ListView
            {
                RowHeight = 80,
                BackgroundColor = Color.Black,
                SeparatorVisibility = SeparatorVisibility.None,
                ItemsSource = items
            };

            sampleList.ItemTemplate = new DataTemplate(() => new MoveToActionCell
            {
                IsAutoCloseEnabled = false,
                MovedCommand = new Command(p => items.Remove((int)p)),
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
                }.With(v => v.SetBinding(WidthRequestProperty, new Binding { Source = sampleList, Path = nameof(Width) })),
                ContextTemplate = new DataTemplate(() => new StackLayout
                {
                    Margin = new Thickness(0, 5),
                    BackgroundColor = Color.Red,
                    Children = {
                        new Label
                        {
                            Text = "Move to Delete",
                            TextColor = Color.White,
                            FontAttributes = FontAttributes.Bold,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center
                        }
                    }
                })
            });

            Content = sampleList;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            DisplayAlert($"{button.CommandParameter} clicked", null, "OK");
            GetParent<MoveToActionCell>(button, button.Parent).ForceClose();
        }
    }
}
