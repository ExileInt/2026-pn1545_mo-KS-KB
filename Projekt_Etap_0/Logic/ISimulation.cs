using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Logic
{

    public interface IBall : INotifyPropertyChanged
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        double Diameter { get; }
    }

    public interface ISimulation
    {
        ObservableCollection<IBall> Balls { get; }
        void Start();
        void Stop();
        void GenerateBall(int ballCount);

    }
    
}
