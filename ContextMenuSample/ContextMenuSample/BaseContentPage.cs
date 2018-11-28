using ContextMenu;
using Xamarin.Forms;

namespace ContextMenuSample
{
    public abstract class BaseContentPage : ContentPage
    {
        protected T GetParent<T>(Button button, Element parent) where T : BaseActionViewCell
        {
            if (!(parent is T actionViewCell))
            {
                actionViewCell = GetParent<T>(button, parent.Parent);
            }

            return actionViewCell;
        }
    }
}