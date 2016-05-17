
using System;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using APPDroid.Framework.Context;
using APPDroid.Framework.Services;
using System.Net.Http;
using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", Icon = "@drawable/ic_admed", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class ActPatientHandle : Activity
	{
		#region Variables and Controls
		int positionPatient;
		EditText EditCodigo;
		TextView namePatient;
		bool codeRead = false;
		string Code;
		DateTime initialDate;
		DateTime finalDate;
		ProgressDialog progressDialog;
		#endregion

		#region OnCreate Activity
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.fragment_codigo_b);

			//Implementación nombre paciente 
			positionPatient = (int)ActivitiesContext.Context.ParametersOne ["position"];
			var Patient = ActivitiesContext.Context.PatientsLoadedInPatientList [positionPatient];
			namePatient = FindViewById<TextView> (Resource.Id.txtNombrePatient);
			namePatient.Visibility = ViewStates.Visible;
			namePatient.Text = string.Format ("{0} {1} {2}", Patient.FirstName, Patient.MiddleName, Patient.LastName);

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			EditCodigo = FindViewById<EditText> (Resource.Id.editCodigo);
			EditCodigo.AfterTextChanged += (sender, e) => {
				if (codeRead)
					return;

				var Editor = sender as EditText;
				if (Editor != null && !String.IsNullOrEmpty (Editor.Text)) {
					if (Editor.Text.Trim ().Length > 1 && Editor.Text.Substring (Editor.Text.Length - 1, 1).Equals ("\n")) {
						codeRead = false;

						Code = Editor.Text;
						Code = Code.Replace ("\n", String.Empty);

						if (Code.Substring (Code.Length - 1, 1).Equals ("\n"))
							Code = Code.Substring (0, Code.Length - 1);

						if (string.IsNullOrEmpty (Editor.Text)) {
							Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();
							Editor.Focusable = true;
							Editor.FocusableInTouchMode = true;
						} else {
							ActionBuscar ();
						}
					}
				}
			};

			var iniDate = (string)ActivitiesContext.Context.ParametersOne ["rango1"];
			var finDate = (string)ActivitiesContext.Context.ParametersOne ["rango2"];

			int intIniDate = -1;
			int intFinDate = -1;

			initialDate = DateTime.Now;
			finalDate = DateTime.Now;

			if (!String.IsNullOrEmpty(iniDate)) {
				if (int.TryParse(iniDate, out intIniDate)) {
					initialDate = DateTime.Now.AddHours (intIniDate * -1);
				}
			}

			if (!String.IsNullOrEmpty(finDate)) {
				if (int.TryParse(finDate, out intFinDate)) {
					finalDate = DateTime.Now.AddHours (intFinDate);
				}
			}


			Button BtnCodigoPaciente = FindViewById<Button>(Resource.Id.btnBuscarPaciente);
			BtnCodigoPaciente.Click += (sender, e) => {
				Code = EditCodigo.Text;
				ActionBuscar ();
			};
		}
		#endregion

		#region event Search
		/// <summary>
		/// Actions the buscar.
		/// </summary>
		public void ActionBuscar ()
		{

			progressDialog.Show();

			if (String.IsNullOrEmpty (Code)) 
			{
				Toast toast = Toast.MakeText(this, Resources.GetString(APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				EditCodigo.Focusable = true;
				EditCodigo.FocusableInTouchMode = true;

				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			} 
			else  
			{
				var HistoryText = Code;
				long HistoryLong = -1;

				if (!long.TryParse(HistoryText, out HistoryLong)) 
				{
					Toast toast = Toast.MakeText(this, Resources.GetString(APPDroid.Framework.Resource.String.txt_numbre_historia), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					EditCodigo.Text = "";
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
					return;
				}
				else 
				{
					if (Convert.ToString(ActivitiesContext.Context.PatientsLoadedInPatientList [positionPatient].History) != HistoryText) {
						Toast toast = Toast.MakeText(this, Resources.GetString(APPDroid.Framework.Resource.String.txt_historia_inco), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						EditCodigo.Text = "";
						if (progressDialog.IsShowing) {
							progressDialog.Hide();
						}
						return;	
					}
				}

				LoadPatientHistory();
			}
		}
		#endregion

		#region patient load
		/// <summary>
		/// Loads the patient history.
		/// </summary>
		public async void LoadPatientHistory (){

			ActivitiesContext.Context.InitialDate = initialDate;
			ActivitiesContext.Context.FinalDate = finalDate;

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new { 
					History = Code,
					InitialDate = initialDate,
					FinalDate = finalDate
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("Patient", new StringContent (jsonRequest, Encoding.UTF8, GetString(APPDroid.Framework.Resource.String.txt_aplicacion_json)));
					await StartPatient (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}
			}
		}
		#endregion

		#region patient Response
		/// <summary>
		/// Starts the patien list.
		/// </summary>
		/// <returns>The patien list.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatient (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {

					string responseJsonText = await response.Content.ReadAsStringAsync ();
					Patient responseInstance = JsonConvert.DeserializeObject<Patient> (responseJsonText);

					if (responseInstance != null) {

						ActivitiesContext.Context.PatientFull = responseInstance.MedicamentslistFull;

						ActivitiesContext.Context.Patient = responseInstance;

						ActivitiesContext.Context.AdministerData = new int[ActivitiesContext.Context.Patient.Medicaments.Count];

						for (int i = 0; i < ActivitiesContext.Context.AdministerData.Length; i++)
							ActivitiesContext.Context.AdministerData [i] = (int)AdministerType.None;	

						ActivitiesContext.Context.PositionOnAdminister = -1;

						ActivitiesContext.Context.FirstPrimary = true;

						var ventana = new Intent (this, typeof(ActPacienteExmpan));
						StartActivity (ventana);
						Finish ();
						if (progressDialog.IsShowing) {
							progressDialog.Hide();
						}

					}else{
						Toast toast = Toast.MakeText(this, responseInstance.ToString(), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
						if (progressDialog.IsShowing) {
							progressDialog.Hide();
						}
					}
				}else{
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}	

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}

			}
			finally
			{
				EditCodigo.Text = "";
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
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
			var WindPatint = new Intent(this, typeof(ActSelectPatient));    
			StartActivity(WindPatint);
			Finish ();
		}
		#endregion

	}

}

