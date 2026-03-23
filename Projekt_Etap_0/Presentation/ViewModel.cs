using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Logic;

namespace Presentation
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<IBall> Balls { get; set; } = new ObservableCollection<IBall>();

        private readonly ISimulation _simulation;

        public ICommand StartCommand
        {
            get;
        }
        public ViewModel(ISimulation simulation)
        {
            _simulation = simulation;

            StartCommand = new CommandHandler(() =>
            {
                _simulation.Start();
                Balls.Clear();
                foreach (IBall ball in _simulation.Balls)
                {
                    Balls.Add(ball);
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
