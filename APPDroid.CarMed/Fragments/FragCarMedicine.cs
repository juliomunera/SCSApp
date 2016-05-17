
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;
using APPDroid.Framework.Services;
using System.Net.Http;
using SCSAPP.Framework.Context;
using Newtonsoft.Json;
using SCSAPP.Services.Messages.Entities;
using APPDroid.CarMed.Adapters;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Helpers;
using Android.Views.InputMethods;

namespace APPDroid.CarMed
{
	public class FragCarMedicine : Fragment
	{
		#region Variables and Controls
		ExpandableListView lisViewExpandable;
		ListView listMedicine;
		Button alistar;
		EditText editCodMe;
		List<Medicament> medicinePatient;
		Button imageViewGuardar;
		bool codeRead = false;
		ProgressDialog progressDialog;
		string Code;
		MasterItem LB;
		#endregion

		#region OnCreateView Fragment
		/// <Docs>The LayoutInflater object that can be used to inflate
		///  any views in the fragment,</Docs>
		/// <param name="savedInstanceState">If non-null, this fragment is being re-constructed
		///  from a previous saved state as given here.</param>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Raises the create view event.
		/// </summary>
		/// <param name="inflater">Inflater.</param>
		/// <param name="container">Container.</param>
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.fragment_car_cargos, container, false);


			lisViewExpandable = view.FindViewById<ExpandableListView> (Resource.Id.ExpandableListvie);

            listMedicine = view.FindViewById<ListView>(Resource.Id.listViewMedicine);
			alistar = view.FindViewById<Button> (Resource.Id.buscarMedicamento);
			editCodMe = view.FindViewById<EditText> (Resource.Id.editCodMe);
			imageViewGuardar = view.FindViewById<Button> (Resource.Id.imageViewGuardar);

			lisViewExpandable.SetAdapter(new ExpandableCarPatien(Activity, ActivitiesContext.Context.PositionPatient));

