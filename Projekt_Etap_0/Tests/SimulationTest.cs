using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public sealed class SimulationTest
    {
        [TestMethod]
        public void GeneratingTooMuchBalls_ThrowsTest()
        {
            IBallRepository testRepository = new BallRepository();
            Simulation simulation = new Simulation(testRepository);

            Assert.Throws<Exception>(() => simulation.GenerateBall(20000000));
        }

        [TestMethod]
        public void CollidingBallsChangeVelocity()
        {
            IBallRepository testRepository = new BallRepository();
            Simulation simulation = new Simulation(testRepository);

            IDataBall dataBall1 = testRepository.CreateBall(new Vector2(100, 140));
            IDataBall dataBall2 = testRepository.CreateBall(new Vector2(112, 140));

            BallAdapter ball1 = new BallAdapter(dataBall1);
            BallAdapter ball2 = new BallAdapter(dataBall2);

            ball1.Velocity = new Vector2(1, 0);
            ball2.Velocity = new Vector2(-1, 0);

            simulation.Balls.Add(ball1);
            simulation.Balls.Add(ball2);

            Vector2 initialVelocity1 = ball1.Velocity;
            Vector2 initialVelocity2 = ball2.Velocity;

            simulation.Start();
            Thread.Sleep(500);
            simulation.Stop();

            Thread.Sleep(100);

            Vector2 finalVelocity1 = ball1.Velocity;
            Vector2 finalVelocity2 = ball2.Velocity;

            bool velocityChanged = (finalVelocity1 != initialVelocity1) || (finalVelocity2 != initialVelocity2);

            Assert.IsTrue(velocityChanged, "Velocity kul powinno się zmienić po zderzeniu");
        }

    }
}