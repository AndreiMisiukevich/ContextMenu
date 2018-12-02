using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace ContextMenu
{
    public class MoveToActionCell : BaseActionViewCell
    {
        private readonly AbsoluteLayout _mainView;
        private View _prevContext;

        public static readonly BindableProperty MovedCommandProperty = BindableProperty.Create(nameof(MovedCommand), typeof(ICommand), typeof(MoveToActionCell), null);

        public event Action<object> Moved;

        public MoveToActionCell()
        {
            _mainView = new AbsoluteLayout();
            _mainView.Children.Add(Scroll, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            View = _mainView;

            ContextMenuOpened += (obj) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (IsAutoCloseEnabled)
                    {
                        ForceClose();
                    }
                    Moved?.Invoke(BindingContext);
                    MovedCommand?.Execute(BindingContext);
                });
            };
        }

        public ICommand MovedCommand
        {
            get => GetValue(MovedCommandProperty) as ICommand;
            set => SetValue(MovedCommandProperty, value);
        }

        protected override void SetContextView(View context)
        {
            if (_prevContext != null)
            {
                _mainView.Children.Remove(_prevContext);
            }
            _prevContext = context;
            AbsoluteLayout.SetLayoutFlags(context, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(context, new Rectangle(0, 0, 1, 1));
            _mainView.Children.Insert(0, context);

            Scroll.ContextView.WidthRequest = View.Width;
            Scroll.ContextView.InputTransparent = true;
        }
    }
}
