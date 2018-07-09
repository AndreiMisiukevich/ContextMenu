using Xamarin.Forms;

namespace ContextMenu
{
	public class ContextViewCell : ViewCell
	{
		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(ContextViewCell), null, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as ContextViewCell).SetContentView(newValue as View);
		});

		public static readonly BindableProperty ContextTemplateProperty = BindableProperty.Create(nameof(ContextTemplate), typeof(DataTemplate), typeof(ContextViewCell), null, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as ContextViewCell).IsContextChanged = true;
		});

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

		protected ContextMenuScrollView Scroll { get; } = new ContextMenuScrollView();

		public ContextViewCell()
		{
			View = Scroll;
		}

		public void ForceClose(bool animated = true)
		=> Scroll.ForceCloseContextMenu(Scroll, animated);

		protected void SetContentView(View content)
		=> (View as ContextMenuScrollView).ContentView = content;

		protected void SetContextView(View context)
		=> (View as ContextMenuScrollView).ContextView = context;

		protected virtual void OnTouchStarted()
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

		protected virtual void OnContextMenuOpened()
		{
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (View is ContextMenuScrollView sideContextBar)
			{
				sideContextBar.TouchStarted += OnTouchStarted;
				sideContextBar.ContextMenuOpened += OnContextMenuOpened;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			if (View is ContextMenuScrollView sideContextBar)
			{
				sideContextBar.TouchStarted -= OnTouchStarted;
				sideContextBar.ContextMenuOpened -= OnContextMenuOpened;
			}
		}

		protected override void OnBindingContextChanged()
		{
			IsContextChanged = true;
			ForceClose(false);
			base.OnBindingContextChanged();
		}
	}
}
