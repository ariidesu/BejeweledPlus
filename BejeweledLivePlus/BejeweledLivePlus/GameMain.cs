using System;
using System.Globalization;
using System.Threading.Tasks;
using BejeweledLivePlus.Localization;
// using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Framework.Utilities;
using SexyFramework;
using SexyFramework.Drivers.App;
using SexyFramework.Misc;

namespace BejeweledLivePlus
{
	public class GameMain : Game
	{
		public class TestData
		{
			public string str = string.Empty;
		}

		private BejeweledLivePlusApp theApp;

		private SpriteBatch mSpriteBatch;

		private SpriteFont mSpriteFont;

		private bool mInitBegin;

		// public PhoneApplicationService mAppService;

		// private GamerServicesComponent mGamerService;

		private DateTime preTime;

		private SexyAppBase.Touch mTouch = new SexyAppBase.Touch();

		private SexyAppBase.MGKeyboard mKeyboard;

		private bool mIsTracking;

		private int mTouchID = -1;

		private float mTouchX;

		private float mTouchY;

		private double subTime;

		// public GamerServicesComponent GamerService => mGamerService;

		public GameMain()
		{
			base.Content = new WP7ContentManager(base.Services);
			base.Content.RootDirectory = "Content";
			base.IsFixedTimeStep = false;
            base.IsMouseVisible = true;
			theApp = new BejeweledLivePlusApp(this);
			SexyFramework.GlobalMembers.gSexyApp = theApp;
			SexyFramework.GlobalMembers.gSexyAppBase = theApp;
			GlobalMembers.gApp = theApp;
			if (PlatformInfo.MonoGamePlatform != MonoGamePlatform.Android &&
			    PlatformInfo.MonoGamePlatform != MonoGamePlatform.iOS)
			{
				Window.AllowUserResizing = true;
				Window.ClientSizeChanged += OnClientSizeChanged;
				mKeyboard = new SexyAppBase.MGKeyboard(Window, theApp);
			}
			// mGamerService = new GamerServicesComponent(this);
			// base.Components.Add(mGamerService);
			// Guide.SimulateTrialMode = false;
			// Guide.SimulateTrialMode = false;
			// mAppService = PhoneApplicationService.Current;
			// mAppService.Activated += OnServiceActivated;
			// mAppService.Deactivated += OnServiceDeactivated;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Strings.Culture = CultureInfo.CurrentCulture;
			mSpriteBatch = new SpriteBatch(base.GraphicsDevice);
			mSpriteFont = base.Content.Load<SpriteFont>("Arial_20");
			preTime = DateTime.Now;
		}

		protected override void LoadContent()
		{
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (theApp.WantExit)
			{
				Exit();
			}

			base.Update(gameTime);
			// try
			// {
			// 	 if (!Guide.IsVisible)
			// 	 {
			// 		base.Update(gameTime);
			// 	 }
			// }
			// catch (GameUpdateRequiredException ex)
			// {
			// 	theApp.HandleGameUpdateRequired(ex);
			// }
			UpdateInput(gameTime);
			// try
			// {
			// 	UpdateInput(gameTime);
			// }
			// catch (GameUpdateRequiredException ex2)
			// {
			// 	theApp.HandleGameUpdateRequired(ex2);
			// }
			// try
			// {
			// 	if (Guide.IsVisible)
			// 	{
			// 		return;
			// 	}
			// }
			// catch (Exception)
			// {
			// }
			if (!mInitBegin)
			{
				GC.Collect();
				theApp.ReadFromRegistry();
				theApp.Init();
				theApp.Start();
				mInitBegin = true;
			}
			theApp.Update(gameTime.ElapsedGameTime.Seconds);
		}

		protected override void Draw(GameTime gameTime)
		{
			if (mInitBegin)
			{
				theApp.Draw(0);
			}
			base.Draw(gameTime);
		}

		protected override void OnActivated(object sender, EventArgs args)
		{
			theApp.OnActivated();
			base.OnActivated(sender, args);
		}

		protected override void OnDeactivated(object sender, EventArgs args)
		{
			theApp.OnDeactivated();
			base.OnDeactivated(sender, args);
		}