			return view;
		}
		#endregion

		#region OnActivityCreated Fragment
		/// <Docs>If the fragment is being re-created from
		///  a previous saved state, this is the state.</Docs>
		/// <summary>
		/// Raises the activity created event.
		/// </summary>
		/// <param name = "savedInstanceState"></param>
		public override void OnActivityCreated(Bundle savedInstanceState){
			base.OnActivityCreated (savedInstanceState);

			var prog = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmocarmed"));
			LB = prog.Variables.FirstOrDefault (v => v.Code.Equals ("LB"));
			var DC = prog.Variables.FirstOrDefault (v => v.Code.Equals ("DC"));

			if (LB.Value == "S") {
				alistar.Enabled = false;
				Toast.MakeText (Activity, GetString(APPDroid.Framework.Resource.String.txt_Valida_codigo_dar), ToastLength.Long).Show();
			}
				
			progressDialog = new ProgressDialog (Activity);
			progressDialog.SetMessage ("Cargando...");
			progressDialog.SetCancelable (false);

			alistar.Click += Alistar_Click;
			imageViewGuardar.Click += ImageViewGuardar_Click;
            ServiceMedicine();

			InputMethodManager imms = (InputMethodManager) Activity.GetSystemService (Context.InputMethodService);
			imms.HideSoftInputFromInputMethod (editCodMe.WindowToken, 0);
			Activity.Window.SetSoftInputMode (SoftInput.StateHidden);

			editCodMe.Click += delegate {
				if(DC.Value.Equals ("N")){
					imms.HideSoftInputFromWindow(editCodMe.WindowToken, 0);
				}
			};

			editCodMe.AfterTextChanged += (sender, e) => {
				if (codeRead)
					return;	

				var Editor = sender as EditText;
				if (Editor != null && !String.IsNullOrEmpty (Editor.Text)) {
					if (Editor.Text.Trim ().Length > 1 && Editor.Text.Substring (Editor.Text.Length - 1, 1).Equals ("\n")) {
						codeRead = false;

						Code = editCodMe.Text;
						Code = Code.Replace ("\n", String.Empty);

						if (Code.Substring (Code.Length - 1, 1).Equals ("\n"))
							Code = Code.Substring (0, Code.Length - 1);

						if (string.IsNullOrEmpty (editCodMe.Text)) {
							Toast.MakeText (Activity, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long).Show ();
						} else {
							ValidateBarCode (Code, medicinePatient, "escaneado");
						}
					}
				}
			};

		}
		#endregion

		#region Click enroll event
		/// <summary>
		/// Alistars the click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Alistar_Click (object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty (editCodMe.Text)) {
				Toast.MakeText (Activity, GetString (APPDroid.Framework.Resource.String.txt_codigo_barra_requerido), ToastLength.Long).Show ();
			} else {
				//Metodo para llamar el servicio.
				ValidateBarCode (editCodMe.Text, medicinePatient, "Digitado");
			}
		}
		#endregion

		#region  ImageViewGuardar_Click
		/// <summary>
		/// Images the view guardar click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void ImageViewGuardar_Click (object sender, EventArgs e)
		{
			int indicador = 0;

			for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) {
				if (ActivitiesContext.Context.listmedicament [i].PendingAmount > ActivitiesContext.Context.listmedicament [i].AmountAlistada) {
					if (ActivitiesContext.Context.listmedicament [i].ResponseCauses == null) {
						//El medicamento no tiene una causa de no administración. 
						//Davi Ciro
						//Soluciòn al control 401717, cambiar el teto del mensaje, se quita el medicamento
						//Toast.MakeText (Activity, String.Format (GetString (APPDroid.Framework.Resource.String.txt_completar_medicamento), ActivitiesContext.Context.listmedicament [i].Code), ToastLength.Long).Show ();
						Toast.MakeText (Activity, GetString (APPDroid.Framework.Resource.String.txt_completar_medicamento), ToastLength.Long).Show ();
						indicador = 1;
						break;
					} 
				}
			}
			if (indicador == 0) {
				//Llamar servicio.
				GuardarCargo ();
			}
		}
		#endregion

		#region save charges
		/// <summary>
		/// Guardars the cargo.
		/// </summary>
		public async void GuardarCargo ()
		{
			progressDialog.Show ();

			var Cmocarmed = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (v => v.Code.Equals ("cmocarmed"));
			var VarIT = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("IT"));

			//var telephonyManager = (TelephonyManager)Activity.GetSystemService (Context.TelephonyService);
			//string deviceId = telephonyManager.DeviceId;
			string ContextImei = DataBaseManager.GetContexts (DataBaseManager.IDContextType.imei);

			var detailSave = new List<DetailSaveOrders> ();

			for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) {
				var det = new List<DetLotCumReg> ();

				if (ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM != null) {
					for (int e = 0; e < ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM.Count; e++) {
						det.Add (new DetLotCumReg {
							NumberLote = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].NumberLote,
							CodeMedic = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].CodeMedic,
							Invima = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].Invima,
							lotNumber = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].lotNumber,
							DocumentSourceEnter = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].DocumentSourceEnter,
							DocumentEnter = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].DocumentEnter,
							ServicesEnter = ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM [e].ServicesEnter
						});					
					}
				}

				detailSave.Add (new DetailSaveOrders {
					SourceOrder = ActivitiesContext.Context.listmedicament [i].OrderSource,
					DocumentOrder = ActivitiesContext.Context.listmedicament [i].OrderDocument,
					LineOrder = ActivitiesContext.Context.listmedicament [i].OrderLine,
					LineOrderCode = ActivitiesContext.Context.listmedicament [i].Code,
					TypeArticle = "C",
					QualityArticle = ActivitiesContext.Context.listmedicament [i].AmountAlistada,
					NotBecauseOffice = ActivitiesContext.Context.listmedicament [i].ResponseCauses ?? String.Empty,
					QuantityPlayed = ActivitiesContext.Context.listmedicament [i].PendingAmount,
					ListItemDetailCum = det
				});
									
			}
				
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
				var request = new
				{
					CodeEad = ContextApp.Instance.SelectedEAD.Code,
					UserCode = ContextApp.Instance.User.Code,
					CodeUserDataBase = ContextApp.Instance.User.Code,
					CodeProgram = "cmocarmed",
					ValueParameter = VarIT.Value,
					PeriodYear = ActivitiesContext.Context.year,
					PeriodMonth = ActivitiesContext.Context.month,
					SourceCar = ActivitiesContext.Context.PatientSelecte.Code,
					WarehouseCode = ActivitiesContext.Context.Almacen.Code,
					ActivitiesContext.Context.EadPatient,
					PatientEpi = ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].NumbreEpisode,
					TypeEpisode = "I",
					NumberHistory = ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].HistoryPatient,
					NombrePatient = string.Format("{0} {1} {2} {3}",ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].FirstPatient,
						ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].SecondPatient,
						ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].Surname,
						ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].SecondSurname),
					NumberIngress = ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].NumberEntryEntry,
					SourceIngress = "",
					DocumentIngress = "",
					DataSystem = DateTime.Now,
					CodeMobile = ContextImei,
					ListMedicament = detailSave
				};	
						
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("SaveOrdersXml", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResulGuardar (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(Activity, String.Format ("Falló de conexión: {0} ", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}
			}
		}
		#endregion

		#region Save result
		/// <summary>
		/// Resuls the guardar.
		/// </summary>
		/// <returns>The guardar.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResulGuardar (HttpResponseMessage result)
		{
			try {
				if (result.IsSuccessStatusCode) {

					string responseJsonText = await result.Content.ReadAsStringAsync ();
					bool respuestaGuardar = JsonConvert.DeserializeObject<bool> (responseJsonText);
					if (respuestaGuardar) {
						Toast.MakeText (Activity, "El proceso de guardado se realizó correctamente.", ToastLength.Long).Show ();
						var test = new List<Medicament> ();
						test.Clear ();

						listMedicine.Adapter = new AdapMedicines (Activity, test, listMedicine);

						editCodMe.Text = "";
					}

				} else {
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
					Toast.MakeText (Activity, responseInstance, ToastLength.Long).Show ();
				}
			} catch (Exception ex) {
				Toast.MakeText (Activity, String.Format (GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long).Show ();
			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
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
		/// <param name="tipo">Tipo.</param>
		public async void ValidateBarCode (string barCode, List<Medicament> medicaments, string tipo)
		{
			progressDialog.Show ();

			var progCn = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (p => p.Code.ToLower ().Equals ("cmocarmed"));
			var CN = progCn.Variables.FirstOrDefault (v => v.Code.Equals ("CN"));

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new
				{
					Code = barCode,
					Type = tipo,
					ConceptRequireBalance = ActivitiesContext.Context.PatientSelecte.UpdateBalance,
					ConceptUseBatch = ActivitiesContext.Context.PatientSelecte.UseBatch,
					ConceptUseCum = ActivitiesContext.Context.PatientSelecte.UseCumCode,
					ConceptRequireInvima = ActivitiesContext.Context.PatientSelecte.UseInvima,
					CN = CN.Value,
					Warehouse = ActivitiesContext.Context.Almacen.Code,
					CanUseCum = ActivitiesContext.Context.indicaCum,
					Year = ActivitiesContext.Context.year,
					Month = ActivitiesContext.Context.month,
					Medicaments = medicaments,
					ExpiringHours = ActivitiesContext.Context.NumberHours ?? 0,
					LB = LB.Value,
					Clinic = "N"
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("ValidateArticle", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResultValidate (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(Activity, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}
			}
		}
		#endregion

		#region Result Validate
		/// <summary>
		/// Results the validate.
		/// </summary>
		/// <returns>The validate.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task ResultValidate (HttpResponseMessage result)
		{
			try {
				if (result.IsSuccessStatusCode) {

					string responseJsonText = await result.Content.ReadAsStringAsync ();
					var medicamentVarcode = JsonConvert.DeserializeObject<Medicament> (responseJsonText);

					bool indicatorValidate = false;

					for (int i = 0; i < ActivitiesContext.Context.listmedicament.Count; i++) {
						
						if (ActivitiesContext.Context.listmedicament [i].Code.Equals (medicamentVarcode.Code)) {

							if (String.IsNullOrEmpty (ActivitiesContext.Context.listmedicament [i].ResponseCauses)) {
								
								if (ActivitiesContext.Context.listmedicament [i].PendingAmount != ActivitiesContext.Context.listmedicament [i].AmountAlistada) {

									indicatorValidate = false;

									if (!ActivitiesContext.Context.listmedicament [i].ExpiringSoon)
										ActivitiesContext.Context.listmedicament [i].ExpiringSoon = medicamentVarcode.ExpiringSoon;

									ActivitiesContext.Context.listmedicament [i].AmountAlistada++;

									if(ActivitiesContext.Context.listmedicament [i].AmountAlistada > medicamentVarcode.AvailableBalance){
										Toast.MakeText (Activity, String.Format("El artículo solo tiene {0} unidades de saldo", medicamentVarcode.AvailableBalance), ToastLength.Long).Show ();
										ActivitiesContext.Context.listmedicament [i].AmountAlistada--;
										break;
									}

									if (ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM == null)
										ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM = new List<DetLotCumReg> ();

									ActivitiesContext.Context.listmedicament [i].ListItemDetailCumM.Add (new DetLotCumReg {
										NumberLote = medicamentVarcode.BatchNumber ?? String.Empty,  
										CodeMedic = medicamentVarcode.CumCode ?? String.Empty,
										Invima = medicamentVarcode.InvimaCode,
										lotNumber = 1,
										DocumentSourceEnter = medicamentVarcode.OrderSource ?? String.Empty,
										DocumentEnter = medicamentVarcode.OrderDocument,
										ServicesEnter = medicamentVarcode.Warehouse ?? String.Empty,
										ExpirationDate = medicamentVarcode.ExpirationDate
									});

									break;
								}else{
									indicatorValidate = true;
								}
							}
						}
					}

					if (indicatorValidate)
						Toast.MakeText (Activity, String.Format ("La cantidad alistada no puede superar la cantidad solicitada"), ToastLength.Long).Show ();

					listMedicine.Adapter = new AdapMedicines (Activity, ActivitiesContext.Context.listmedicament, listMedicine);

				} else {
					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
					Toast.MakeText (Activity, responseInstance, ToastLength.Long).Show ();
				}
			} catch (Exception ex) {
				Toast.MakeText (Activity, String.Format (GetString (APPDroid.Framework.Resource.String.txt_error), ex.Message), ToastLength.Long).Show ();
			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
				editCodMe.Text = "";
			}
		}
		#endregion

		#region Service Medicine
		/// <summary>
		/// Services the medicine.
		/// </summary>
		async void ServiceMedicine ()
		{
			progressDialog.Show ();

			var Cmocarmed = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault (v => v.Code.Equals ("cmocarmed"));
			var CSv = Cmocarmed.Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("CS"));

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.DrugCharges)) {
				var request = new
				{
					PatientOrders = ActivitiesContext.Context.listPatients [ActivitiesContext.Context.PositionPatient].Codedocument,
		            Year = ActivitiesContext.Context.year,
		            Month = ActivitiesContext.Context.month,
		            Warehouse = ActivitiesContext.Context.Almacen.Code,                   
		            CS = CSv.Value,
		            ActivitiesContext.Context.AttachedConcept,
					PatientEad = ActivitiesContext.Context.EadPatient
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("DrugsByPatient", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await ResponseMedicine (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(Activity, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
					if (progressDialog.IsShowing) {
						progressDialog.Hide();
					}
				}
			}
		}
		#endregion

		#region Response Medicine
		/// <summary>
		/// Responses the medicine.
		/// </summary>
		/// <returns>The medicine.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task ResponseMedicine (HttpResponseMessage response)
		{
			try {
				if (response.IsSuccessStatusCode) {
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					medicinePatient = JsonConvert.DeserializeObject<List<Medicament>> (responseJsonText);

					ActivitiesContext.Context.listmedicament = medicinePatient;
		                  
					ActivitiesContext.Context.AdministerData = new int[ActivitiesContext.Context.listmedicament.Count];
					for (int i = 0; i < ActivitiesContext.Context.AdministerData.Length; i++)
						ActivitiesContext.Context.AdministerData [i] = (int)AdministerType.None;

					ActivitiesContext.Context.PositionOnAdminister = -1;

					//medicinePatient = medicinePatient.OrderByDescending(l => l.ItemLocation).ThenBy (l => l.OrderDate).ToList ();

					listMedicine.Adapter = new AdapMedicines (Activity, medicinePatient, listMedicine);

				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html"))
						Toast.MakeText (Activity, ExceptionMsg, ToastLength.Long).Show ();
					else
						Toast.MakeText (Activity, JsonConvert.DeserializeObject<string> (ExceptionMsg), ToastLength.Long).Show ();	
				}
			} catch (Exception ex) {
				Toast.MakeText (Activity, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();	
			} finally {
				if (progressDialog.IsShowing) {
					progressDialog.Hide ();
				}
			}
		}
		#endregion

	}
}

