using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace BejeweledLivePlus.Android
{
    [Activity(
        Label = "BejeweledPlus",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
        Immersive = true,
        HardwareAccelerated = true
    )]
    public class Activity1 : AndroidGameActivity
    {
        private GameMain _game;
        private View _view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _game = new GameMain();
            _view = _game.Services.GetService(typeof(View)) as View;
            
            

            SetContentView(_view);
            _game.Run();
        }
    }
}