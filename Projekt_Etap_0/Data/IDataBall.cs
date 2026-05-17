using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public interface IDataBall : INotifyPropertyChanged
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        int Diameter { get; }
        void StartMoving();
        void StopMoving();

        int Id { get; set; }

    }
}
