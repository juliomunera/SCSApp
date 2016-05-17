
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using APPDroid.Framework.Services;
using SCSAPP.Services.Messages;
using Newtonsoft.Json;
using APPDroid.Framework.Context;
using System.Net.Http;
using SCSAPP.Framework.Context;
using System.Text;
using Android.Views;


namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", Icon = "@drawable/ic_admed", ParentActivity = typeof(ActPacienteExmpan), ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ActDiluyente : Activity{

		#region Variables and Controls
		List<MasterItem> DataDiluents = null; 
		Spinner SpiDiluyente;
		MasterItem DiluentSelected = null; 
		TextView NombreMedicamento;
		EditText EditCantidad;
		ProgressDialog progressDialog;
		bool _clickOnLoginBtn = false;
		#endregion

		#region #region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name = "savedInstanceState"></param>
		/// <param name="savedInstanceState">Bundle.</param>
		protected override void OnCreate (Bundle savedInstanceState){
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.app_diluyente);

			NombreMedicamento = FindViewById<TextView> (Resource.Id.NombreMedica);
			EditCantidad = FindViewById<EditText> (Resource.Id.EditCantidad);
			SpiDiluyente = FindViewById<Spinner> (Resource.Id.spinnerDiluyente);
			SpiDiluyente.ItemSelected += Event_diluents;

			NombreMedicamento.Text = ActivitiesContext.Context.Patient.Medicaments[ActivitiesContext.Context.PositionOnAdminister].Description;

			LoadDiluente ();

			Button cancelar = FindViewById<Button> (Resource.Id.btnCancelarDiluyente);
			cancelar.Click += Cancelar_Click;

			Button aceptar = FindViewById<Button> (Resource.Id.btnDiluyebte);
			aceptar.Click += Aceptar_Click; 

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

		}
		#endregion

		#region Click accept
		/// <summary>
		/// Aceptars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Aceptar_Click (object sender, EventArgs e)
		{

			if (_clickOnLoginBtn) 
				return;

			_clickOnLoginBtn = true;

			if (String.IsNullOrEmpty (EditCantidad.Text)) {
				// Julio Munera - David Ciro
				// Solucion Control #401368
				// 2016-05-09 : Se inicializa de nuevo la variable para que pueda ingresar de nuevo a las validaciones.
			    _clickOnLoginBtn = false;
				Toast toast = Toast.MakeText(this, string.Format ("{0}", GetString(APPDroid.Framework.Resource.String.txt_camposrequeridos)), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
				return;
			}

			if (!TextInputFormatValid(EditCantidad.Text))
				return;

			var solicitaFirma = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault ().Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("SF"));

			if (solicitaFirma.Value.Equals ("S")) {
				//Siempre pide firma
				ActivitiesContext.Context.DiluentSelecteds = DiluentSelected;
				ActivitiesContext.Context.DiluentSelecteds.Value = EditCantidad.Text;
				ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
				var windFirma = new Intent (this, typeof(ActConfirmar));
				StartActivity (windFirma);
				Finish ();
			} else if (solicitaFirma.Value.Equals ("N") || solicitaFirma.Value.Equals (String.Empty)) {
				//Solo si es por primera ves
				if (ActivitiesContext.Context.FirstPrimary) {
					ActivitiesContext.Context.DiluentSelecteds = DiluentSelected;
					ActivitiesContext.Context.DiluentSelecteds.Value = EditCantidad.Text;
					ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
					var windFirma = new Intent (this, typeof(ActConfirmar));
					StartActivity (windFirma);
					Finish ();
				} else {
					ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
					AdministrarManual ();	
				}
			} else if (solicitaFirma.Value.Equals ("J")) {
				//Nunca pide firma
				ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;
				AdministrarManual ();	
			}
		}
		#endregion

		#region Click cancel
		/// <summary>
		/// Determines whether this instance cancelar click the specified sender e.
		/// </summary>
		/// <returns><c>true</c> if this instance cancelar click the specified sender e; otherwise, <c>false</c>.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Cancelar_Click (object sender, EventArgs e)
		{
			var alerDialog = (new AlertDialog.Builder (this)).Create ();
			alerDialog.SetTitle (Resource.String.txt_alertas);
			alerDialog.SetMessage (GetString(APPDroid.Framework.Resource.String.txt_desea_cancelar));
			alerDialog.SetButton ("Si", delegate {
				var	ventanaPrincipal = new Intent(this, typeof(ActPacienteExmpan));
				StartActivity(ventanaPrincipal);
				Finish ();
			});
			alerDialog.SetButton2 ("No", (s, ev) =>  {
				var dialog = s as AlertDialog;
				if (dialog != null) {
					dialog.Dismiss ();	
				}
			});
			alerDialog.Show ();
		}
		#endregion

		#region Validate EditText.
		/// <summary>
		/// Texts the input format valid.
		/// </summary>
		/// <returns><c>true</c>, if input format valid was texted, <c>false</c> otherwise.</returns>
		public bool TextInputFormatValid (string Data)
		{
			if (Data.Contains (".") || Data.Contains (",")) {
				if (Data.Contains (".") && Data.Contains (",")) {
					Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_campo_separador), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					return false;
				}
				var BeforeChar = string.Empty;
				var AfterChar = String.Empty;

				var CharValue = Data.Contains (".") ? "." : ",";

				BeforeChar = Data.Substring (0, Data.IndexOf (CharValue));
				AfterChar = Data.Substring (Data.IndexOf (CharValue) + 1);

				if (BeforeChar.Length > 4) {
					Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_superior_4), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					return false;
				}

				if (AfterChar.Length > 2) {
					Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_superar_2), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					return false;
				}
			} else {
				if (Data.Length > 4) {
					Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_superior_4), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					return false;
				}
			}
			return true;
		}
			
		/// <summary>
		/// Events the diluents.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Event_diluents (object sender, AdapterView.ItemSelectedEventArgs e){
			var editor = sender as Spinner;
			if (editor != null) {
				DiluentSelected = DataDiluents.FirstOrDefault (d => d.Value.Equals (editor.SelectedItem.ToString ()));
			}

		}
		#endregion

		#region Service response method.
		/// <summary>
		/// Administrars the manual.
		/// </summary>
		public async void AdministrarManual (){

			progressDialog.Show();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var parameter = new {
					ActivitiesContext.Context.IsScanning,
					Medicament = ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister],
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode, Location = new {Code = ActivitiesContext.Context.Patient.Location.Code}},
					User = new { Code = ContextApp.Instance.User.Code, Password = ContextApp.Instance.User.Password, AdministrativeStructure = ContextApp.Instance.SelectedEAD.Code, MainSpeciality = new { Code = ContextApp.Instance.SelectedSpeciality.Code } },
					Diluent = new {Code = DiluentSelected.Code, Order = DiluentSelected.Order, Value = EditCantidad.Text }
				};

				var jsonRequest = JsonConvert.SerializeObject (parameter, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync (GetString (APPDroid.Framework.Resource.String.txt_administrar), new StringContent (jsonRequest, System.Text.Encoding.UTF8, GetString (APPDroid.Framework.Resource.String.txt_aplicacion_json)));
					await AdministrarResponse (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
					_clickOnLoginBtn = false;
				}
			}
		}

		/// <summary>
		/// Administrars the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task AdministrarResponse (HttpResponseMessage result){
			try{
				if (result.IsSuccessStatusCode) {

					ActivitiesContext.Context.AdministerData[ActivitiesContext.Context.PositionOnAdminister] = (int)AdministerType.AdminManual;

					var intent = new Intent(this, typeof(ActPacienteExmpan));
					StartActivity(intent);
					Finish();

				} else {
					
					LoadPatientHistoryDiluente();

					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format (GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
			finally
			{
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}

				_clickOnLoginBtn = false;
			}

		}
		#endregion

		#region Send parameters for diluents
		/// <summary>
		/// Loads the patient history.
		/// </summary>
		public async void LoadPatientHistoryDiluente () {

			progressDialog.Show ();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new {
					History = ActivitiesContext.Context.Patient.History,
					InitialDate = ActivitiesContext.Context.InitialDate,
					FinalDate = ActivitiesContext.Context.FinalDate
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

		#region Patients Recover
		/// <summary>
		/// Starts the patient.
		/// </summary>
		/// <returns>The patient.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatient (HttpResponseMessage response) {
			
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

						var ventana = new Intent (this, typeof(ActPacienteExmpan));
						StartActivity (ventana);

					}else{
						Toast toast = Toast.MakeText(this, responseInstance.ToString(), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}

				}else{
					
					ActivitiesContext.Context.Patient.Medicaments = new List<Medicament> ();
					var ventana = new Intent (this, typeof(ActPacienteExmpan));
					StartActivity (ventana);

				}	

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format(Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
			finally
			{
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}
		}
		#endregion

		#region for diluents
		/// <summary>
		/// Loads the diluente.
		/// </summary>
		public async void LoadDiluente (){
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)){
				var result = await httpClient.GetAsync ("Diluents");
				await StarDiluents (result);
			}
		}
		#endregion

		#region Diluents response
		/// <summary>
		/// Stars the diluents.
		/// </summary>
		/// <returns>The diluents.</returns>
		/// <param name = "response"></param>
		/// <param name="response">Result.</param>
		public async System.Threading.Tasks.Task StarDiluents (HttpResponseMessage response){
			try {
				if(response.IsSuccessStatusCode){
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					DataDiluents = JsonConvert.DeserializeObject<List<MasterItem>> (responseJsonText);
					if (DataDiluents != null)
						SetDataDiluents();

				}else{
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					Toast toast = Toast.MakeText(this, ExceptionMsg, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
				
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format(GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

		#region Show Diluents
		/// <summary>
		/// Sets the data diluents.
		/// </summary>
		public void SetDataDiluents ()	{
			if (SpiDiluyente != null) {
				
				var dataD = (from i in DataDiluents.AsEnumerable() select i.Value).ToArray();
				
				var DataServicio = new ArrayAdapter (this, Resource.Layout.item_spinner, dataD);
				DataServicio.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				SpiDiluyente.Adapter = DataServicio;
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
		public override void OnBackPressed (){
			var	ventanaPrincipal = new Intent(this, typeof(ActPacienteExmpan));
			StartActivity(ventanaPrincipal);
			Finish ();
		}
		#endregion

	}
}

