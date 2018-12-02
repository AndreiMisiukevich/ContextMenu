using Xamarin.Forms;

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
            if (LastOpenedCell != this)
            {
                SetLastOpenedCell(this);
            }
        }

        protected override void SetContextView(View context)
        => (View as ContextMenuScrollView).ContextView = context;

        protected override void OnDisappearing()
        {
            if (LastOpenedCell == this)
            {
                SetLastOpenedCell(null);
            }
        }

        private void SetLastOpenedCell(SideActionBarCell cell)
        {
            if (IsAutoCloseEnabled)
            {
                LastOpenedCell?.ForceClose();
            }
            LastOpenedCell = cell;
        }
    }
}
