using System;
using Xamarin.Forms.Platform.iOS;
using ContextMenu.iOS;
using Xamarin.Forms;
using Foundation;
using ContextMenu;

[assembly: ExportRenderer(typeof(BaseContextMenuView), typeof(ContextMenuViewRenderer))]
namespace ContextMenu.iOS
{
	[Preserve(AllMembers = true)]
	public class ContextMenuViewRenderer : ScrollViewRenderer
	{
		public static void Preserve()
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
			(Element as BaseContextMenuView)?.OnFlingStarted();
		}

		private void OnDraggingEnded(object sender, EventArgs e)
		{
			(Element as BaseContextMenuView)?.OnTouchEnded();
		}

		private void OnDraggingStarted(object sender, EventArgs e)
		{
			(Element as BaseContextMenuView)?.OnTouchStarted();
		}
	}
}
