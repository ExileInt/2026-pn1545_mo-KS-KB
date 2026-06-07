using Data;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

public class Ball : INotifyPropertyChanged, IDataBall
{
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private Vector2 _position;
    private Vector2 _velocity;
    private CancellationTokenSource _cts;
    private DateTime _lastColorChangeTime = DateTime.Now;

    private string _color = "Red";
    public string Color
    {
        get
        {
            _lock.EnterReadLock();
            try { return _color; }
            finally { _lock.ExitReadLock(); }
        }
        set
        {
            _lock.EnterWriteLock();
            try { _color = value; }
            finally { _lock.ExitWriteLock(); }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
        }
    }

    private int _id = 0;
    public int Id
    {
        get
        {
            _lock.EnterReadLock();
            try { return _id; }
            finally { _lock.ExitReadLock(); }
        }
        set
        {
            _lock.EnterWriteLock();
            try { _id = value; }
            finally { _lock.ExitWriteLock(); }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
        }
    }

    public const int Diameter = 14;
    private readonly int _diameter = Diameter;

    public event PropertyChangedEventHandler PropertyChanged;

    public Ball(Vector2 position)
    {
        if (position.X < 0 || position.Y < 0 || position.X > (560 - _diameter) || position.Y > (280 - _diameter))
            throw new ArgumentException("Position out of bounds");

        _position = position;
        _velocity = Vector2.Zero;
    }

    public Vector2 Position
    {
        get
        {
            _lock.EnterReadLock();
            try { return _position; }
            finally { _lock.ExitReadLock(); }
        }
        set
        {
            _lock.EnterWriteLock();
            try { _position = value; }
            finally { _lock.ExitWriteLock(); }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
        }
    }

    public Vector2 Velocity
    {
        get
        {
            _lock.EnterReadLock();
            try { return _velocity; }
            finally { _lock.ExitReadLock(); }
        }
        set
        {
            _lock.EnterWriteLock();
            try { _velocity = value; }
            finally { _lock.ExitWriteLock(); }
        }
    }

    int IDataBall.Diameter => _diameter;

    public void StartMoving()
    {
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        Task.Run(async () =>
        {
            string[] colors = new string[] { "Red", "Blue", "Yellow", "Purple" };

            while (!token.IsCancellationRequested)
            {
                moveIfLegal(this);

                if ((DateTime.Now - _lastColorChangeTime).TotalMilliseconds >= 5000)
                {
                    int colorIndex = Array.IndexOf(colors, Color);
                    if (colorIndex == -1) colorIndex = 0;

                    colorIndex = (colorIndex + 1) % colors.Length;
                    Color = colors[colorIndex];
                    _lastColorChangeTime = DateTime.Now;
                }

                await Task.Delay(16, token).ContinueWith(t => { });
            }
        }, token);
    }

    public void StopMoving() => _cts?.Cancel();

    public void moveIfLegal(Ball ball)
    {
        Vector2 tempPostition = Vector2.Add(ball.Position, ball.Velocity);

        if (tempPostition.X < 0 || tempPostition.X > 560 - _diameter)
        {
            ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
        }

        if (tempPostition.Y < 0 || tempPostition.Y > 280 - _diameter)
        {
            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
        }

        ball.Position = Vector2.Add(ball.Position, ball.Velocity);
    }

}
