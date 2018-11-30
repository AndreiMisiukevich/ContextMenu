using System;
using Xamarin.Forms;

namespace ContextMenuSample
{
    public static class BindingExtensions
    {
        public static TView With<TView>(this TView view, Action<TView> action) where TView : View
        {
            action?.Invoke(view);
            return view;
        }
    }
}
