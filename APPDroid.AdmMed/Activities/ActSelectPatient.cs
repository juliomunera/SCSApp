
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Context;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Services;
using Newtonsoft.Json;
using System.Net.Http;
using APPDroid.AdmMed.Adapters;
using Android.Views;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", Icon = "@drawable/ic_admed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]	
	public class ActSelectPatient : Activity
	{
		#region Variables and Controls
		ImageButton regresar1;
		ImageButton regresar2;
		ImageButton adelantar1;
		ImageButton adelantar2;
		Button actualizar;
		EditText rango1;
		EditText rango2;
		int? badInitial;
		int? badFinal;
		DateTime initialDate;
		DateTime finalDate;
		bool _refreshInprogress = false;
		DateTime timeSystem;
		ListView listPatienUne;
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.layout_list_patien_select_one);

			timeSystem = DateTime.Now;
			listPatienUne = FindViewById<ListView> (Resource.Id.listPatient);

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

			loadingPatient ();

			listPatienUne.ItemClick += (sender, args) => ListItemClicked (args.Position);

		}
		#endregion

		#region ListItemClicked
		void ListItemClicked (int position)
		{
			
			ActivitiesContext.Context.ParametersOne = new Dictionary<string, object> ();
			ActivitiesContext.Context.ParametersOne.Add ("position", position);
			ActivitiesContext.Context.ParametersOne.Add("rango1", rango1.Text);
			ActivitiesContext.Context.ParametersOne.Add("rango2", rango2.Text);

			var intent = new Intent (this, typeof(ActPatientHandle));
			StartActivity(intent);

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
		
			} catch (Exception ex) {

				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				_refreshInprogress = false;
			}
		}
		#endregion	

		#region loadingPatient
		/// <summary>
		/// Loadings the patient.
		/// </summary>
		void loadingPatient()
		{
			string Hour1 ;
			string Hour3;
			string iniDate;
			string finDate;

			var tiedefatr = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("TIEDEFATR"));
			var tiedefade = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (f => f.Code.ToUpper ().Equals ("TIEDEFADE"));

			Hour1 = tiedefatr.Value;

			if (ContextApp.Instance.NursingParametersS.Hour1 < Convert.ToInt32 (tiedefatr.Value)) 
			{
				Hour1 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour1);
			}

			iniDate = (tiedefatr == null || Hour1 == null) ? "1" : Hour1;
			Hour3 = tiedefade.Value;
			if (ContextApp.Instance.NursingParametersS.Hour3 < Convert.ToInt32 (tiedefade.Value)) 
			{
				Hour3 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour3);
			}

			finDate = (tiedefade == null || Hour3 == null) ? "1" : Hour3;

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

			badInitial = (int?)ActivitiesContext.Context.Parameters ["initialBed"];
			badFinal = (int?)ActivitiesContext.Context.Parameters ["finalBed"];

			LoadPacientes ((string)ActivitiesContext.Context.Parameters ["locatioin"], badInitial, badFinal, initialDate, finalDate);
		}
		#endregion

		#region loadingPatient
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

		#region StartPatienList
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

					if (responseInstance != null) 
					{
						ActivitiesContext.Context.PatientsLoadedInPatientList = responseInstance;
						listPatienUne.Adapter = new AdaPacientes(this);
					}
					else
					{
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
			var WindPatint = new Intent(this, typeof(ActMain));    
			StartActivity(WindPatint);
			Finish ();
		}
		#endregion

	}
}

