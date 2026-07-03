using task04;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }
        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            ISpaceship fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(40, fighter.FirePower);
        }
        [Fact]
        public void Cruiser_ShouldBeStrongerThanFighter()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.FirePower < cruiser.FirePower);
        }

        [Fact]
        public void CruiserFireAndMove()
        {
            ISpaceship cruiser = new Cruiser();
            var exception = Record.Exception(() =>
            {
                cruiser.Fire();
                cruiser.MoveForward();
            });
            Assert.Null(exception);
        }
        [Fact]
        public void FighterRotate()
        {
            ISpaceship fighter = new Fighter();
            int angle = 90;
            using var sw = new StringWriter();
            Console.SetOut(sw);

            fighter.Rotate(angle);
            string expectedOutput = $"Истребитель повернулся на 90 градусов" + Environment.NewLine;
            Assert.Equal(expectedOutput, sw.ToString());
        }


    }
}
