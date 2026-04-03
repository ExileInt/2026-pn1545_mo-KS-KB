using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Data
{
    public interface IBallRepository
    {
        IDataBall CreateBall(Vector2 position);
        int DefaultDiameter {  get; }
    }
    public class BallRepository : IBallRepository
    {
        public int DefaultDiameter => 14;
        public IDataBall CreateBall(Vector2 position)
        {
            return new Ball(position);
        }
    }
}
