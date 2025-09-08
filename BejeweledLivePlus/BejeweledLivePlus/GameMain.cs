using System;
using System.Globalization;
using BejeweledLivePlus.Localization;
// using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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

		private Texture2D mSplash;

		private Texture2D mSplashCopyRight;

		private SpriteFont mSpriteFont;

		private bool mIsLoading = true;

		private double mElipseTime;

		private bool mInitBegin;

		// public PhoneApplicationService mAppService;

		// private GamerServicesComponent mGamerService;

		private DateTime preTime;

		private int mGameOffsetX;

		private int mGameOffsetY;

		private float mGameScaleRatio = 1.333f;

		private SexyAppBase.Touch mTouch = new SexyAppBase.Touch();

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
			base.IsFixedTimeStep = true;
			base.IsMouseVisible = true;
			theApp = new BejeweledLivePlusApp(this);
			SexyFramework.GlobalMembers.gSexyApp = theApp;
			SexyFramework.GlobalMembers.gSexyAppBase = theApp;
			GlobalMembers.gApp = theApp;
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
			mGameScaleRatio = GraphicsDevice.Viewport.Width / 480;
			Strings.Culture = CultureInfo.CurrentCulture;
			mSpriteBatch = new SpriteBatch(base.GraphicsDevice);
			mSplash = base.Content.Load<Texture2D>("Default-Landscape");
			mSplashCopyRight = base.Content.Load<Texture2D>("copyright/" + Strings.Legal);
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
			if (!mIsLoading)
			{
				theApp.Update(gameTime.ElapsedGameTime.Seconds);
				return;
				// try
				// {
				// 	theApp.Update(gameTime.ElapsedGameTime.Seconds);
				// 	return;
				// }
				// catch (GameUpdateRequiredException ex4)
				// {
				// 	theApp.HandleGameUpdateRequired(ex4);
				// 	return;
				// }
			}
			mElipseTime += gameTime.ElapsedGameTime.TotalSeconds;
			if (!mInitBegin)
			{
				GC.Collect();
				theApp.ReadFromRegistry();
				theApp.Init();
				theApp.Start();
				mInitBegin = true;
			}
			else if (mElipseTime >= 3.0)
			{
				mIsLoading = false;
				DateTime now = DateTime.Now;
				TimeSpan timeSpan = now - preTime;
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			if (mIsLoading)
			{
				mSpriteBatch.Begin();
				mSpriteBatch.Draw(mSplash, new Rectangle(0, 0, 480, 800), Color.White);
				mSpriteBatch.Draw(mSplashCopyRight, new Rectangle(0, 736, 480, 64), Color.White);
				mSpriteBatch.End();
			}
			else
			{
				if (mSplash != null)
				{
					mSplash.Dispose();
					mSplash = null;
					mSplashCopyRight.Dispose();
					mSplashCopyRight = null;
				}
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
			if (mIsLoading)
			{
				mElipseTime -= 2.0;
			}
			theApp.OnDeactivated();
			base.OnDeactivated(sender, args);
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			if (theApp.IsLoadingCompleted())
			{
				theApp.OnExiting();
				theApp.RegistrySave();
			}
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
					if (mIsLoading)
					{
						Exit();
					}
					else
					{
						theApp.OnHardwardBackButtonPressed();
					}
				}
				else
				{
					flag = false;
				}
			}
			theApp.GetTouchInputOffset(ref mGameOffsetX, ref mGameOffsetY);
			TouchCollection state = TouchPanel.GetState();
			
			// TODO: Add mouse hovering
			if (!TouchPanel.GetCapabilities().IsConnected)
			{
				MouseState mouseState = Mouse.GetState();
				TouchLocation location = new TouchLocation(1, 
					mouseState.LeftButton == ButtonState.Pressed ? TouchLocationState.Pressed : TouchLocationState.Released,
					mouseState.Position.ToVector2());
				state = new TouchCollection(new [] { location });
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
						float num = (mTouchX - (float)mGameOffsetX) * mGameScaleRatio;
						float num2 = (mTouchY - (float)mGameOffsetY) * mGameScaleRatio;
						mTouch.SetTouchInfo(new SexyFramework.Misc.Point((int)num, (int)num2), _TouchPhase.TOUCH_BEGAN, DateTime.Now.TimeOfDay.TotalMilliseconds);
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
			float num3 = (mTouchX - (float)mGameOffsetX) * mGameScaleRatio;
			float num4 = (mTouchY - (float)mGameOffsetY) * mGameScaleRatio;
			mTouch.SetTouchInfo(new SexyFramework.Misc.Point((int)num3, (int)num4), (!flag3) ? _TouchPhase.TOUCH_MOVED : _TouchPhase.TOUCH_ENDED, DateTime.Now.TimeOfDay.TotalMilliseconds);
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
