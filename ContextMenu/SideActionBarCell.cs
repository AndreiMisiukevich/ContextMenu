using Xamarin.Forms;
using System;

namespace ContextMenu
{
	public class SideActionBarCell : BaseActionViewCell
	{
		public SideActionBarCell()
		{
			View = Scroll;
		}

		protected override void SetContextView(View context)
		=> (View as ContextMenuScrollView).ContextView = context;
	}
}
