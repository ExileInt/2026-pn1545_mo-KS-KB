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
        public ObservableCollection<IBall> Balls { get; set; } = new ObservableCollection<IBall>();
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
                    int count = Convert.ToInt32(_ballsCount);
                    if (count != 0)
                    {
                        _simulation.Balls.Clear();
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                _simulation.GenerateBall(count);

                            }
                            Balls.Clear();  
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        foreach (IBall ball in _simulation.Balls)
                        {
                            Balls.Add(ball);
                        }

                        _simulation.Start();
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
