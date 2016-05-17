
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using APPDroid.AdmMed.Adapters;
using APPDroid.Framework.Helpers;
using APPDroid.Framework.Services;
using System.Net.Http;
using SCSAPP.Services.Messages;
using Newtonsoft.Json;
using APPDroid.Framework.Context;
using SCSAPP.Framework.Context;
using Android.Graphics;
using Android.Text;
using Android.Views.InputMethods;

namespace APPDroid.AdmMed.Activities
{
	[Activity (Label = "@string/app_admed", Icon = "@drawable/ic_admed", NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]	
	[IntentFilter(new string[] { "barcodescanner.RECVR" }, Categories = new[] { Intent.CategoryDefault })]
	public class ActPacienteExmpan : Activity {

		#region Variables and Controls
		const string STR_COLOR_FORMAT = "<font color={0}>{1}</font><br/>";
		const string STR_ALE = "ALE";
		const string STR_REL = "REL";
		AdaMedicamento adapterMedicamentos;
		ImageButton scan;
		ExpandableListView lisView;
		ListView ListViewMed;
		Button alergias;
		EditText editCodMe;
		string BarCode;
		DateTime initialDate;
		DateTime finalDate;
		DateTime initialDateValidate;
		DateTime finalDateValidate;
		bool codeRead = false;
		Button btnClosePatient;
		Medicament responseInstanceValCode;
		string Code;
		ProgressDialog progressDialog;
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Bundle.</param>
		protected override void OnCreate (Bundle savedInstanceState){
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.app_list_med);

			lisView = FindViewById<ExpandableListView> (Resource.Id.myExpandableListview);
			lisView.SetAdapter (new AdaExpaPaciente(this, ActivitiesContext.Context.Patient));
			ListViewMed = FindViewById<ListView>(Resource.Id.listViewMedicamentos);
			editCodMe = FindViewById<EditText> (Resource.Id.editCodMe);
			editCodMe.RequestFocus ();

			progressDialog = new ProgressDialog (this);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			InputMethodManager imms = (InputMethodManager)GetSystemService (Context.InputMethodService);
			imms.HideSoftInputFromInputMethod (editCodMe.WindowToken, 0);
			// try hide the keyboard 
			Window.SetSoftInputMode (SoftInput.StateHidden);

			var programa = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var DC = programa.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));

			editCodMe.Click += delegate {
				if(DC.Value.Equals ("N")){
					imms.HideSoftInputFromWindow(editCodMe.WindowToken, 0);
				}
			};
				
			adapterMedicamentos = new AdaMedicamento (this);
			ListViewMed.Adapter = adapterMedicamentos;

			btnClosePatient = FindViewById<Button> (Resource.Id.btnClosePatient);
			btnClosePatient.Click += BtnClosePatient_Click;

			alergias = FindViewById<Button> (Resource.Id.btnAlergias);
			alergias.Click += Alergias_Click;

			scan = FindViewById<ImageButton> (Resource.Id.buscarMedicamento);
			scan.Click += Scan_Click;

