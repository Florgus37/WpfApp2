using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Models;

namespace WpfApp2.ViewModels
{
    public class NotesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NoteItem> Notes { get; set; }
        public ObservableCollection<NoteItem> DisplayedNotes { get; set; }
        public NotesViewModel()
        {
            var (_, notes) = Services.DataManager.LoadData();
            Notes = new ObservableCollection<NoteItem>(notes);
            DisplayedNotes = new ObservableCollection<NoteItem>(notes);

            SaveNoteCommand = new RelayCommand(SaveNote);
            NewNoteCommand = new RelayCommand(NewNote);
            DeleteNoteCommand = new RelayCommand(DeleteNote, () => SelectedNote != null);
        }

        private void SearchNotes()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText) ?
                Notes :
                new ObservableCollection<NoteItem>(Notes.Where(n =>
                    n.Title.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    n.Content.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0));

            DisplayedNotes.Clear();
            foreach (var note in filtered)
                DisplayedNotes.Add(note);
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                SearchNotes();
            }
        }
        private NoteItem _selectedNote;
        public NoteItem SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
                OnPropertyChanged(nameof(SelectedNote));
                if (value != null)
                {
                    Title = value.Title;
                    Content = value.Content;
                }
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public ICommand SaveNoteCommand { get; set; }
        public ICommand NewNoteCommand { get; set; }
        public ICommand DeleteNoteCommand { get; set; }
        public ICommand SearchCommand { get; set; }


        private void SaveNote()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Заголовок не может быть пустым.");
                return;
            }

            if (SelectedNote == null)
            {
                var newNote = new NoteItem { Title = Title, Content = Content };
                Notes.Add(newNote);
                SelectedNote = newNote;
            }
            else
            {
                SelectedNote.Title = Title;
                SelectedNote.Content = Content;
            }
        }

        private void NewNote()
        {
            SelectedNote = null;
            Title = "";
            Content = "";
        }

        private void DeleteNote()
        {
            if (MessageBox.Show("Удалить заметку?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Notes.Remove(SelectedNote);
                NewNote();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}