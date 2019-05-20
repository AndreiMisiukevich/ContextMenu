using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace ContextMenu
{
    public class SwipeContextMenuView : BaseContextMenuView
    {
        internal Action<View> ContextAction { get; set; }

        protected override void SetContextView(View context)
            => ContextAction?.Invoke(context);
    }

    public class SwipeContextHolder : AbsoluteLayout
    {
        private View _prevContext;

        public static readonly BindableProperty MovedCommandProperty = BindableProperty.Create(nameof(MovedCommand), typeof(ICommand), typeof(SwipeContextHolder), null);

        public static readonly BindableProperty VisibleWidthPercentageProperty = BindableProperty.Create(nameof(VisibleWidthPercentage), typeof(double), typeof(SwipeContextHolder), 1.0);

        public event Action<object> Moved;

        public SwipeContextMenuView ContextMenu { get; private set; }

        private bool _hasRenderer;

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
            if(child is SwipeContextMenuView menuView)
            {
                ContextMenu = menuView;
                menuView.ContextAction = new Action<View>(context =>
                {
                    if (_prevContext != null)
                    {
                        Children.Remove(_prevContext);
                    }
                    _prevContext = context;
                    SetLayoutFlags(context, AbsoluteLayoutFlags.All);
                    SetLayoutBounds(context, new Rectangle(0, 0, 1, 1));
                    Children.Insert(0, context);

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
