using System.Windows.Input;
using Xamarin.Forms;

namespace ContextMenuSample.ViewModels
{
    public class MoveToDeleteViewModel : BaseItemsViewModel
    {
        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new Command(item =>
        {
            Items.Remove(item as Item);
        })); 
    }
}

