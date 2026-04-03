using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Data
{
    public interface IDataBall : INotifyPropertyChanged
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        int Diameter { get; }
    }
}