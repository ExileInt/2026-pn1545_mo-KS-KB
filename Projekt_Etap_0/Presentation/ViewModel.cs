using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Logic;

namespace Presentation
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private readonly Simulation simulation = new Simulation();

        private void OnStateChanged(string parametr)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parametr));
        }

        public ICommand StartCommand
        {
            get;
        }
        public ViewModel()
        {
            StartCommand = new CommandHandler(() =>
            {
                simulation.Start();
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
