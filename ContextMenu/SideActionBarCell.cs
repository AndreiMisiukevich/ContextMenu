using Xamarin.Forms;
using System.Collections.Generic;

namespace ContextMenu
{
    public class SideActionBarCell : BaseActionViewCell
    {
        private static readonly Dictionary<Element, BaseActionViewCell> _cellBindings = new Dictionary<Element, BaseActionViewCell>();

        public SideActionBarCell()
        {
            View = Scroll;
            TouchStarted += OnTouchStarted;
        }

        protected virtual void OnTouchStarted(BaseActionViewCell sender)
        {
            if (_cellBindings.TryGetValue(Parent, out BaseActionViewCell cell))
            {
                if (cell == this)
                {
                    return;
                }
                cell.ForceClose();
            }
            _cellBindings[Parent] = this;
        }

        protected override void SetContextView(View context)
        => (View as ContextMenuScrollView).ContextView = context;

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_cellBindings.TryGetValue(Parent, out BaseActionViewCell cell) && cell == this)
            {
                ForceClose(false);
                _cellBindings.Remove(Parent);
            }
        }
    }
}
