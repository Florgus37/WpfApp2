using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp2.ViewModels;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new ViewModels.MainViewModel();
            DataContext = vm;

            TasksFrame.Content = new Views.TasksView { DataContext = vm.TasksVM };
            NotesFrame.Content = new Views.NotesView { DataContext = vm.NotesVM };

            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            Services.DataManager.SaveData(vm.TasksVM.AllTasks.ToList(), vm.NotesVM.Notes.ToList());
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
        private void About_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Персональный органайзер студента");
    }
}