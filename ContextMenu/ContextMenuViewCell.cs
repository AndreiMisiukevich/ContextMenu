using System;
using Xamarin.Forms;

namespace ContextMenu
{
	public abstract class ContextMenuViewCell : ViewCell
	{
		#region props
		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(ContextMenuViewCell), null, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as ContextMenuViewCell).SetContentView(newValue as View);
		});

		public static readonly BindableProperty ContextTemplateProperty = BindableProperty.Create(nameof(ContextTemplate), typeof(DataTemplate), typeof(ContextMenuViewCell), null, propertyChanged: (bindable, oldValue, newValue) =>
		{
			(bindable as ContextMenuViewCell).IsContextChanged = true;
		});

		public DataTemplate ContextTemplate
		{
			get => GetValue(ContextTemplateProperty) as DataTemplate;
			set => SetValue(ContextTemplateProperty, value);
		}

		public View Content
		{
			get => GetValue(ContentProperty) as View;
			set => SetValue(ContentProperty, value);
		}
		#endregion

		protected ContextMenuScrollView Scroll { get; } = new ContextMenuScrollView();
		protected bool IsContextChanged { get; private set; }

		public ContextMenuViewCell()
		{
			View = Scroll;
		}

		protected void ForceClose(bool animated = true)
		=> Scroll.ForceCloseContextMenu(Scroll, animated);

		protected void SetIsOneCanBeOpened(bool flag)
		=> Scroll.IsOneMenuCanBeOpened = flag;

		protected void SetContentView(View content)
		=> (View as ContextMenuScrollView).ContentView = content;

		protected void SetContextView(View context)
		=> (View as ContextMenuScrollView).ContextView = context;

		protected void SetView(View content, View context)
		{
			SetContentView(content);
			SetContextView(context);
		}

		protected virtual void OnTouchStarted()
		{
			if (IsContextChanged)
			{
				IsContextChanged = false;

				var template = ContextTemplate is DataTemplateSelector selector 
					? selector.SelectTemplate(BindingContext, this) 
	              	: ContextTemplate;
				
				if(template == null)
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
