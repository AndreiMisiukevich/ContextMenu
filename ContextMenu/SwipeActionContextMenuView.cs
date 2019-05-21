using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace ContextMenu.Views
{
    public class SwipeActionContextMenuView : BaseContextMenuView
    {
        internal Action<View> ContextAction { get; set; }

        protected override void SetContextView(View context)
            => ContextAction?.Invoke(context);
    }

    public class SwipeActionContextHolder : AbsoluteLayout
    {
        private View _prevContext;

        public static readonly BindableProperty MovedCommandProperty = BindableProperty.Create(nameof(MovedCommand), typeof(ICommand), typeof(SwipeActionContextHolder), null);

        public static readonly BindableProperty VisibleWidthPercentageProperty = BindableProperty.Create(nameof(VisibleWidthPercentage), typeof(double), typeof(SwipeActionContextHolder), 1.0);

        public event Action<object> Moved;

        public SwipeActionContextMenuView ContextMenu { get; private set; }

        private bool _hasRenderer;

        private ContentView _context;

        public SwipeActionContextHolder()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                _context = new ContentView
                {
                    IsClippedToBounds = true
                };

                SetLayoutFlags(_context, AbsoluteLayoutFlags.All);
                SetLayoutBounds(_context, new Rectangle(0, 0, 1, 1));
                Children.Insert(0, _context);
            }
        }

        public ICommand MovedCommand
        {
            get => GetValue(MovedCommandProperty) as ICommand;
            set => SetValue(MovedCommandProperty, value);
        }

        public double VisibleWidthPercentage
        {
            get => (double)GetValue(VisibleWidthPercentageProperty);
            set => SetValue(VisibleWidthPercentageProperty, value);
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            if(child is SwipeActionContextMenuView menuView)
            {
                ContextMenu = menuView;
                menuView.ContextAction = new Action<View>(context =>
                {
                    if (_prevContext != null)
                    {
                        Children.Remove(_prevContext);
                    }
                    _prevContext = context;

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        _context.Content = context;
                    }
                    else
                    {
                        SetLayoutFlags(context, AbsoluteLayoutFlags.All);
                        SetLayoutBounds(context, new Rectangle(0, 0, 1, 1));
                        Children.Insert(0, context);
                    }

                    ContextMenu.ContextView.WidthRequest = Math.Min(Width, Math.Abs(Width * VisibleWidthPercentage));
                    ContextMenu.ContextView.InputTransparent = true;
                });
                SetLayoutFlags(menuView, AbsoluteLayoutFlags.All);
                SetLayoutBounds(menuView, new Rectangle(0, 0, 1, 1));

                menuView.ContextMenuOpened += OnContextMenuOpened;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Renderer")
            {
                if (!_hasRenderer)
                {
                    _hasRenderer = true;
                    return;
                }
                _hasRenderer = false;
                if(ContextMenu != null)
                {
                    ContextMenu.ContextMenuOpened -= OnContextMenuOpened;
                    ContextMenu.ContextAction = null;
                }
            }
        }

        private void OnContextMenuOpened(BaseContextMenuView menuView)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ContextMenu.IsAutoCloseEnabled)
                {
                    ContextMenu.ForceClose();
                }
                Moved?.Invoke(BindingContext);
                MovedCommand?.Execute(BindingContext);
            });
        }
    }
}
