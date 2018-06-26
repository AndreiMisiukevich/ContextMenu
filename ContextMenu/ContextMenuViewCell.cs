using System;
using Xamarin.Forms;

namespace ContextMenu
{
	public abstract class ContextMenuViewCell: ViewCell
	{
		protected ContextMenuScrollView Scroll { get; } = new ContextMenuScrollView();
		private bool _isContextChanged;

		public ContextMenuViewCell()
		{
			View = Scroll;
		}

		protected void ForceClose() 
		=> Scroll.ForceCloseContextMenu(Scroll);

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
			if (_isContextChanged)
			{
				_isContextChanged = false;
				var contextView = BuildContextView(BindingContext);
				if (contextView == null)
				{
					return;
				}
				SetContextView(contextView);
			}
		}

		protected virtual void OnContextMenuOpened()
		{
		}

		protected abstract View BuildContextView(object bindingContext);

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
			_isContextChanged = true;
			base.OnBindingContextChanged();
		}
	}
}
