using Xamarin.Forms;
using System;

namespace ContextMenu
{
    public abstract class BaseActionViewCell : ViewCell
    {
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(SideActionBarCell), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseActionViewCell).SetContentView(newValue as View);
        });

        public static readonly BindableProperty ContextTemplateProperty = BindableProperty.Create(nameof(ContextTemplate), typeof(DataTemplate), typeof(SideActionBarCell), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            (bindable as BaseActionViewCell).IsContextChanged = true;
        });

        public static readonly BindableProperty IsAutoCloseEnabledProperty = BindableProperty.Create(nameof(IsAutoCloseEnabled), typeof(bool), typeof(MoveToActionCell), true);

        public event Action<BaseActionViewCell> ContextMenuOpened;

        public event Action<BaseActionViewCell> TouchStarted;

        private bool IsContextChanged { get; set; }

        public View Content
        {
            get => GetValue(ContentProperty) as View;
            set => SetValue(ContentProperty, value);
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

        protected ContextMenuScrollView Scroll { get; } = new ContextMenuScrollView();

        public void ForceClose(bool animated = true)
        => Scroll.ForceCloseContextMenu(Scroll, animated);

        protected void SetContentView(View content)
        => Scroll.ContentView = content;

        protected abstract void SetContextView(View context);

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Scroll.TouchStarted += OnTouchStarted;
            Scroll.ActionBarOpened += OnContextMenuOpened;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Scroll.TouchStarted -= OnTouchStarted;
            Scroll.ActionBarOpened -= OnContextMenuOpened;
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
    }
}
