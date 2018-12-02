using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Widget;
using System.Reflection;
using XScrollView = Xamarin.Forms.ScrollView;
using Android.Content;
using Android.Runtime;
using ContextMenu.Droid;
using ContextMenu;

[assembly: ExportRenderer(typeof(ContextMenuScrollView), typeof(ContextMenuScrollViewRenderer))]
namespace ContextMenu.Droid
{
	[Preserve(AllMembers = true)]
	public class ContextMenuScrollViewRenderer : ScrollViewRenderer
	{
		private GestureDetector _detector;
		private bool _isAttachedNew;

		[Obsolete("For Forms <= 2.4")]
		public ContextMenuScrollViewRenderer()
		{
			CreateGestureDetector();
		}

		public ContextMenuScrollViewRenderer(Context context) : base(context)
		{
			CreateGestureDetector();
		}

		public override bool OnTouchEvent(MotionEvent ev)
		{
			try
			{
				_detector.OnTouchEvent(ev);
			}
			catch(ObjectDisposedException)
			{
				CreateGestureDetector();
			}
			return base.OnTouchEvent(ev);
		}

		public override bool OnGenericMotionEvent(MotionEvent e)
		{
			try
			{
				_detector.OnGenericMotionEvent(e);
			}
			catch (ObjectDisposedException)
			{
				CreateGestureDetector();
			}
			return base.OnGenericMotionEvent(e);
		}

		public override bool DispatchTouchEvent(MotionEvent e)
		{
			switch (e.ActionMasked)
			{
				case MotionEventActions.Down:
					(Element as ContextMenuScrollView)?.OnTouchStarted();
					break;
				case MotionEventActions.Up:
				case MotionEventActions.Cancel:
					(Element as ContextMenuScrollView)?.OnTouchEnded();
					break;
			}
			return base.DispatchTouchEvent(e);
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
				((IScrollViewController)e.OldElement).ScrollToRequested -= OnScrollToRequestedNew;
			}
			if (e.NewElement != null)
			{
				e.NewElement.PropertyChanged += OnElementPropertyChanged;
				Controller.ScrollToRequested += OnScrollToRequestedNew;
			}
		}

		protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ChildCount > 0)
			{
				var bar = GetChildAt(0);
				bar.HorizontalScrollBarEnabled = false;
				bar.VerticalScrollBarEnabled = false;
				bar.OverScrollMode = OverScrollMode.Never;
			}
		}

		private void OnFlinged()
		{
			(Element as ContextMenuScrollView)?.OnFlingStarted();
		}

		#region Copied from XF sources

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			_isAttachedNew = true;

			var attachedField = typeof(ScrollViewRenderer).GetField("_isAttached", BindingFlags.NonPublic | BindingFlags.Instance);
			attachedField.SetValue(this, false);
		}

		protected override void OnDetachedFromWindow()
		{
			base.OnDetachedFromWindow();
			_isAttachedNew = false;
		}

		private async void OnScrollToRequestedNew(object sender, ScrollToRequestedEventArgs e)
		{
			if (!_isAttachedNew)
			{
				return;
			}

			var cycle = 0;
			while (IsLayoutRequested)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(1));
				cycle++;

				if (cycle >= 10)
					break;
			}

			var x = (int)Context.ToPixels(e.ScrollX);
			var y = (int)Context.ToPixels(e.ScrollY);
			var scroll = Element as XScrollView;

			var hScrollField = typeof(ScrollViewRenderer).GetField("_hScrollView", BindingFlags.NonPublic | BindingFlags.Instance);
			var hScrolView = hScrollField.GetValue(this) as HorizontalScrollView;

			if (hScrolView == null) return;

			int currentX = scroll.Orientation == ScrollOrientation.Horizontal || scroll.Orientation == ScrollOrientation.Both ? hScrolView.ScrollX : ScrollX;
			int currentY = scroll.Orientation == ScrollOrientation.Vertical || scroll.Orientation == ScrollOrientation.Both ? ScrollY : hScrolView.ScrollY;
			if (e.Mode == ScrollToMode.Element)
			{
				var itemPosition = Controller.GetScrollPositionForElement(e.Element as VisualElement, e.Position);
				x = (int)Context.ToPixels(itemPosition.X);
				y = (int)Context.ToPixels(itemPosition.Y);
			}

			hScrolView.SmoothScrollingEnabled = true;
			SmoothScrollingEnabled = true;

			var animated = e.ShouldAnimate;
			switch ((Element as XScrollView).Orientation)
			{
				case ScrollOrientation.Horizontal:
					if (animated)
					{
						Device.BeginInvokeOnMainThread(() => hScrolView.SmoothScrollTo(x, y));
						break;
					}
					hScrolView.ScrollTo(x, y);
					break;

				case ScrollOrientation.Vertical:
					if (animated)
					{
						Device.BeginInvokeOnMainThread(() => SmoothScrollTo(x, y));
						break;
					}
					ScrollTo(x, y);
					break;

				default:

					if (animated)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							hScrolView.SmoothScrollTo(x, y);
							SmoothScrollTo(x, y);
						});
						break;
					}
					hScrolView.ScrollTo(x, y);
					ScrollTo(x, y);
					break;

			}
			Controller.SendScrollFinished();
		}

		private void CreateGestureDetector()
		{
			var listener = new ContextGestureListener();
			_detector = new GestureDetector(Context, listener);
			listener.Flinged += OnFlinged;
		}

		#endregion
	}

	internal class ContextGestureListener : GestureDetector.SimpleOnGestureListener
	{
		internal event Action Flinged;

		public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			Flinged?.Invoke();
			return base.OnFling(e1, e2, velocityX, velocityY);
		}
	}
}
