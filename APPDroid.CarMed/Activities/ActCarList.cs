
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using APPDroid.CarMed.Adapters;
using Android.Content.Res;
using Android.Support.V4.App;
using APPDroid.Framework.Context;
using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Services;
using System.Net.Http;
using Newtonsoft.Json;
using SCSAPP.Services.Messages.Entities;

namespace APPDroid.CarMed.Activities
{
	[Activity (Label = "@string/app_carmed", Icon = "@drawable/ic_carmed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]		
	public class ActCarList : FragmentActivity
	{
		#region Variables and Controls
		DrawerToggles drawerToggle;
		string drawerTitle;
		string title;
		DrawerLayout m_Drawer;
		ListView _drawerList;
		ImageButton actualizar;
		ProgressDialog progressDialog;
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.layout_car_patients);

			title = drawerTitle = Title;
			m_Drawer = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView> (Resource.Id.left_drawer);

			_drawerList.Adapter = new AdapCargarPatients(this);

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			actualizar = FindViewById<ImageButton> (Resource.Id.imageButton);
			actualizar.Click += Actualizar_Click;

			//Set click handler when item is selected
			_drawerList.ItemClick += (sender, e) => EventoClicFragment (e.Position);

			//Set Drawer Shadow
			m_Drawer.SetDrawerShadow (Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			drawerToggle = new DrawerToggles (this, m_Drawer,
				Resource.Drawable.drawer_shadow_dark,
				Resource.String.drawer_open,
				Resource.String.drawer_close);

			//Display the current fragments title and update the options menu
			drawerToggle.DrawerClosed += (o, args) => {
				ActionBar.Title = title;
				InvalidateOptionsMenu ();
			};

			//Display the drawer title and update the options menu
			drawerToggle.DrawerOpened += (o, args) => {
				ActionBar.Title = drawerTitle;
				InvalidateOptionsMenu ();
			};

			//Set the drawer lister to be the toggle.
			m_Drawer.SetDrawerListener (drawerToggle);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			//Abierto por defecto
			m_Drawer.OpenDrawer((int)GravityFlags.Left);
		}
		#endregion

		#region update event
		/// <summary>
		/// Actualizars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Actualizar_Click (object sender, EventArgs e)
		{
			var Cmocarmed = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (v => v.Code.Equals ("cmocarmed"));
			var VarPi = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("PI"));
			var VarPC = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("PC"));

			LoadePatient (VarPi, VarPC);	
		}
		#endregion

		#region Loade Patient
		/// <summary>
		/// Loades the patient.
		/// </summary>
		/// <param name="varPi">Variable pi.</param>
		/// <param name = "varPC"></param>
		public async void LoadePatient (MasterItem varPi, MasterItem varPC)
		{
			try {
				progressDialog.Show ();
				using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
					var request = new
					{
						Location = ActivitiesContext.Context.ubicacionCar,
						AdminEad = ActivitiesContext.Context.EadPatient,
						PatientStatus = varPi.Value,
						AdmissionCharges = varPC.Value,
						UserSession = ContextApp.Instance.SelectedEAD.UserGroup,
						NumberOrder = ActivitiesContext.Context.NumeroOrdenCar
					};
					var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
					try{
						var result = await httpClient.PostAsync ("PatientsRecover", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
						await StartPatients (result);
					}catch (Exception ex) {
						Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						if (progressDialog.IsShowing) {
							progressDialog.Hide();
						}
					}
				}	
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region Response Patient
		/// <summary>
		/// Starts the patients.
		/// </summary>
		/// <returns>The patients.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatients (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {

					string responseJsonText = await response.Content.ReadAsStringAsync ();
					var listPatients = JsonConvert.DeserializeObject<List<ListOfPatiens>> (responseJsonText);

					int cantidadR = listPatients.Count;

					for (int i = 0; i < cantidadR; i++) {
						var liscero = listPatients.FirstOrDefault (p => p.ordersQuantity == 0);
						listPatients.Remove (liscero);
					}

					listPatients.OrderBy (k => k.CodeRoom);

					if (listPatients.Count > 0) {
						ActivitiesContext.Context.listPatients = listPatients;
						_drawerList.Adapter = new AdapCargarPatients (this);
					}
				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html")) {
						Toast.MakeText (this, ExceptionMsg, ToastLength.Long).Show ();	
					} else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();	
					}
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

		#region Event Click Fragment
		/// <summary>
		/// Eventos the clic fragment.
		/// </summary>
		/// <param name="position">Position.</param>
        void EventoClicFragment(int position) 
        {
            ActivitiesContext.Context.PositionPatient = position;
            var obj = new FragCarMedicine();
            var fragmentTransation = FragmentManager.BeginTransaction();
			fragmentTransation.Replace(Resource.Id.content_frame, obj);
            fragmentTransation.Commit();
            _drawerList.SetItemChecked(position, true);

            m_Drawer.CloseDrawers();
        }
		#endregion

		#region OnPrepare Options Menu
		/// <Docs>The options menu as last shown or first initialized by
		///  onCreateOptionsMenu().</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Prepare the Screen's standard options menu to be displayed. This is
		///  called right before the menu is shown, every time it is shown. You can
		///  use this method to efficiently enable/disable items or otherwise
		///  dynamically modify the contents.</para>
		/// <summary>
		/// Raises the prepare options menu event.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			//MenuInflater.Inflate(Resource.Menu.main_menu, menu);
			var drawerOpen = m_Drawer.IsDrawerOpen((int)GravityFlags.Left);
			//when open don't show anything
			for (int i = 0; i < menu.Size (); i++)
				menu.GetItem (i).SetVisible (!drawerOpen);


			return base.OnPrepareOptionsMenu (menu);
		}
		#endregion

		#region OnPost Create
		/// <summary>
		/// Raises the post create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}
		#endregion

		#region OnConfiguration Changed
		/// <Docs>The new device configuration.</Docs>
		/// <remarks>Called by the system when the device configuration changes while your
		///  component is running. Note that, unlike activities, other components
		///  are never restarted when a configuration changes: they must always deal
		///  with the results of the change, such as by re-retrieving resources.</remarks>
		/// <summary>
		/// Raises the configuration changed event.
		/// </summary>
		/// <param name="newConfig">New config.</param>
		public override void OnConfigurationChanged (Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			drawerToggle.OnConfigurationChanged (newConfig);
		}
		#endregion

		#region OnOptions Item Selected
		// Pass the event to ActionBarDrawerToggle, if it returns
		// true, then it has handled the app icon touch event
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			return drawerToggle.OnOptionsItemSelected (item) || base.OnOptionsItemSelected (item);
		}
		#endregion

	}
}

