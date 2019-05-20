using System.Windows.Input;
using Xamarin.Forms;

namespace ContextMenuSample.ViewModels
{
    public class MoveToDeleteViewModel : BaseViewModel
    {
        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new Command<Item>(item =>
        {
            Items.Remove(item);
        })); 
    }
}

