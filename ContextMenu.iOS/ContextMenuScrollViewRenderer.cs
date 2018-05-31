using System;
using Xamarin.Forms.Platform.iOS;
using ContextMenu.iOS;
using Xamarin.Forms;
using Foundation;
using ContextMenu;

[assembly: ExportRenderer(typeof(ContextMenuScrollView), typeof(ContextMenuScrollViewRenderer))]
namespace ContextMenu.iOS
{
	[Preserve(AllMembers = true)]
	public class ContextMenuScrollViewRenderer : ScrollViewRenderer
	{
		public static void Initialize()
		{
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				ShowsHorizontalScrollIndicator = false;
				ShowsVerticalScrollIndicator = false;

				DraggingEnded -= OnDraggingEnded;
				DraggingStarted -= OnDraggingStarted;

				DraggingEnded += OnDraggingEnded;
				DraggingStarted += OnDraggingStarted;

				DecelerationStarted -= OnDecelerationStarted;
				DecelerationStarted += OnDecelerationStarted;

				DecelerationRate = DecelerationRateFast;

				Bounces = false;
			}
		}


		private void OnDecelerationStarted(object sender, EventArgs e)
		{
			(Element as ContextMenuScrollView)?.OnFlingStarted();
		}

		private void OnDraggingEnded(object sender, EventArgs e)
		{
			(Element as ContextMenuScrollView)?.OnTouchEnded();
		}

		private void OnDraggingStarted(object sender, EventArgs e)
		{
			(Element as ContextMenuScrollView)?.OnTouchStarted();
		}
	}
}
