
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using APPDroid.Framework.Helpers;
using SCSAPP.Android.Adapters;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Context;

namespace AppDroid.Main.Activities
{
	[Activity(Label = "@string/panel_control", Icon = "@drawable/ic_servinte", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class AppHomeList : Activity
	{
		#region Methods Principal
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle){
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.app_home_list);

			if (ContextApp.Instance.SelectedEAD != null) {
				FindViewById<GridView> (Resource.Id.gridApp).Adapter = new ImageAdapter (this);
			} else {
				DataBaseManager.DeleteContext(DataBaseManager.IDContextType.ContextApp);
				Intent inten = new Intent (this, typeof(MainActivity));
				StartActivity (inten);
				Finish();
			}

			StartService (new Intent (this, typeof(InconsistencyService)));

			ActivitiesContext.Context.MenuType = typeof(AppHomeList);

		}
		#endregion

		#region Methods Menus
		/// <Docs>The options menu in which you place your items.</Docs>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Raises the create options menu event.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnCreateOptionsMenu (IMenu menu){
			MenuInflater.Inflate(Resource.Menu.main_menu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		/// <Docs>The menu item that was selected.</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">This hook is called whenever an item in your options menu is selected.
		///  The default implementation simply returns false to have the normal
		///  processing happen (calling the item's Runnable or sending a message to
		///  its Handler as appropriate). You can use this method for any items
		///  for which you would like to do processing without those other
		///  facilities.</para>
		/// <summary>
		/// Raises the options item selected event.
		/// </summary>
		/// <param name="item">Item.</param>
		public override bool OnOptionsItemSelected (IMenuItem item){
			switch (item.ItemId){
			case Resource.Id.menu_salir:
				AlertSalir ();
				break;
			} 

			return base.OnOptionsItemSelected (item);
		}
		#endregion

		#region Methods Regresar Activity
		/// <Docs>Called when the activity has detected the user's press of the back
		///  key.</Docs>
		/// <para tool="javadoc-to-mdoc">Called when the activity has detected the user's press of the back
		///  key. The default implementation simply finishes the current activity,
		///  but you can override this to do whatever you want.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 5"></since>
		/// <summary>
		/// Raises the back pressed event.
		/// </summary>
		public override void OnBackPressed (){
			AlertSalir ();
		}
			
		/// <summary>
		/// Alerts the salir.
		/// </summary>
		public void AlertSalir(){
			var alerDialog = (new Android.App.AlertDialog.Builder(this)).Create();
			alerDialog.SetTitle (APPDroid.Framework.Resource.String.txt_alertas);
			alerDialog.SetMessage (Resources.GetString (APPDroid.Framework.Resource.String.txt_cerrar_sesion));
			alerDialog.SetButton (Resources.GetString (APPDroid.Framework.Resource.String.btn_aceptar), delegate {
				DataBaseManager.DeleteContext(DataBaseManager.IDContextType.ContextApp);
				Intent inten = new Intent (this, typeof(MainActivity));
				StartActivity (inten);
				Finish();
			});
			alerDialog.SetButton2 (Resources.GetString (APPDroid.Framework.Resource.String.btn_cancelar), delegate {
				alerDialog.Dismiss();
			});
			alerDialog.Show();
		}
		#endregion

	}
}

