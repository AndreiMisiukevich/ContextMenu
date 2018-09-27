using Xamarin.Forms;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ContextMenu
{
    public class SideActionBarCell : BaseActionViewCell
    {
        public static SideActionBarCell LastOpenedCell { get; private set; }

        public SideActionBarCell()
        {
            View = Scroll;
            TouchStarted += OnTouchStarted;
        }

        protected virtual void OnTouchStarted(BaseActionViewCell sender)
        {
            if (LastOpenedCell == this)
            {
                return;
            }
            LastOpenedCell?.ForceClose();
            LastOpenedCell = this;
        }

        protected override void SetContextView(View context)
        => (View as ContextMenuScrollView).ContextView = context;

        protected override void OnDisappearing()
        {
            if (this == LastOpenedCell)
            {
                LastOpenedCell?.ForceClose(false);
                LastOpenedCell = null;
            }
        }
    }
}
