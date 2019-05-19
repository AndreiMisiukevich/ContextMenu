using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ContextMenu
{
    public class SideContextMenuView : BaseContextMenuView
    {
        public static SideContextMenuView LastOpenedView { get; private set; }

        private bool _hasRenderer;

        public SideContextMenuView()
        {
            TouchStarted += OnTouchStarted;
            TouchEnded += OnTouchEnded;
        }

        public override void ForceOpen(bool animated = true)
        {
            SetThisAsLastOpenedView();
            base.ForceOpen(animated);
        }

        protected virtual void OnTouchStarted(BaseContextMenuView sender)
            => SetThisAsLastOpenedView();

        protected virtual void OnTouchEnded(BaseContextMenuView sender)
            => SetThisAsLastOpenedView();

        protected override void SetContextView(View context)
            => ContextView = context;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName == "Renderer")
            {
                if(!_hasRenderer)
                {
                    _hasRenderer = true;
                    return;
                }
                _hasRenderer = false;
                if (LastOpenedView == this)
                {
                    SetLastOpenedView(null);
                }
            }
        }

        private void SetThisAsLastOpenedView()
        {
            if (LastOpenedView != this)
            {
                SetLastOpenedView(this);
            }
        }

        private void SetLastOpenedView(SideContextMenuView view)
        {
            if (IsAutoCloseEnabled)
            {
                LastOpenedView?.ForceClose();
            }
            LastOpenedView = view;
        }
    }
}
