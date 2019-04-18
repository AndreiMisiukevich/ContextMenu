using Xamarin.Forms;
using System;
using System.Threading.Tasks;

namespace ContextMenu
{
    public abstract class BaseActionViewCell : ViewCell
    {
        private double _originalMainContentWidth;
        private bool _isFirst = true;

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(SideActionBarCell), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseActionViewCell).SetContentView(content: newValue as View);
        });

        public static readonly BindableProperty OuterLeftContentProperty = BindableProperty.Create(nameof(OuterLeftContent), typeof(View), typeof(View), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseActionViewCell).SetContentView(outerLeftContent: newValue as View);
            ((View)newValue).SizeChanged += new EventHandler((sender, e) => BaseActionViewCell_SizeChanged(sender, e, bindable as BaseActionViewCell));
        });

        public static readonly BindableProperty OuterRightContentProperty = BindableProperty.Create(nameof(OuterRightContent), typeof(View), typeof(View), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseActionViewCell).SetContentView(outerRightContent: newValue as View);
            ((View)newValue).SizeChanged += new EventHandler((sender, e) => BaseActionViewCell_SizeChanged(sender, e, bindable as BaseActionViewCell));
        });

        private static void BaseActionViewCell_SizeChanged(object sender, EventArgs e, BaseActionViewCell baseActionViewCell)
        {
            baseActionViewCell.SetMainContentWidth();
        }

        public static readonly BindableProperty ContextTemplateProperty = BindableProperty.Create(nameof(ContextTemplate), typeof(DataTemplate), typeof(SideActionBarCell), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseActionViewCell).IsContextChanged = true;
        });

        public static readonly BindableProperty IsAutoCloseEnabledProperty = BindableProperty.Create(nameof(IsAutoCloseEnabled), typeof(bool), typeof(MoveToActionCell), true);

        public event Action<BaseActionViewCell> ContextMenuOpened;

        public event Action<BaseActionViewCell> ContextMenuClosed;

        public event Action<BaseActionViewCell> TouchStarted;

        public event Action<BaseActionViewCell> TouchEnded;

        private bool IsContextChanged { get; set; }

        public View Content
        {
            get => GetValue(ContentProperty) as View;
            set => SetValue(ContentProperty, value);
        }

        public View OuterLeftContent
        {
            get => GetValue(OuterLeftContentProperty) as View;
            set => SetValue(OuterLeftContentProperty, value);
        }

        public View OuterRightContent
        {
            get => GetValue(OuterRightContentProperty) as View;
            set => SetValue(OuterRightContentProperty, value);
        }

        public DataTemplate ContextTemplate
        {
            get => GetValue(ContextTemplateProperty) as DataTemplate;
            set => SetValue(ContextTemplateProperty, value);
        }

        public bool IsAutoCloseEnabled
        {
            get => (bool)GetValue(IsAutoCloseEnabledProperty);
            set => SetValue(IsAutoCloseEnabledProperty, value);
        }

        public double MainContentWidth { get; set; }

        protected ContentView AllContent { get; } = new ContentView();

        protected ContextMenuScrollView Scroll { get; } = new ContextMenuScrollView();

        public void ForceClose(bool animated = true)
        => Scroll.ForceCloseContextMenu(Scroll, animated);

        public virtual async void ForceOpen(bool animated = true)
        {
            SetContextViewIfNeeded();
            var context = Scroll.ContextView;
            if(context == null)
            {
                return;
            }
            var width = Math.Max(context.Width, context.WidthRequest);
            var widthCompletionSource = new TaskCompletionSource<bool>();
            if(width <= 0)
            {
                EventHandler onSizeChanged = null;
                onSizeChanged = (sender, e) =>
                {
                    var v = (View)sender;
                    if (v.Width > 0 && v.Height > 0)
                    {
                        v.SizeChanged -= onSizeChanged;
                        widthCompletionSource.SetResult(true);
                    }
                };
                context.SizeChanged += onSizeChanged;
            }
            else
            {
                widthCompletionSource.SetResult(true);
            }
            await widthCompletionSource.Task;
            Scroll.ForceOpenContextMenu(Scroll, animated);
        }

        protected void SetMainContentWidth()
        {
            if (Content == null)
            {
                MainContentWidth = 0;
                return;
            }

            if (_isFirst)
            {
                _originalMainContentWidth = Content.WidthRequest;
                _isFirst = false;
            }
            MainContentWidth = _originalMainContentWidth;

            if (OuterLeftContent != null)
            {
                MainContentWidth -= OuterLeftContent.Width;
            }

            if (OuterRightContent != null)
            {
                MainContentWidth -= OuterRightContent.Width;
            }

            Content.WidthRequest = MainContentWidth;
        }

        protected void SetContentView(View content = null, View outerLeftContent = null, View outerRightContent = null)
        {
            if (content == null && outerLeftContent == null && outerRightContent == null)
            {
                return;
            }

            if (content != null)
            {
                Scroll.ContentView = content;
            }
            if (outerLeftContent == null && outerRightContent == null)
            {
                AllContent.Content = Scroll;
            }
            else if (outerLeftContent != null && outerRightContent == null)
            {
                Grid _grid = new Grid
                {
                    ColumnSpacing = 0
                };

                _grid.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition()
                };

                _grid.ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Star) }
                };

                _grid.Children.Add(outerLeftContent);
                Grid.SetRow(outerLeftContent, 0);
                Grid.SetColumn(outerLeftContent, 0);

                _grid.Children.Add(Scroll);
                Grid.SetRow(Scroll, 0);
                Grid.SetColumn(Scroll, 1);

                AllContent.Content = _grid;
            }
            else if (outerLeftContent == null && outerRightContent != null)
            {
                Grid _grid = new Grid();

                _grid.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition()
                };

                _grid.ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Auto) }
                };

                _grid.Children.Add(Scroll);
                Grid.SetRow(Scroll, 0);
                Grid.SetColumn(Scroll, 0);

                _grid.Children.Add(outerRightContent);
                Grid.SetRow(outerRightContent, 0);
                Grid.SetColumn(outerRightContent, 1);

                AllContent.Content = _grid;
            }
            else if (outerLeftContent != null && outerRightContent != null)
            {
                Grid _grid = new Grid();

                _grid.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition()
                };

                _grid.ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,  GridUnitType.Auto) }
                };

                _grid.Children.Add(outerLeftContent);
                Grid.SetRow(outerLeftContent, 0);
                Grid.SetColumn(outerLeftContent, 0);

                _grid.Children.Add(Scroll);
                Grid.SetRow(Scroll, 0);
                Grid.SetColumn(Scroll, 1);

                _grid.Children.Add(outerRightContent);
                Grid.SetRow(outerRightContent, 0);
                Grid.SetColumn(outerRightContent, 2);

                AllContent.Content = _grid;
            }
        }

        protected abstract void SetContextView(View context);

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Scroll.TouchStarted += OnTouchStarted;
            Scroll.TouchEnded += OnTouchEnded;
            Scroll.ActionBarOpened += OnContextMenuOpened;
            Scroll.ActionBarClosed += OnContextMenuClosed;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Scroll.TouchStarted -= OnTouchStarted;
            Scroll.TouchEnded -= OnTouchEnded;
            Scroll.ActionBarOpened -= OnContextMenuOpened;
            Scroll.ActionBarClosed -= OnContextMenuClosed;
        }

        protected override void OnBindingContextChanged()
        {
            IsContextChanged = true;
            ForceClose(false);
            base.OnBindingContextChanged();
        }

        private void OnTouchStarted()
        {
            TouchStarted?.Invoke(this);
            SetContextViewIfNeeded();
        }

        private void OnTouchEnded() => TouchEnded?.Invoke(this);

        private void SetContextViewIfNeeded()
        {
            if (IsContextChanged)
            {
                IsContextChanged = false;

                var template = ContextTemplate is DataTemplateSelector selector
                    ? selector.SelectTemplate(BindingContext, this)
                      : ContextTemplate;

                if (template == null)
                {
                    return;
                }
                SetContextView(template.CreateContent() as View);
            }
        }

        private void OnContextMenuOpened() => ContextMenuOpened?.Invoke(this);

        private void OnContextMenuClosed() => ContextMenuClosed?.Invoke(this);
    }
}
