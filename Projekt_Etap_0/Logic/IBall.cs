using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface IBall : INotifyPropertyChanged
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        double Diameter { get; }

        void StartMoving();
        void StopMoving();

        int Id { get; set; }
        string Color { get; }
    }
}
