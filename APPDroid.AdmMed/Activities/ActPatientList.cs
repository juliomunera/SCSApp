
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using APPDroid.AdmMed.Adapters;
using APPDroid.AdmMed.Fragments;

using Android.Support.V4.App;
using Android.Support.V4.Widget;
using APPDroid.Framework.Services;
using SCSAPP.Framework.Context;
using System.Net.Http;
using SCSAPP.Services.Messages;
using Newtonsoft.Json;
using APPDroid.Framework.Context;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_admed")]			
	public class ActPatientList : FragmentActivity{

		#region Variables and Controls
		MyActionBarDrawerToggle drawerToggle;
		string drawerTitle;
		string title;
		DrawerLayout m_Drawer;
		ListView _drawerList;
		DateTime timeSystem;
		ImageButton regresar1, regresar2, adelantar1, adelantar2;
		Button actualizar;
		EditText rango1, rango2;
		int? badInitial, badFinal;
		DateTime initialDate;
		DateTime finalDate;
		bool _refreshInprogress = false;
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Bundle.</param>
		protected override void OnCreate (Bundle savedInstanceState){
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.layout_adm_listaPacien);

			timeSystem = DateTime.Now;
			title = drawerTitle = Title;
			m_Drawer = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView> (Resource.Id.left_drawer);

			regresar1 = FindViewById<ImageButton> (Resource.Id.regresar1);
			regresar1.Click += Regresar1_Click;
			adelantar1 = FindViewById<ImageButton> (Resource.Id.avanzar1);
			adelantar1.Click += Adelantar1_Click;

			actualizar = FindViewById<Button> (Resource.Id.actualizar);
			actualizar.Click += Actualizar_Click;

			regresar2 = FindViewById<ImageButton> (Resource.Id.regresar2);
			regresar2.Click += Regresar2_Click;
			adelantar2 = FindViewById<ImageButton> (Resource.Id.avanzar2);
			adelantar2.Click += Adelantar2_Click;

			rango1 = FindViewById<EditText> (Resource.Id.ragoInicial);
			rango2 = FindViewById<EditText> (Resource.Id.rangoFinal);

			LoadingPatienValue ();

			//Set click handler when item is selected
			_drawerList.ItemClick += (sender, args) => ListItemClicked (args.Position);

			//Set Drawer Shadow
			m_Drawer.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			drawerToggle = new MyActionBarDrawerToggle (this, m_Drawer,
				Resource.Drawable.ic_drawer_light,
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

			//if first time you will want to go ahead and click first item.
			if (savedInstanceState == null) {
				ListItemClicked (0);
			}
				
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			//Abierto por defecto
			m_Drawer.OpenDrawer((int)GravityFlags.Left);

		}
		#endregion

		#region click return 1
		/// <summary>
		/// Regresar1s the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Regresar1_Click (object sender, EventArgs e){
			int valor12Regresar;
			valor12Regresar = Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour1) > 12 ? 12 : Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour1);
			rango1.Text = Convert.ToInt32 (rango1.Text) <= 1 ? string.Format("{0}",valor12Regresar) : Convert.ToString (Convert.ToInt32 (rango1.Text) - 1);
		}
		#endregion

		#region click return 2
		/// <summary>
		/// Regresar2s the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Regresar2_Click (object sender, EventArgs e){
			int valor12Regresar;
			valor12Regresar = Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour3) > 12 ? 12 : Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour3);
			rango2.Text = Convert.ToInt32 (rango2.Text) <= 1 ? string.Format("{0}",valor12Regresar) : Convert.ToString (Convert.ToInt32 (rango2.Text) - 1);
		}
		#endregion

		#region click to advance 1
		/// <summary>
		/// Adelantar1s the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Adelantar1_Click (object sender, EventArgs e){
			int valor12Adelantar;
			valor12Adelantar = Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour1) > 12 ? 12 : Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour1);
			rango1.Text = Convert.ToInt32 (rango1.Text) >= valor12Adelantar ? "1" : Convert.ToString (Convert.ToInt32 (rango1.Text) + 1);
		}
		#endregion

		#region click to advance 2
		/// <summary>
		/// Adelantar2s the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Adelantar2_Click (object sender, EventArgs e){
			int valor12Adelantar;
			valor12Adelantar = Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour3) > 12 ? 12 : Convert.ToInt32 (ContextApp.Instance.NursingParametersS.Hour3);
			rango2.Text = Convert.ToInt32 (rango2.Text) >= valor12Adelantar ? "1" : Convert.ToString (Convert.ToInt32 (rango2.Text) + 1);			
		}
		#endregion

		#region click update
		/// <summary>
		/// Actualizars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Actualizar_Click (object sender, EventArgs e){
			RefreshData ();
		}
		#endregion

		#region load patient
		/// <summary>
		/// Loads the pacientes.
		/// </summary>
		/// <param name="location">Location.</param>
		/// <param name="initialbed">Initialbed.</param>
		/// <param name="finalbed">Finalbed.</param>
		/// <param name="initialdate">Initialdate.</param>
		/// <param name="finaldate">Finaldate.</param>
		public async void LoadPacientes(string location, int? initialbed, int? finalbed, DateTime initialdate, DateTime finaldate)
		{	
			ActivitiesContext.Context.PatientsLoadedInPatientList = new List<Patient> ();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) 
			{
				var request = new
				{
					Location = location,
					InitialBed = initialbed,
					FinalBed = finalbed,
					FinalDate = finaldate,
					InitialDate = initialdate
				};

				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings  { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("PatientList", new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString(APPDroid.Framework.Resource.String.txt_aplicacion_json)));
					await StartPatienList (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}	
		}
		#endregion

		#region LoadingPatienValue
		/// <summary>
		/// Loadings the patien value.
		/// </summary>
		public void LoadingPatienValue(){

			string Hour1 ;
			string Hour3;
			string iniDate;
			string finDate;
			var tiedefatr = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("TIEDEFATR"));
			var tiedefade = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (f => f.Code.ToUpper ().Equals ("TIEDEFADE"));

			if (ContextApp.Instance.NursingParametersS.Hour1 != 0) 
			{
				Hour1 = tiedefatr.Value;

				if (ContextApp.Instance.NursingParametersS.Hour1 < Convert.ToInt32 (tiedefatr.Value)) 
				{
					Hour1 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour1);
				}

				iniDate = (tiedefatr == null || Hour1 == null) ? "1" : Hour1;
			} 
			else 
			{
				iniDate = (tiedefatr == null || tiedefatr.Value == null) ? "1" : tiedefatr.Value;
			}

			if (ContextApp.Instance.NursingParametersS.Hour3 != 0) 
			{
				Hour3 = tiedefade.Value;
				if (ContextApp.Instance.NursingParametersS.Hour3 < Convert.ToInt32 (tiedefade.Value)) 
				{
					Hour3 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour3);
				}

				finDate = (tiedefade == null || Hour3 == null) ? "1" : Hour3;
			} 
			else 
			{
				finDate = (tiedefade == null || tiedefade.Value == null) ? "1" : tiedefade.Value;
			}


			int rango1Value = 0;
			int rango2Value = 0;

			if (int.TryParse (iniDate, out rango1Value)) {
				if(rango1Value > 12){
					iniDate = "12";
				}
			} else {
				iniDate = "1";
			}

			if (int.TryParse (finDate, out rango2Value)) {
				if(rango2Value > 12){
					finDate = "12";
				}
			}
			else {
				finDate = "1";
			}

			rango1.Text = iniDate;
			rango2.Text = finDate;

			initialDate = DateTime.Now;
			finalDate = DateTime.Now;

			if (!String.IsNullOrEmpty(iniDate)) {
				if (int.TryParse(iniDate, out rango1Value)) {
					initialDate = DateTime.Now.AddHours (rango1Value * -1);
				}
			}

			if (!String.IsNullOrEmpty(finDate)) {
				if (int.TryParse(finDate, out rango2Value)) {
					finalDate = DateTime.Now.AddHours (rango2Value);
				}
			}

			badInitial = (int?)ActivitiesContext.Context.Parameters ["initialBed"] ?? null;
			badFinal = (int?)ActivitiesContext.Context.Parameters ["finalBed"] ?? null;

			LoadPacientes ((string)ActivitiesContext.Context.Parameters ["locatioin"], badInitial, badFinal, initialDate, finalDate);
		}
		#endregion

		#region patient Response
		/// <summary>
		/// Starts the patien list.
		/// </summary>
		/// <returns>The patien list.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatienList (HttpResponseMessage response)	{
			try {
				if (response.IsSuccessStatusCode) 
				{	
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					List<Patient> responseInstance = JsonConvert.DeserializeObject<List<Patient>> (responseJsonText);
					if (responseInstance != null) {
						ActivitiesContext.Context.PatientsLoadedInPatientList = responseInstance;
						_drawerList.Adapter = new AdaPacientes(this);
					}else{
						Toast toast = Toast.MakeText(this, APPDroid.Framework.Resource.String.txt_des_serializada, ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}
				}else{
					
					var	ventanaPrincipal = new Intent(this, typeof(ActMain));
					StartActivity(ventanaPrincipal);
					Finish ();

					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}

		}
		#endregion

		#region ListItemClicked
		/// <summary>
		/// Lists the item clicked.
		/// </summary>
		/// <param name="position">Position.</param>
		void ListItemClicked (int position){

			Android.Support.V4.App.Fragment argument = FragCodigoManilla.NewInstance (rango1.Text, rango2.Text);

			SupportFragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, argument).Commit ();

			_drawerList.SetItemChecked (position, true);
			//m_Drawer.CloseDrawers();
		}
		#endregion

		#region OnPrepareOptionsMenu
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
		public override bool OnPrepareOptionsMenu (IMenu menu){
			var drawerOpen = m_Drawer.IsDrawerOpen((int)GravityFlags.Left);
			//when open don't show anything
			for (int i = 0; i < menu.Size (); i++)
				menu.GetItem (i).SetVisible (!drawerOpen);

			return base.OnPrepareOptionsMenu (menu);
		}
		#endregion

		#region OnPostCreate
		/// <summary>
		/// Raises the post create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnPostCreate (Bundle savedInstanceState){
			base.OnPostCreate (savedInstanceState);
			drawerToggle.SyncState ();
		}
		#endregion

		#region OnConfigurationChanged
		/// <Docs>The new device configuration.</Docs>
		/// <remarks>Called by the system when the device configuration changes while your
		///  component is running. Note that, unlike activities, other components
		///  are never restarted when a configuration changes: they must always deal
		///  with the results of the change, such as by re-retrieving resources.</remarks>
		/// <summary>
		/// Raises the configuration changed event.
		/// </summary>
		/// <param name="newConfig">New config.</param>
		public override void OnConfigurationChanged (Configuration newConfig){
			base.OnConfigurationChanged (newConfig);
			drawerToggle.OnConfigurationChanged (newConfig);
		}
		#endregion

		#region OnOptionsItemSelected
		// Pass the event to ActionBarDrawerToggle, if it returns
		// true, then it has handled the app icon touch event
		public override bool OnOptionsItemSelected (IMenuItem item){
			if (drawerToggle.OnOptionsItemSelected (item))
				return true;

			return base.OnOptionsItemSelected (item);
		}
		#endregion

		#region RefreshData
		/// <summary>
		/// Refreshs the data.
		/// </summary>
		void RefreshData ()
		{
			try {
				if (_refreshInprogress)
					return;

				_refreshInprogress = true;

				timeSystem = DateTime.Now;
				initialDate = timeSystem.AddHours (-Convert.ToInt32 (rango1.Text));
				finalDate = timeSystem.AddHours (+Convert.ToInt32 (rango2.Text));
				LoadPacientes ((string)ActivitiesContext.Context.Parameters ["locatioin"], badInitial, badFinal, initialDate, finalDate);
				ListItemClicked (1);

				_refreshInprogress = false;	
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				_refreshInprogress = false;
 			}
		}
		#endregion

		#region OnBackPressed
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
		public override void OnBackPressed ()
		{
			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var VCP = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));

			if(VCP.Value.Equals("L")){
				var WindPatint = new Intent(this, typeof(ActMain));    
				StartActivity(WindPatint);
				Finish ();
			}else{
				var WindPatintH = new Intent(this, ActivitiesContext.Context.HomeType);    
				StartActivity(WindPatintH);    
				Finish ();    
			}

		}
		#endregion

	}
}

