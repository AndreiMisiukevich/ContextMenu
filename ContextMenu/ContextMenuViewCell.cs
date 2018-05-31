using System;
using Xamarin.Forms;

namespace ContextMenu
{
	public abstract class ContextMenuViewCell: ViewCell
	{
		private bool _isContextChanged;

		public ContextMenuViewCell()
		{
			View = new ContextMenuScrollView();
		}

		protected void SetContentView(View content)
		{
			(View as ContextMenuScrollView).ContentView = content;
		}

		protected void SetContextView(View context)
		{
			(View as ContextMenuScrollView).ContextView = context;
		}

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

		protected abstract View BuildContextView(object bindingContext);

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (View is ContextMenuScrollView sideContextBar)
			{
				sideContextBar.TouchStarted += OnTouchStarted;
			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			if (View is ContextMenuScrollView sideContextBar)
			{
				sideContextBar.TouchStarted -= OnTouchStarted;
			}
		}

		protected override void OnBindingContextChanged()
		{
			_isContextChanged = true;
			base.OnBindingContextChanged();
		}
	}
}
