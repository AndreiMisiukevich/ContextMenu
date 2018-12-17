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
            TouchEnded += OnTouchEnded;
        }

        public override void ForceOpen(bool animated = true)
        {
            SetThisAsLastOpenedCell();
            base.ForceOpen(animated);
        }

        protected virtual void OnTouchStarted(BaseActionViewCell sender)
        => SetThisAsLastOpenedCell();

        protected virtual void OnTouchEnded(BaseActionViewCell sender)
        => SetThisAsLastOpenedCell();

        protected override void SetContextView(View context)
        => (View as ContextMenuScrollView).ContextView = context;

        protected override void OnDisappearing()
        {
            if (LastOpenedCell == this)
            {
                SetLastOpenedCell(null);
            }
        }

        private void SetThisAsLastOpenedCell()
        {
            if (LastOpenedCell != this)
            {
                SetLastOpenedCell(this);
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
