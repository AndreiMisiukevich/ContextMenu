using Xamarin.Forms;

namespace ContextMenuSample
{
    public abstract class BaseContentPage : ContentPage
    {
        protected T GetParent<T>(Element element) where T : Element
        {
            if (element is T view)
            {
                return view;
            }
            return GetParent<T>(element.Parent);
        }
    }
}