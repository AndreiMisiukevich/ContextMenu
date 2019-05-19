using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace ContextMenu
{
    internal class SwipeContextScroll : BaseContextMenuView
    {
        public Action<View> ContextAction { get; set; }

        protected override void SetContextView(View context)
            => ContextAction?.Invoke(context);
    }

    public class SwipeContextMenuView : AbsoluteLayout
    {
        #region bindable
        public static readonly BindableProperty ViewProperty = BindableProperty.Create(nameof(View), typeof(View), typeof(SwipeContextMenuView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as SwipeContextMenuView).ContextMenu.View = newValue as View;
        });

        public static readonly BindableProperty ContextTemplateProperty = BindableProperty.Create(nameof(ContextTemplate), typeof(DataTemplate), typeof(SwipeContextMenuView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as SwipeContextMenuView).ContextMenu.ContextTemplate = newValue as DataTemplate;
        });

        public static readonly BindableProperty AcceptWidthPercentageProperty = BindableProperty.Create(nameof(AcceptWidthPercentage), typeof(double), typeof(SwipeContextMenuView), 0.33, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as SwipeContextMenuView).ContextMenu.AcceptWidthPercentage = (double)newValue;
        });

        public static readonly BindableProperty IsAutoCloseEnabledProperty = BindableProperty.Create(nameof(IsAutoCloseEnabled), typeof(bool), typeof(SwipeContextMenuView), true, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as SwipeContextMenuView).ContextMenu.IsAutoCloseEnabled = (bool)newValue;
        });

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
        #endregion

        private View _prevContext;

        public static readonly BindableProperty MovedCommandProperty = BindableProperty.Create(nameof(MovedCommand), typeof(ICommand), typeof(SwipeContextMenuView), null);

        public event Action<object> Moved;

        public BaseContextMenuView ContextMenu { get; }

        public SwipeContextMenuView()
        {
            var scroll = new SwipeContextScroll
            {
                ContextAction = new Action<View>(context =>
                {
                    if (_prevContext != null)
                    {
                        Children.Remove(_prevContext);
                    }
                    _prevContext = context;
                    SetLayoutFlags(context, AbsoluteLayoutFlags.All);
                    SetLayoutBounds(context, new Rectangle(0, 0, 1, 1));
                    Children.Insert(0, context);

                    ContextMenu.ContextView.WidthRequest = Width;
                    ContextMenu.ContextView.InputTransparent = true;
                })
            };
            Children.Add(scroll, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            scroll.ContextMenuOpened += (obj) =>
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
            };
            ContextMenu = scroll;
        }

        public ICommand MovedCommand
        {
            get => GetValue(MovedCommandProperty) as ICommand;
            set => SetValue(MovedCommandProperty, value);
        }
    }
}
