using System.Runtime.InteropServices;
using Foundation;
using ManagedBass;
using UIKit;

namespace BejeweledLivePlus.iOS
{
    [Register("AppDelegate")]
    internal class Program : UIApplicationDelegate
    {
        private static GameMain game;

        internal static void RunGame()
        {
            MapLibraryNames();
            game = new GameMain();
            game.Run();
        }

        private static void MapLibraryNames()
        {
            NativeLibrary.SetDllImportResolver(typeof(Bass).Assembly,
                (_, assembly, path) => NativeLibrary.Load("@rpath/bass.framework/bass", assembly, path));
        }

        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(Program));
        }

        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }
    }
}
