using System.Windows.Input;
using WPF_APP.Utilities;

namespace WPF_APP.ViewModel
{
    class NavigatorVM : ViewModelBase
    {
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged();}
        }
        public ICommand HomeCommand { get; set; }
        public ICommand TeachersCommand { get; set; }
        public ICommand SubjectsCommand { get; set; }
        public ICommand ScheduleCommand { get; set; }
        public ICommand HelpCommand { get; set; }

        private void Home(object obj) => CurrentView = new HomeVM();
        private void Teacher(object obj) => CurrentView = new TeacherVM();
        private void Subject(object obj) => CurrentView = new SubjectVM();
        private void Schedule(object obj) => CurrentView = new ScheduleVM();
        private void Help(object obj) => CurrentView = new HelpVM();

        public NavigatorVM()
        {
            HomeCommand = new RelayCommand(Home);
            TeachersCommand = new RelayCommand(Teacher);
            SubjectsCommand = new RelayCommand(Subject);
            ScheduleCommand = new RelayCommand(Schedule);
            HelpCommand = new RelayCommand(Help);
            CurrentView = new HomeVM();

        }

    }
}
