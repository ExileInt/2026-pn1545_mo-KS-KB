using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using System;

namespace Tests
{
    [TestClass]
    public sealed class BallTest
    {
        [TestMethod]
        public void BallGetsCreatedTest()
        {
            Ball ball = new Ball(new Vector2(10, 10));
            Assert.IsNotNull(ball);
            Assert.AreEqual(new Vector2(10, 10), ball.Position);
        }

        [TestMethod]
        public void BallGetsUpdatedTest()
        {
            Ball ball = new Ball(new Vector2(10, 10));
            ball.Position = new Vector2(20, 20);
            Assert.AreEqual(new Vector2(20, 20), ball.Position);
        }

        [TestMethod]
        public void BallGetsCreatedWithInvalidPositionTest()
        {
            Assert.Throws<ArgumentException>(() => new Ball(new Vector2(-2, 5)));
            Assert.Throws<ArgumentException>(() => new Ball(new Vector2(600, 300)));
        }

        [TestMethod]
        public void Change_TriggersPropertyChangedTest()
        {
            Ball ball = new Ball(new Vector2(10, 10));
            bool eventFired = false;

            ball.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Position")
                {
                    eventFired = true;
                }
            };

            ball.Position = new Vector2(20, 20);

            Assert.IsTrue(eventFired);
        }
    }
}