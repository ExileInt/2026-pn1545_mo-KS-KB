using System;
using System.Collections.ObjectModel;
using System.Numerics;
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
            var simulation = new Simulation(testRepository);

            Assert.Throws<Exception>(() => simulation.GenerateBall(20000000));
        }

    }
}