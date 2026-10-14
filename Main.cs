namespace Ball2D
{
    public static class Program
    {
        public static void Main()
        {
            using var game = new Ball2DSim();
            game.Run();
        }
    }
}