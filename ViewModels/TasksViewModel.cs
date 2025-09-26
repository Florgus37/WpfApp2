using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp2.Models;

namespace WpfApp2.ViewModels
{
    public class TasksViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TaskItem> AllTasks { get; set; }
        public ObservableCollection<TaskItem> FilteredTasks { get; set; }

        private string _filter = "All";
        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged(nameof(Filter));
                ApplyFilter();
            }
        }

        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
                (DeleteTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ToggleTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } = "Средний";
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(1);

        public ICommand AddTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand ToggleTaskCommand { get; set; }

        public TasksViewModel()
        {
            var (tasks, _) = Services.DataManager.LoadData();
            AllTasks = new ObservableCollection<TaskItem>(tasks);
            FilteredTasks = new ObservableCollection<TaskItem>(AllTasks);

            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask, () => SelectedTask != null);
            ToggleTaskCommand = new RelayCommand(ToggleTask, () => SelectedTask != null);
        }

        private void AddTask()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Название задачи не может быть пустым.");
                return;
            }

            var task = new TaskItem
            {
                Title = Title,
                Description = Description,
                Priority = Priority,
                DueDate = DueDate
            };

            AllTasks.Add(task);
            ApplyFilter();
            ClearInputs();
        }

        private void DeleteTask()
        {
            if (MessageBox.Show("Удалить задачу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AllTasks.Remove(SelectedTask);
                ApplyFilter();
            }
        }

        private void ToggleTask()
        {
            SelectedTask.IsCompleted = !SelectedTask.IsCompleted;
        }

        private void ApplyFilter()
        {
            FilteredTasks.Clear();
            var filtered = AllTasks.AsEnumerable();

            switch (Filter)
            {
                case "Active":
                    filtered = filtered.Where(t => !t.IsCompleted);
                    break;
                case "Completed":
                    filtered = filtered.Where(t => t.IsCompleted);
                    break;
            }

            foreach (var t in filtered)
                FilteredTasks.Add(t);
        }

        private void ClearInputs()
        {
            Title = "";
            Description = "";
            Priority = "Средний";
            DueDate = DateTime.Now.AddDays(1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}