using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Logic
{

    public interface IBall : INotifyPropertyChanged
    {
        double X { get; }
        double Y { get; }
        double Diameter { get; }
    }

    public interface ISimulation
    {
        ObservableCollection<IBall> Balls { get; }
        void Start();

    }
    
}
