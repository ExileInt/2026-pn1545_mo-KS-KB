using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void BallGetsCreatedTest()
        {
            Ball ball = new Ball(5, new Vector2(5,5));
            Assert.IsNotNull(ball);
            Assert.AreEqual(5, ball.Radius);
            Assert.AreEqual(new Vector2(5, 5), ball.Position);
        }
    }
}