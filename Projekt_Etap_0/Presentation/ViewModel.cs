using System;
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

        private readonly ISimulation _simulation;

        private string _ballsCount = string.Empty;
        public ObservableCollection<IBall> Balls => _simulation.Balls;

        public string BallsCount
        {
            get => _ballsCount;
            set
            {
                _ballsCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BallsCount)));
            }
        }

        public ICommand StartCommand
        {
            get;
        }
        public ICommand StopCommand
        {
            get;
        }
        public ViewModel(ISimulation simulation)
        {
            _simulation = simulation;
            StartCommand = new CommandHandler(() =>
            {
                if (!String.IsNullOrEmpty(_ballsCount))
                {
                    if (int.TryParse(_ballsCount, out int count) && count != 0)
                    {
                        try
                        {
                            _simulation.StartWith(count);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

            });

            StopCommand = new CommandHandler(() =>
            {
                _simulation.Stop();
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
