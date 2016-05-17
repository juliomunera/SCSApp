using System;
using Android.Views;
using Android.Support.V4.App;
using Android.App;
using Android.Support.V4.Widget;

namespace APPDroid.CarMed.Activities
{
	public class ActionBarDrawerEventArgs : EventArgs
	{
		public View DrawerView { get; set; }
		public float SlideOffset { get; set; }
		public int NewState { get; set; }
	}

	#region ActionBarDrawerChangedEventHandler
	/// <summary>
	/// Action bar drawer changed event handler.
	/// </summary>
	public delegate void ActionBarDrawerChangedEventHandler(object s, ActionBarDrawerEventArgs e);
	#endregion

	#region DrawerToggles
	/// <summary>
	/// Drawer toggles.
	/// </summary>
	public class DrawerToggles : ActionBarDrawerToggle
	{
		public DrawerToggles(Activity activity,
			DrawerLayout drawerLayout,
			int drawerImageRes,
			int openDrawerContentDescRes,
			int closeDrawerContentDescRes)
			: base(activity,
				drawerLayout,
				drawerImageRes,
				openDrawerContentDescRes,
				closeDrawerContentDescRes)
		{

		}

		public event ActionBarDrawerChangedEventHandler DrawerClosed;
		public event ActionBarDrawerChangedEventHandler DrawerOpened;
		public event ActionBarDrawerChangedEventHandler DrawerSlide;
		public event ActionBarDrawerChangedEventHandler DrawerStateChanged;

		public override void OnDrawerClosed(View drawerView)
		{
			if (null != DrawerClosed)
				this.DrawerClosed(this, new ActionBarDrawerEventArgs { DrawerView = drawerView });
			base.OnDrawerClosed(drawerView);
		}

		public override void OnDrawerOpened(View drawerView)
		{
			if (null != DrawerOpened)
				DrawerOpened(this, new ActionBarDrawerEventArgs { DrawerView = drawerView });
			base.OnDrawerOpened(drawerView);
		}

		public override void OnDrawerSlide(View drawerView, float slideOffset)
		{
			if (null != DrawerSlide)
				DrawerSlide(this, new ActionBarDrawerEventArgs
					{
						DrawerView = drawerView,
						SlideOffset = slideOffset
					});
			base.OnDrawerSlide(drawerView, slideOffset);
		}

		public override void OnDrawerStateChanged(int newState)
		{
			if (null != DrawerStateChanged)
				this.DrawerStateChanged(this, new ActionBarDrawerEventArgs
					{
						NewState = newState
					});
			base.OnDrawerStateChanged(newState);
		}

	}
	#endregion

}

