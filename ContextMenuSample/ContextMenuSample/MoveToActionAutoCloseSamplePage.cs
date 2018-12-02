using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ContextMenu;
using ContextMenuSample.Converters;
using Xamarin.Forms;

namespace ContextMenuSample
{
    public class MoveToActionAutoCloseSamplePage : BaseContentPage
    {
        public MoveToActionAutoCloseSamplePage()
        {
            var sampleList = new ListView
            {
                RowHeight = 80,
                BackgroundColor = Color.Black,
                SeparatorVisibility = SeparatorVisibility.None,
                ItemsSource = new ObservableCollection<MessageModel>(GenerateRandomMessages(50))
            };

            sampleList.ItemTemplate = new DataTemplate(() =>
            {
                var cell = new MoveToActionCell
                {
                    MovedCommand = new Command<MessageModel>(OnMovedExecuted),
                    Content = new ContentView
                    {
                        Margin = new Thickness(0, 5),
                        BackgroundColor = Color.White,
                        Content = new Label
                        {
                            TextColor = Color.Black,
                            FontSize = 16,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            VerticalOptions = LayoutOptions.CenterAndExpand
                        }.With(v => v.SetBinding(Label.TextProperty, new Binding(nameof(MessageModel.Message))))
                    }.With(v => v.SetBinding(WidthRequestProperty, new Binding { Source = sampleList, Path = nameof(Width) })),
                    ContextTemplate = new DataTemplate(() => new StackLayout
                    {
                        Margin = new Thickness(0, 5),
                        Children = { new Label
                            {
                                TextColor = Color.White,
                                FontAttributes = FontAttributes.Bold,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                HorizontalOptions = LayoutOptions.EndAndExpand,
                                HorizontalTextAlignment = TextAlignment.End,
                                VerticalTextAlignment = TextAlignment.Center
                            }.With(v => v.SetBinding(Label.TextProperty, new Binding(nameof(MessageModel.IsMuted), converter: new IsMutedToTextConverter())))
                        }
                    }.With(v => v.SetBinding(BackgroundColorProperty, new Binding(nameof(MessageModel.IsMuted), converter: new IsMutedToBackgroundColorConverter()))))
                };

                return cell;
            });

            Content = sampleList;
        }

        private void OnMovedExecuted(MessageModel messageModel)
        {
            messageModel.IsMuted = !messageModel.IsMuted;
            Console.WriteLine(messageModel);
        }

        #region Dummy methods

        private IList<MessageModel> GenerateRandomMessages(int count)
        {
            var result = new List<MessageModel>();

            for (int i = 0; i < count; i++)
            {
                result.Add(new MessageModel
                {
                    Message = $"Chat with {GetRandomName()} (index: {i})",
                    IsMuted = false
                });
            }

            return result;
        }

        private string GetRandomName()
        {
            var names = new List<string>
            {
            "Deeanna Duhaime",
            "Abby Abram", "Renda Reavis",
            "Susann Sites", "Jordan Jain", "Chase Casanova", "Shea Sandberg", "Luetta Lavigne",
            "Catalina Carte", "Domenica Dolson", "Harry Humbertson", "Diedre Dewees", "Gary Graver",
            "Eliza Enyeart", "Mahalia Margerum", "Olevia Orange", "Gisela Guthrie", "Leandro Leventhal",
             "Claudine Crisman", "Faustino Fairless"
             };
            var index = new Random().Next(0, names.Count);

            var result = names.ElementAt(index);
            return result;
        }

        #endregion
    }

    /// <summary>
    /// Dummy message entity.
    /// </summary>
    public class MessageModel : BindableObject
    {
        private bool _isMuted;

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        public string Message { get; set; }

        public override string ToString()
        {
            var mutedText = IsMuted ? "Muted" : "Unmuted";
            return $"{Message} => {mutedText}";
        }
    }
}