			editCodMe.AfterTextChanged += (sender, e) => {
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
						} else {
							ValidateBarCode (Code, ActivitiesContext.Context.Patient.Medicaments, ActivitiesContext.Context.PatientFull);
						}
					}
				}
			};

		}
		#endregion

		#region Click close patient
		/// <summary>
		/// Buttons the close patient click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void BtnClosePatient_Click (object sender, EventArgs e)
		{
			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var VCP = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));

			if (VCP.Value.Equals ("L")) {
				var WindPatint = new Intent (this, typeof(ActPatientList));	
				StartActivity (WindPatint);	
				Finish ();
			} else if (VCP.Value.Equals ("V")) {
				var WindPatintH = new Intent(this, typeof(ActSelectPatient));	
				StartActivity(WindPatintH);	
				Finish ();	
			}else if(VCP.Value.Equals ("H")) {
				var WindPatintH = new Intent(this, typeof(FragActCamaCodigo));	
				StartActivity(WindPatintH);	
				Finish ();	
			}

		}
		#endregion

		#region Click to scan code
		/// <summary>
		/// Scans the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Scan_Click (object sender, EventArgs e){

			BarCode = editCodMe.Text;

			if (string.IsNullOrEmpty (BarCode)) {
				Toast toast = Toast.MakeText(this, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			} else if (BarCode.Contains ("'")) {
				Toast toast = Toast.MakeText(this, "Medicamento no existe", ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			} else {
				ValidateBarCode (BarCode, ActivitiesContext.Context.Patient.Medicaments, ActivitiesContext.Context.PatientFull);
			}

		}
		#endregion

		#region click allergies

		/// <summary>
		/// Alergiases the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void Alergias_Click (object sender, EventArgs e){
			//Implementacion new antecedentes 
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {

				var request = new
				{
					ActivitiesContext.Context.Patient.History,
					ActivitiesContext.Context.InitialDate,
					ActivitiesContext.Context.FinalDate,
					ActivitiesContext.Context.Patient.Gendertable,
					ActivitiesContext.Context.Patient.Birthdate
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ListBackground", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResponseAntecedent (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region validate barcode
		/// <summary>
		/// Validates the bar code.
		/// </summary>
		/// <param name="barCode">Bar code.</param>
		/// <param name="medicaments">Medicaments.</param>
		/// <param name = "medicamentsFull"></param>
		public async void ValidateBarCode (string barCode, List<Medicament> medicaments, List<Medicament> medicamentsFull)	{

			progressDialog.Show();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new
				{
					Code = barCode,
					Medicaments = medicaments,
					ListMedicamentsFull = medicamentsFull,
					Startdate = ActivitiesContext.Context.InitialDate,
					Finaldate = ActivitiesContext.Context.FinalDate
				};

				try{
					var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
					var result = await httpClient.PostAsync ("ValidateArticle", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResultValidate (result);
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

		#region validate response barcode
		/// <summary>
		/// Results the validate.
		/// </summary>
		/// <returns>The validate.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResultValidate (HttpResponseMessage result){
			try{				
				if (result.IsSuccessStatusCode) {
					initialDateValidate = DateTime.Now;

					finalDateValidate = DateTime.Now;

					string iniDateValidate1 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour1);
					int rango1Value1 = 0;

					string finDateValidate2 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour2);
					int rango1Value2 = 0;

					string finDateValidate3 = string.Format("{0}",ContextApp.Instance.NursingParametersS.Hour3);


					string responseJsonText = await result.Content.ReadAsStringAsync ();
					responseInstanceValCode = JsonConvert.DeserializeObject<Medicament> (responseJsonText);

					//if(ActivitiesContext.Context.Patient.OrderOutputIndicator.Equals("N"))
					//TODO: Julio: Cambiar debido a que no se reemplica el ambiente.
					if(ActivitiesContext.Context.Patient.OrderOutputIndicator.Equals("S"))
					{
						//True hora1 hora2 orden de salida confirmada
						if (!String.IsNullOrEmpty(iniDateValidate1)) {
							if (int.TryParse(iniDateValidate1, out rango1Value1)) {
								initialDateValidate = DateTime.Now.AddHours (rango1Value1 * -1);
							}
						}

						if (!String.IsNullOrEmpty(finDateValidate2)) {
							if (int.TryParse(finDateValidate2, out rango1Value2)) {
								finalDateValidate = DateTime.Now.AddHours (rango1Value2);
							}
						}

						if(Between(responseInstanceValCode.Dose.ScheduledDate,initialDateValidate,finalDateValidate))
							ValidateStateMedicament(responseInstanceValCode);
						else
						{
							Toast toast = Toast.MakeText(this, "La hora de administración no corresponde para este medicamento", ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();

							//Actulizar
							horaParameter ();
							LoadPatientHistory ();
						}
					}
					else
					{
						//False hora1 hora3 sin orden de salida 
						if (!String.IsNullOrEmpty(iniDateValidate1)) {
							if (int.TryParse(iniDateValidate1, out rango1Value1)) {
								initialDateValidate = DateTime.Now.AddHours (rango1Value1 * -1);
							}
						}

						if (!String.IsNullOrEmpty(finDateValidate3)) {
							if (int.TryParse(finDateValidate3, out rango1Value2)) {
								finalDateValidate = DateTime.Now.AddHours (rango1Value2);
							}
						}

						if(Between(responseInstanceValCode.Dose.ScheduledDate,initialDateValidate,finalDateValidate))
							ValidateStateMedicament(responseInstanceValCode);
						else
						{

							Toast toast = Toast.MakeText(this, "La hora de administración no corresponde para este medicamento", ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();
							//Actulizar
							horaParameter ();
							LoadPatientHistory ();

						}
					}

				}else{

					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

				}

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format (GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			} finally { 
				editCodMe.Text = String.Empty;
				editCodMe.RequestFocus ();
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}
		}
		#endregion

		#region validate patient
		/// <summary>
		/// Validates the state medicament.
		/// </summary>
		/// <returns><c>true</c>, if state medicament was validated, <c>false</c> otherwise.</returns>
		/// <param name="responseInstance">Response instance.</param>
		public async void ValidateStateMedicament (Medicament responseInstance)
		{
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
				var request = new
				{
					Medicament = responseInstance,
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode}
				};

				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ValidateMedicamentAdminister", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResulValidateMedicament (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region validate drug response
		/// <summary>
		/// Resuls the validate medicament.
		/// </summary>
		/// <returns>The validate medicament.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResulValidateMedicament (HttpResponseMessage result){
			try{
				if (result.IsSuccessStatusCode) 
				{
					Medicament MedFinded = null;

					var ProgChemedica = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.Equals ("cmoadmmed"));
					var varCA = ProgChemedica.Variables.FirstOrDefault (v => v.Code.ToUpper ().Equals ("CA"));

					if (varCA.Value.Equals("S")) {
						if(!responseInstanceValCode.IsPreDispatched){
							Toast toast = Toast.MakeText(this, "El medicamento requiere predespacho.", ToastLength.Long);
							toast.SetGravity(GravityFlags.Center, 0, 0);
							toast.Show();
							return;
						}
					}
						
					for (int i = 0; i < ActivitiesContext.Context.Patient.Medicaments.Count; i++) {

						if(ActivitiesContext.Context.PositionOnAdminister == -1 )
						{
							if(ActivitiesContext.Context.Patient.Medicaments[i].Code.Equals(responseInstanceValCode.Code))
							{
								MedFinded = ActivitiesContext.Context.Patient.Medicaments[i];
								MedFinded.BarCode = new List<string> { BarCode };
								MedFinded.ConcentrationFactor = responseInstanceValCode.ConcentrationFactor;
								ActivitiesContext.Context.PositionOnAdminister = i;
								break;
							}
							else
								ActivitiesContext.Context.PositionOnAdminister = i;
						}
						else if(ActivitiesContext.Context.Patient.Medicaments[i].Code.Equals(responseInstanceValCode.Code) && (ActivitiesContext.Context.AdministerData[ActivitiesContext.Context.PositionOnAdminister] != 2)){
							MedFinded = ActivitiesContext.Context.Patient.Medicaments[i];
							MedFinded.BarCode = new List<string> { BarCode };
							MedFinded.ConcentrationFactor = responseInstanceValCode.ConcentrationFactor;
							ActivitiesContext.Context.PositionOnAdminister = i;
							break;
						}
						else
						{
							ActivitiesContext.Context.PositionOnAdminister = i;
							if(ActivitiesContext.Context.PositionOnAdminister < ActivitiesContext.Context.Patient.Medicaments.Count)
								ActivitiesContext.Context.PositionOnAdminister++;

						}
					}

					if (MedFinded != null) {
						if(MedFinded.Dose.Quantity > responseInstanceValCode.ConcentrationFactor){
							//Pasa requerir dosis 
							ActivitiesContext.Context.medicamentDose = MedFinded;
							var windDose = new Intent(this, typeof(ActDosis));
							StartActivity(windDose);	

						}else if(MedFinded.Dose.Quantity <= responseInstanceValCode.ConcentrationFactor){
							//Pasa administrar el medicamento
							var alerDialog = (new AlertDialog.Builder(this)).Create();
							alerDialog.SetTitle ("Administrar");

							String nombre = MedFinded.Description;
							nombre+=System.Environment.NewLine;
							nombre+=string.Format("Dosis: {0} {1}", Math.Round(MedFinded.Dose.Quantity, 2), MedFinded.Unit);

							alerDialog.SetMessage (nombre);
							alerDialog.SetButton (Resources.GetString (APPDroid.Framework.Resource.String.btn_aceptar), delegate {

								ActivitiesContext.Context.IsScanning = true;
								ActivitiesContext.Context.AdministarMedicament = true;

								if(MedFinded.RequireDiluent){
									var windDiluent = new Intent(this, typeof(ActDiluyente));
									StartActivity(windDiluent);	
									return;
								}

								//Solicitar Firma
								var solicitaFirma = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault ().Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("SF"));

								ActivitiesContext.Context.Patient.Medicaments[ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = true;

								if(solicitaFirma.Value.Equals("S")){

									//Siempre pide firma
									var windFirma = new Intent(this, typeof(ActConfirmar));
									StartActivity(windFirma);
								}
								else if(solicitaFirma.Value.Equals("N") || solicitaFirma.Value.Equals(String.Empty)){

									//Solo si es por primera vez
									if(ActivitiesContext.Context.FirstPrimary){
										var windFirma2 = new Intent(this, typeof(ActConfirmar));
										StartActivity(windFirma2);
									}
									else
									{
										AdministerHandler(MedFinded);
									}
								}
								else if(solicitaFirma.Value.Equals("J")){
									//Nunca pide firma
									AdministerHandler(MedFinded);
								}
							});

							alerDialog.SetButton2 (Resources.GetString (APPDroid.Framework.Resource.String.btn_cancelar), delegate {
								alerDialog.Dismiss();
							});

							alerDialog.Show();
						}
					}
					else {
						ActivitiesContext.Context.PositionOnAdminister = 0;
						Toast toast = Toast.MakeText(this, GetString(APPDroid.Framework.Resource.String.txt_medicament), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}
				}
				else
				{
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();

					horaParameter ();
					LoadPatientHistory ();

				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, string.Format (GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
			finally{ 
				editCodMe.Text = String.Empty;
				editCodMe.RequestFocus ();
			}
		}
		#endregion

		#region Date range
		/// <summary>
		/// Between the specified input, date1 and date2.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <param name="date1">Date1.</param>
		/// <param name="date2">Date2.</param>
		public bool Between(DateTime input, DateTime date1, DateTime date2)
		{
			return (input >= date1 && input <= date2);
		}
		#endregion

		#region administer medication
		/// <summary>
		/// Administers the handler.
		/// </summary>
		/// <param name="medicament">Medicament.</param>
 		public async void AdministerHandler (Medicament medicament){

			progressDialog.Show();

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {

				var request = new
				{
					IsScanning = true,
					Medicament = medicament,
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode, Location = new {Code = ActivitiesContext.Context.Patient.Location.Code}},
					User = new {Code = ContextApp.Instance.User.Code, Password = ContextApp.Instance.User.Password, AdministrativeStructure = ContextApp.Instance.SelectedEAD.Code, MainSpeciality = new { Code = ContextApp.Instance.SelectedSpeciality.Code }},
					Diluent = new {}
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync (GetString (APPDroid.Framework.Resource.String.txt_administrar), new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await AdministerResponse (result);
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

		#region Managing drug response
		/// <summary>
		/// Administers the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task AdministerResponse (HttpResponseMessage result){
			try{
				if (result.IsSuccessStatusCode) {

					ActivitiesContext.Context.AdministerData[ActivitiesContext.Context.PositionOnAdminister] = (int)AdministerType.AdminBarCode;

					var intent = new Intent(this, typeof(ActPacienteExmpan));
					StartActivity(intent);

				}else{

					horaParameter ();
					LoadPatientHistory ();

					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format (GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
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

		#region response history
		/// <summary>
		/// Responses the antecedent.
		/// </summary>
		/// <returns>The antecedent.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResponseAntecedent (HttpResponseMessage result){

			try{
				
				if (result.IsSuccessStatusCode) {

					string responseJsonText = await result.Content.ReadAsStringAsync ();
					var responseInstance = JsonConvert.DeserializeObject<List<Backgroundtype>> (responseJsonText);

					if (responseInstance != null) {
						var colletprePred = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLORFONDOANTEM"));
						var colorletraantem = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLORLETRAANTEM"));
						var colorletraalerm = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLORLETRAALERM"));

						string colorFondo = colletprePred == null ? "#FFFFC0" : colletprePred.Value;
						string colorLetra1 = colorletraantem == null ? "#FF0000" : colorletraantem.Value;
						string colorLetra2 = colorletraalerm == null ? "#0000FF" : colorletraalerm.Value;

						StringBuilder Str = new StringBuilder();

						responseInstance.ForEach ( Element => {
							if(Element.Code.Equals(STR_ALE)){
								Str.AppendFormat(STR_COLOR_FORMAT, colorLetra2, Element.Description);
							}
							else if(Element.Code.Equals(STR_REL)){
								Str.AppendFormat(STR_COLOR_FORMAT, colorLetra1, Element.Description);
							}
						});

						var alerDialog = (new AlertDialog.Builder(this)).Create();
						var inflater = LayoutInflater.Inflate(Resource.Layout.app_alergias, null);

						TextView textView = inflater.FindViewById<TextView> (Resource.Id.txtAlergiaDescrip);
						textView.TextFormatted = Html.FromHtml (String.Format("{0}", Str.ToString()));
						//textView.Gravity();
						inflater.FindViewById<LinearLayout> (Resource.Id.layoutFondo).SetBackgroundColor (Color.ParseColor (colorFondo));
						alerDialog.SetTitle ("Antecedentes!");
						alerDialog.SetView(inflater);
						alerDialog.Show();
					}

				}else{
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format (GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

		#region create menu
		/// <Docs>The options menu in which you place your items.</Docs>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Raises the create options menu event.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public override bool OnCreateOptionsMenu (IMenu menu){
			MenuInflater.Inflate(Resource.Menu.menu_actulizar, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		#endregion

		#region select menu event
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
		/// <param name = "item"></param>
		public override bool OnOptionsItemSelected (IMenuItem item){

			if (item.ItemId == Resource.Id.menu_salir_AdmMed)
				AlertSalir ();

			if (item.ItemId == Resource.Id.menu_actualizar) {

				horaParameter ();

				LoadPatientHistory ();
			}
				
			return base.OnOptionsItemSelected (item);
		}
		#endregion

		#region load patient
		/// <summary>
		/// Loads the patient history.
		/// </summary>
		public async void LoadPatientHistory (){

			progressDialog.Show();

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

		#region patient Response
		/// <summary>
		/// Starts the patient.
		/// </summary>
		/// <returns>The patient.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatient (HttpResponseMessage response)	{
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
						//ActivitiesContext.Context.FirstPrimary = true;

						var ventana = new Intent (this, typeof(ActPacienteExmpan));
						StartActivity (ventana);
						
					}else{
						Toast toast = Toast.MakeText(this, responseInstance.ToString(), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}

				}else{
					
					ActivitiesContext.Context.Patient.Medicaments = new List<Medicament> ();
					adapterMedicamentos = new AdaMedicamento (this);
					ListViewMed.Adapter = adapterMedicamentos;

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
			finally{
				if (progressDialog.IsShowing) {
					progressDialog.Hide();
				}
			}

		}
		#endregion

		#region exit event
		/// <summary>
		/// Alerts the salir.
		/// </summary>
		public void AlertSalir(){
			var alerDialog = (new AlertDialog.Builder(this)).Create();
			alerDialog.SetTitle (APPDroid.Framework.Resource.String.txt_alertas);
			alerDialog.SetMessage (Resources.GetString (APPDroid.Framework.Resource.String.txt_cerrar_sesion));
			alerDialog.SetButton (Resources.GetString (APPDroid.Framework.Resource.String.btn_aceptar), delegate {
				DataBaseManager.DeleteContext(DataBaseManager.IDContextType.ContextApp);
				var inten = new Intent (this, ActivitiesContext.Context.LoginType);
				StartActivity (inten);
				Finish();
			});
			alerDialog.SetButton2 (Resources.GetString (APPDroid.Framework.Resource.String.btn_cancelar), delegate {
				alerDialog.Dismiss();
			});
			alerDialog.Show();
		}
		#endregion

		#region UpdateReformulado
		/// <summary>
		/// Updates the reformulado.
		/// </summary>
		/// <param name="position">Position.</param>
		public async void UpdateReformulado (int position){
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new
				{
					Medicament = ActivitiesContext.Context.Patient.Medicaments [position],
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode, Location = new {Code = ActivitiesContext.Context.Patient.Location.Code}}
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat});
				try{
					var result = await httpClient.PostAsync ("UpdateMedicament", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await UpdateResponse (result, position);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region UpdateResponse
		/// <summary>
		/// Updates the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="result">Result.</param>
		/// <param name = "position"></param>
		public async System.Threading.Tasks.Task UpdateResponse (HttpResponseMessage result, int position){
			try{
				if (result.IsSuccessStatusCode) {
					//LLamar colores.
					ActivitiesContext.Context.Patient.Medicaments[position].IsReformulated = false;
					var intent = new Intent(this, typeof(ActPacienteExmpan));
					StartActivity(intent);
					Finish();
				}else{
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(this, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}catch (Exception ex) {
				Toast toast = Toast.MakeText(this, String.Format (GetString(APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long);
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
			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmoadmmed"));
			var VCP = prog.Variables.FirstOrDefault (v => v.Code.Equals ("CP"));

			if (VCP.Value.Equals ("L")) {
				var WindPatint = new Intent (this, typeof(ActPatientList));	
				StartActivity (WindPatint);	
				Finish ();
			} else if (VCP.Value.Equals ("V")) {
				var WindPatintH = new Intent(this, typeof(ActPatientHandle));	
				StartActivity(WindPatintH);	
				Finish ();	
			}else if(VCP.Value.Equals ("H")) {
				var WindPatintH = new Intent(this, typeof(FragActCamaCodigo));	
				StartActivity(WindPatintH);	
				Finish ();	
			}
		}
		#endregion

		#region parameters hours
		/// <summary>
		/// Horas the parameter.
		/// </summary>
		public void horaParameter(){
			
			var tiedefatr = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("TIEDEFATR"));
			var tiedefade = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (f => f.Code.ToUpper ().Equals ("TIEDEFADE"));

			var iniDate = (tiedefatr == null || tiedefatr.Value == null) ? "1" : tiedefatr.Value;
			var finDate = (tiedefade == null || tiedefade.Value == null) ? "1" : tiedefade.Value;

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
		}
		#endregion

	}
}