		protected override void OnExiting(object sender, ExitingEventArgs args)
		{
			if (theApp.IsLoadingCompleted())
			{
				theApp.OnExiting();
				theApp.RegistrySave();
			}
		}

		private void OnClientSizeChanged(object sender, EventArgs args)
		{
			theApp.HandleWindowResize(Window.ClientBounds.Width, Window.ClientBounds.Height);
		}

		protected void OnServiceActivated(object sender, EventArgs args)
		{
			theApp.OnServiceActivated();
		}

		protected void OnServiceDeactivated(object sender, EventArgs args)
		{
			theApp.OnServiceDeactivated();
		}

		private void UpdateInput(GameTime gameTime)
		{
			bool flag = GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
			subTime += gameTime.ElapsedGameTime.TotalSeconds;
			if (flag)
			{
				if (subTime > 0.4000000059604645)
				{
					subTime = 0.0;
					theApp.OnHardwardBackButtonPressed();
				}
				else
				{
					flag = false;
				}
			}
			
			TouchCollection state = TouchPanel.GetState();

			if (!TouchPanel.GetCapabilities().IsConnected)
			{
				MouseState mouseState = Mouse.GetState();
				TouchLocation location = new TouchLocation(1,
					mouseState.LeftButton == ButtonState.Pressed ? TouchLocationState.Pressed : TouchLocationState.Released,
					mouseState.Position.ToVector2());
				state = new TouchCollection(new [] { location });

				int actualMouseX = (int)location.Position.X;
				int actualMouseY = (int)location.Position.Y;
				theApp.mGraphicsDriver.RemapMouse(ref actualMouseX, ref actualMouseY);
				// If we are in a game and the mouse is hovering inside the board region, we call MouseMove so Board.KeyDown can get the mouse position
				// This check is in because the interface is not designed for mouse hover
				if (theApp.mInterfaceState == InterfaceState.INTERFACE_STATE_INGAME && actualMouseY >= theApp.mBoard.GetBoardY() && actualMouseY <= theApp.mBoard.GetBoardY() + theApp.mWidth)
				{
					theApp.mWidgetManager.MouseMove(actualMouseX, actualMouseY);
				}
			}
			
			if (!mIsTracking)
			{
				foreach (TouchLocation item in state)
				{
					if (item.State == TouchLocationState.Pressed)
					{
						mIsTracking = true;
						mTouchID = item.Id;
						mTouchX = item.Position.X;
						mTouchY = item.Position.Y;
						int num = (int)mTouchX;
						int num2 = (int)mTouchY;
						theApp.mGraphicsDriver.RemapMouse(ref num, ref num2);
						mTouch.SetTouchInfo(new SexyFramework.Misc.Point(num, num2), _TouchPhase.TOUCH_BEGAN, DateTime.Now.TimeOfDay.TotalMilliseconds);
						theApp.TouchBegan(mTouch);
						break;
					}
				}
				return;
			}
			TouchLocation touchLocation = default(TouchLocation);
			bool flag2 = false;
			foreach (TouchLocation item2 in state)
			{
				if (item2.Id == mTouchID)
				{
					flag2 = true;
					touchLocation = item2;
				}
			}
			bool flag3 = true;
			if (flag2)
			{
				switch (touchLocation.State)
				{
				case TouchLocationState.Pressed:
				case TouchLocationState.Moved:
					flag3 = false;
					mTouchX = touchLocation.Position.X;
					mTouchY = touchLocation.Position.Y;
					break;
				case TouchLocationState.Released:
					mTouchX = touchLocation.Position.X;
					mTouchY = touchLocation.Position.Y;
					break;
				}
			}
			if (flag3)
			{
				mIsTracking = false;
			}
			int num3 = (int)mTouchX;
			int num4 = (int)mTouchY;
			theApp.mGraphicsDriver.RemapMouse(ref num3, ref num4);
			mTouch.SetTouchInfo(new SexyFramework.Misc.Point(num3, num4), (!flag3) ? _TouchPhase.TOUCH_MOVED : _TouchPhase.TOUCH_ENDED, DateTime.Now.TimeOfDay.TotalMilliseconds);
			if (flag3)
			{
				theApp.TouchEnded(mTouch);
			}
			else
			{
				theApp.TouchMoved(mTouch);
			}
		}
	}
}
