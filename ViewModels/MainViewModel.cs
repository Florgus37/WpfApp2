using System.ComponentModel;

namespace WpfApp2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public TasksViewModel TasksVM { get; set; }
        public NotesViewModel NotesVM { get; set; }

        public MainViewModel()
        {
            TasksVM = new TasksViewModel();
            NotesVM = new NotesViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}