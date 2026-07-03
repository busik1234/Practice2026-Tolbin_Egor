namespace task04
{
    public interface ISpaceship
    {
        void MoveForward();      // Движение вперед
        void Rotate(int angle);  // Поворот на угол (градусы)
        void Fire();             // Выстрел ракетой
        int Speed { get; }       // Скорость корабля
        int FirePower { get; }   // Мощность выстрела
    }
    public class Cruiser : ISpaceship
    {
        public void MoveForward()
        {
            Console.WriteLine("Крейсер движется вперед");
        }
        public void Rotate(int angle)
        {
            Console.WriteLine("Крейсер повернулся на " + angle + " градусов");
        }
        public void Fire()
        {
            Console.WriteLine("Крейсер выстрелил ракетой");
        }
        public int Speed { get; } = 50;
        public int FirePower { get; } = 100;
    }
    public class Fighter : ISpaceship
    {
        public void MoveForward()
        {
            Console.WriteLine("Истребитель движется вперед");
        }
        public void Rotate(int angle)
        {
            Console.WriteLine("Истребитель повернулся на " + angle + " градусов");
        }
        public void Fire()
        {
            Console.WriteLine("Истребитель выстрелил ракетой");
        }
        public int Speed { get; } = 100;
        public int FirePower { get; } = 40;
    }
}
