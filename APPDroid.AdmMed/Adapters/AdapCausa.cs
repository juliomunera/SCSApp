using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages;
using SCSAPP.Framework.Context;
using APPDroid.AdmMed.Activities;
using APPDroid.Framework.Context;
using APPDroid.Framework.Services;
using System.Net.Http;
using Newtonsoft.Json;

namespace APPDroid.AdmMed.Adapters
{			
	public class AdapCausa : BaseAdapter{

		#region Variables and Controls
		readonly Context context;
		List<MasterItem> listCauses;
		AlertDialog alertDialog;
		#endregion

		#region constructor method
		public AdapCausa(Context context, List<MasterItem> listCauses, AlertDialog alertDialog){
			this.context = context;
			this.listCauses = listCauses;
			this.alertDialog = alertDialog;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get { return listCauses.Count;}
		}
		#endregion

		#region GetItem
		/// <Docs>Position of the item whose data we want within the adapter's 
		///  data set.</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Get the data item associated with the specified position in the data set.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <param name="position">Position.</param>
		public override Java.Lang.Object GetItem(int position){
			return null;
		}
		#endregion

		#region GetItemId
		public override long GetItemId(int position){
			return position;
		}
		#endregion

		#region GetView
		/// <Docs>The position of the item within the adapter's data set of the item whose view
		///  we want.</Docs>
		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <returns>The view.</returns>
		/// <param name="position">Position.</param>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetView(int position, View convertView, ViewGroup parent){

			var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
			convertView = inflater.Inflate(APPDroid.Framework.Resource.Layout.items_master, parent, false);

			TextView nombre = convertView.FindViewById<TextView> (APPDroid.Framework.Resource.Id.EadNombre);
			nombre.Text = listCauses[position].Value.Trim();

			convertView.Click += (sender, e) => {

				var solicitaFirma = ContextApp.Instance.SelectedEAD.Programs.FirstOrDefault ().Variables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("SF"));

				ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.NoAdministrationCause = listCauses [position].Code.Trim ();
				ActivitiesContext.Context.Patient.Medicaments [ActivitiesContext.Context.PositionOnAdminister].Dose.IsAdministered = false;

				if (solicitaFirma.Value.Equals ("S")) {
					//Siempre pide firma
					alertDialog.Dismiss ();
					ActivitiesContext.Context.IsScanning = false;
					ActivitiesContext.Context.AdministarMedicament = false;
					var windFirma = new Intent (context, typeof(ActConfirmar));
					context.StartActivity (windFirma);

				} else if (solicitaFirma.Value.Equals ("N") || solicitaFirma.Value.Equals (String.Empty)) {
					//Solo si es por primera ves
					alertDialog.Dismiss ();
					if (ActivitiesContext.Context.FirstPrimary) {
						//ActivitiesContext.Context.FirstPrimary = false;
						ActivitiesContext.Context.IsScanning = false;
						ActivitiesContext.Context.AdministarMedicament = false;
						var windFirma = new Intent (context, typeof(ActConfirmar));
						context.StartActivity (windFirma);
					} else {
						AdministerHandler ();	
					}
				} else if (solicitaFirma.Value.Equals ("J")) {
					//Nunca pide firma
					alertDialog.Dismiss ();
					AdministerHandler ();
				}
			};

			return convertView;
		}
		#endregion

		#region AdministerHandler
		/// <summary>
		/// Administers the handler.
		/// </summary>
		async void AdministerHandler (){
			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {

				var request = new
				{
					Medicament = ActivitiesContext.Context.Patient.Medicaments[ActivitiesContext.Context.PositionOnAdminister],
					Patient = new { Episode = ActivitiesContext.Context.Patient.Episode, Location = new {Code = ActivitiesContext.Context.Patient.Location.Code}},
					User = new {Code = ContextApp.Instance.User.Code, Password = ContextApp.Instance.User.Password, AdministrativeStructure = ContextApp.Instance.SelectedEAD.Code, MainSpeciality = new { Code = ContextApp.Instance.SelectedSpeciality.Code }},
					Diluent = new {}
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("Administer", new StringContent (jsonRequest, System.Text.Encoding.UTF8, "application/json"));
					await AdministerResponse (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(context, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region Administer Response
		/// <summary>
		/// Administers the response.
		/// </summary>
		/// <returns>The response.</returns>
		/// <param name="result">Result.</param>
		public async System.Threading.Tasks.Task AdministerResponse (HttpResponseMessage result){
			try{
				if (result.IsSuccessStatusCode) {
					ActivitiesContext.Context.AdministerData[ActivitiesContext.Context.PositionOnAdminister] = (int)AdministerType.NoAdmin;
					var intent = new Intent(context, typeof(ActPacienteExmpan));
					context.StartActivity(intent);

				} else {

					LoadPatientHistoryCausa ();

					string ExceptionMsg = await result.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(context, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			} catch (Exception ex) {
				Toast toast = Toast.MakeText(context, string.Format ("Error: {0}", ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

		#region Load PatientHistory Causa
		/// <summary>
		/// Loads the patient history.
		/// </summary>
		public async void LoadPatientHistoryCausa (){

			using (var httpClient = WebServices.GetBaseHttpClient (URIType.Drugs_Administration)) {
				var request = new {
					History = ActivitiesContext.Context.Patient.History,
					InitialDate = ActivitiesContext.Context.InitialDate,
					FinalDate = ActivitiesContext.Context.FinalDate
				};
				var jsonRequest = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("Patient", new StringContent (jsonRequest, System.Text.Encoding.UTF8, "application/json"));
					await StartPatientCausa (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(context, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region Start Patient Causa
		/// <summary>
		/// Starts the patient.
		/// </summary>
		/// <returns>The patient.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task StartPatientCausa (HttpResponseMessage response)	{
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

						var ventana = new Intent (context, typeof(ActPacienteExmpan));
						context.StartActivity (ventana);

					}else{
						Toast toast = Toast.MakeText(context, responseInstance.ToString(), ToastLength.Long);
						toast.SetGravity(GravityFlags.Center, 0, 0);
						toast.Show();
					}

				}else{
					
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);

					Toast toast = Toast.MakeText(context, responseInstance, ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}	

			} catch (Exception ex) {
				Toast toast = Toast.MakeText(context, string.Format(context.Resources.GetString(APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long);
				toast.SetGravity(GravityFlags.Center, 0, 0);
				toast.Show();
			}
		}
		#endregion

	}
}

