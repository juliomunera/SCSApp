
using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.OS;
using Android.Widget;
using APPDroid.Framework.Services;
using System.Net.Http;
using Newtonsoft.Json;
using SCSAPP.Services.Messages.Entities;
using APPDroid.CarMed.Adapters;
using APPDroid.Framework.Helpers;
using Android.Views;

namespace APPDroid.CarMed.Activities
{
	[Activity (Label = "Inconsistencias", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]			
	public class ActInconsistencia : Activity
	{
		#region Variables and Controls
		ListView listIncon;
		#endregion

		#region OnCreate Activity
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.layout_inconsistencia);

			listIncon = FindViewById<ListView> (Resource.Id.listViewTablero);

			//var telephonyManager = (TelephonyManager) GetSystemService(Context.TelephonyService);
			//string deviceId = telephonyManager.DeviceId;
			string ContextImei = DataBaseManager.GetContexts (DataBaseManager.IDContextType.imei);
			GetInconsistences (ContextImei);

		}
		#endregion

		#region GetInconsistences
		/// <summary>
		/// Gets the inconsistences.
		/// </summary>
		/// <param name="CodeImei">Code imei.</param>
		public async void GetInconsistences(string CodeImei)
		{
			using (var httpClient = WebServices.GetBaseHttpClient (SCSAPP.Framework.Context.URIType.Inconsistences)) 
			{
				var jsonRequest = JsonConvert.SerializeObject (CodeImei, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("GetInconsistences", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await SetInconsistences (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(this, String.Format ("Falló de conexión: {0}", ex.Message), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region SetInconsistences
		/// <summary>
		/// Sets the inconsistences.
		/// </summary>
		/// <returns>The inconsistences.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task SetInconsistences(HttpResponseMessage response){
			try {
				
				if (response.IsSuccessStatusCode) {
					string responseJsonText = await response.Content.ReadAsStringAsync ();
					List<Incosistence> inconsistences = JsonConvert.DeserializeObject<List<Incosistence>> (responseJsonText);

					if(inconsistences.Count > 0){
						
						listIncon.Adapter = new AdapIncosistences(this, inconsistences);

					}else{
						Toast.MakeText (this, "No se encontraron Inconsistencias", ToastLength.Long).Show ();	
					}
				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html")) {
						Toast.MakeText (this, ExceptionMsg, ToastLength.Long).Show ();	
						Finish ();
					} else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast.MakeText (this, responseInstance, ToastLength.Long).Show ();
					}
				}
			} catch (Exception ex) {
				Toast.MakeText (this, String.Format (Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();
			} finally {
				
			}
		}
		#endregion

	}

}

