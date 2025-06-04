using BejeweledLivePlus;
using System;
 
namespace BejeweledLivePlus.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameMain())
                game.Run();
        }
    }
}