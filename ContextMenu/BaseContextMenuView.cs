using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace ContextMenu
{
    public enum ScrollDirection { Close, Open }
    public enum ScrollState { Closed, Opened, Moving }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseContextMenuView : ScrollView
    {
        public static readonly BindableProperty ViewProperty = BindableProperty.Create(nameof(View), typeof(View), typeof(BaseContextMenuView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseContextMenuView).SetViewContent(newValue as View);
        });

        public static readonly BindableProperty ContextTemplateProperty = BindableProperty.Create(nameof(ContextTemplate), typeof(DataTemplate), typeof(BaseContextMenuView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseContextMenuView).IsContextChanged = true;
        });

        public static readonly BindableProperty AcceptWidthPercentageProperty = BindableProperty.Create(nameof(AcceptWidthPercentage), typeof(double), typeof(BaseContextMenuView), 0.33);

        public static readonly BindableProperty IsAutoCloseEnabledProperty = BindableProperty.Create(nameof(IsAutoCloseEnabled), typeof(bool), typeof(BaseContextMenuView), true);

        public static readonly BindableProperty ForceCloseCommandProperty = BindableProperty.Create(nameof(ForceCloseCommand), typeof(ICommand), typeof(BaseContextMenuView), null, BindingMode.OneWayToSource);

        public static readonly BindableProperty ForceOpenCommandProperty = BindableProperty.Create(nameof(ForceOpenCommand), typeof(ICommand), typeof(BaseContextMenuView), null, BindingMode.OneWayToSource);

        public event Action<BaseContextMenuView> ContextMenuOpened;
        public event Action<BaseContextMenuView> ContextMenuClosed;
        public event Action<BaseContextMenuView> TouchStarted;
        public event Action<BaseContextMenuView> TouchEnded;

        protected BaseContextMenuView()
        {
            Orientation = ScrollOrientation.Horizontal;
            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalScrollBarVisibility = ScrollBarVisibility.Never;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never;

            Content = ViewStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                Children = {
                    _contextView
                }
            };

            Scrolled += OnScrolled;

            ForceCloseCommand = new Command(parameter => ForceClose(parameter is bool boolean ? boolean : true));
            ForceOpenCommand = new Command(parameter => ForceOpen(parameter is bool boolean ? boolean : true));
        }

        private bool IsContextChanged { get; set; }

        public View View
        {
            get => GetValue(ViewProperty) as View;
            set => SetValue(ViewProperty, value);
        }

        public DataTemplate ContextTemplate
        {
            get => GetValue(ContextTemplateProperty) as DataTemplate;
            set => SetValue(ContextTemplateProperty, value);
        }

        public double AcceptWidthPercentage
        {
            get => (double)GetValue(AcceptWidthPercentageProperty);
            set => SetValue(AcceptWidthPercentageProperty, value);
        }

        public bool IsAutoCloseEnabled
        {
            get => (bool)GetValue(IsAutoCloseEnabledProperty);
            set => SetValue(IsAutoCloseEnabledProperty, value);
        }

        public ICommand ForceCloseCommand
        {
            get => (ICommand)GetValue(ForceCloseCommandProperty);
            set => SetValue(ForceCloseCommandProperty, value);
        }

        public ICommand ForceOpenCommand
        {
            get => (ICommand)GetValue(ForceOpenCommandProperty);
            set => SetValue(ForceOpenCommandProperty, value);
        }

        public void ForceClose(bool animated = true)
            => ForceCloseContextMenu(this, animated);

        public virtual async void ForceOpen(bool animated = true)
        {
            SetContextViewIfNeeded();
            var context = ContextView;
            if (context == null)
            {
                return;
            }
            var width = Math.Max(context.Width, context.WidthRequest);
            var widthCompletionSource = new TaskCompletionSource<bool>();
            if (width <= 0)
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
            ForceOpenContextMenu(this, animated);
        }

        protected void SetViewContent(View content)
        => ContentView = content;

        protected abstract void SetContextView(View context);

        protected override void OnBindingContextChanged()
        {
            IsContextChanged = true;
            ForceClose(false);
            base.OnBindingContextChanged();
        }

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

        #region LEGACY CODE

        private View _contentView;
        private View _contextView = new ContentView { WidthRequest = 1 };

        protected StackLayout ViewStack { get; }
        protected ScrollDirection CurrentDirection { get; private set; }
        protected ScrollState CurrentState { get; private set; }
        protected bool HasBeenAccelerated { get; private set; }
        protected double PrevScrollX { get; private set; }
        protected bool IsInteracted { get; private set; }
        protected bool IsOpenDirection => CurrentDirection == ScrollDirection.Open;
        protected bool IsDirectionAndStateSame => (int)CurrentDirection == (int)CurrentState;

        public View ContentView
        {
            get => _contentView;
            set
            {
                if (value != _contentView)
                {
                    if (_contentView != null)
                    {
                        ViewStack.Children.Remove(_contentView);
                    }
                    if (value != null)
                    {
                        ViewStack.Children.Insert(0, value);
                    }
                }
                _contentView = value;
            }
        }

        public View ContextView
        {
            get => _contextView;
            set
            {
                value = value ?? new ContentView { WidthRequest = 1 };
                if (value != _contextView)
                {
                    value.IsVisible = false;
                    ViewStack.Children.Remove(_contextView);
                    ViewStack.Children.Add(value);
                    value.IsVisible = true;
                }
                _contextView = value;
            }
        }

        public async void ForceCloseContextMenu(BaseContextMenuView view, bool animated)
        {
            if (view == null)
            {
                return;
            }

            try
            {
                if (view.ScrollX > 0)
                {
                    await view.ScrollToAsync(0, 0, animated);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async void ForceOpenContextMenu(BaseContextMenuView view, bool animated)
        {
            var width = view?.ContextView?.Width;
            if (width.GetValueOrDefault() <= 0)
            {
                return;
            }

            try
            {
                await view.ScrollToAsync(width.Value, 0, animated);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public virtual void OnTouchStarted()
        {
            IsInteracted = true;
            TouchStarted?.Invoke(this);
            SetContextViewIfNeeded();
            HasBeenAccelerated = false;
        }

        public virtual async void OnTouchEnded()
        {
            IsInteracted = false;
            TouchEnded?.Invoke(this);
            CheckActionBarState();
            if (Device.RuntimePlatform == Device.Android)
            {
                await Task.Delay(10);
            }

            if (HasBeenAccelerated)
            {
                return;
            }

            var width = ContextView.Width;
            var isOpen = IsOpenDirection
                        ? ScrollX > GetMovingWidth(width)
                        : ScrollX > width - GetMovingWidth(width);
            await ScrollToAsync(isOpen ? width : 0, 0, true);
        }

        public virtual async Task OnFlingStarted(bool needScroll = true, bool animated = true, bool inMainThread = false)
        {
            HasBeenAccelerated = true;

            if (needScroll)
            {
                var task = ScrollToAsync(IsOpenDirection ? ContextView.Width : 0, 0, animated);
                if (inMainThread)
                {
                    var completionSource = new TaskCompletionSource<bool>();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await task;
                        completionSource.SetResult(true);
                    });
                    await completionSource.Task;
                    return;
                }
                await task;
            }
        }

        public async Task MoveSideMenu(bool isOpen = false, bool animated = true)
        {
            CurrentDirection = isOpen
                ? ScrollDirection.Open
                : ScrollDirection.Close;

            await OnFlingStarted(!IsDirectionAndStateSame, animated, true);
        }

        protected async void OnMoveActionInvoked(bool isOpen = false)
        {
            await MoveSideMenu(isOpen, true);
        }

        protected virtual void OnScrolled(object sender, ScrolledEventArgs args)
        {
            CurrentDirection = Math.Abs(PrevScrollX - ScrollX) < double.Epsilon
                    ? CurrentDirection
                    : PrevScrollX > ScrollX
                        ? ScrollDirection.Close
                        : ScrollDirection.Open;
            PrevScrollX = ScrollX;

            CheckScrollState();
            CheckActionBarState();
        }

        protected double GetMovingWidth(double contextWidth)
        => contextWidth * AcceptWidthPercentage;

        protected void CheckScrollState()
        {
            if (Math.Abs(ScrollX) <= double.Epsilon)
            {
                CurrentState = ScrollState.Closed;
                return;
            }

            if (Math.Abs(ScrollX - ContextView.Width) <= double.Epsilon)
            {
                CurrentState = ScrollState.Opened;
                return;
            }

            CurrentState = ScrollState.Moving;
        }

        protected bool CheckIsOpen()
        {
            var width = ContextView.Width;
            return IsOpenDirection
                            ? ScrollX > GetMovingWidth(width)
                            : ScrollX > width - GetMovingWidth(width);
        }

        protected void CheckActionBarState()
        {
            if (IsInteracted)
            {
                return;
            }

            if (ScrollX >= ContextView.Width)
            {
                ContextMenuOpened?.Invoke(this);
                return;
            }
            if (ScrollX <= 0)
            {
                ContextMenuClosed?.Invoke(this);
            }
        }

        #endregion
    }
}
