using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;

using APPDroid.Framework.Helpers;
using APPDroid.Framework.Services;

using SCSAPP.Framework.Context;
using SCSAPP.Framework.Services;
using Android.Telephony;

namespace AppDroid.Main.Activities
{
	[Activity (Label = "@string/txt_configuracion", Icon = "@drawable/ic_servinte", ParentActivity = typeof(MainActivity), ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class AppSetting : Activity
	{
		#region Variables and Controls
		EditText editCliente, edtPassworCliente, editUrl, edituiDevice;
		Button btnActivar, btnDesactivar;
		Button btnGenerar;
		#endregion

		#region Overrides Methods
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name = "savedInstanceState"></param>	 
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			// Create your application here
			SetContentView (Resource.Layout.app_settings);

			editCliente = FindViewById<EditText> (Resource.Id.editCliente);
			edtPassworCliente = FindViewById<EditText> (Resource.Id.editPassword);
			editUrl = FindViewById<EditText> (Resource.Id.url);
			edituiDevice = FindViewById<EditText> (Resource.Id.uiDevice);

			btnActivar = FindViewById<Button> (Resource.Id.btnActivar);
			btnDesactivar = FindViewById<Button> (Resource.Id.btnDesactivar);
			btnGenerar = FindViewById<Button> (Resource.Id.btnGenerar);

			RestoreFromDBContext();

			btnActivar.Click += BtnActive_Click;
			btnDesactivar.Click += BtnDesactivar_Click;
			btnGenerar.Click += BtnGenerar_Click;

			var version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version;

			TextView versionText = FindViewById<TextView> (Resource.Id.txtVersion);
			TextView versionFecha = FindViewById<TextView> (Resource.Id.txtFecha);

			versionText.Text = String.Format ("v {0}.{1}", version.Major, version.Minor);
			/* ------------------------------
			 * Modificado por: Julio Munera
			 * Fecha: 2016-05-14
			 * ------------------------------ */
			versionFecha.Text = "2016/05/14";

		}
		#endregion

		#region Events click
		void BtnGenerar_Click (object sender, EventArgs e)
		{
			String guid = Guid.NewGuid().ToString();
			edituiDevice.Text = guid;
		}
		#endregion

		#region Events Activar
		/// <summary>
		/// Buttons the active click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void BtnActive_Click (object sender, EventArgs e){

			if (ValiInput ()) {
				DataBaseManager.DeleteContext (DataBaseManager.IDContextType.Wcf);
				DataBaseManager.DeleteContext (DataBaseManager.IDContextType.Lincence);

				var License = new LicenseController (editCliente.Text, edtPassworCliente.Text);
				ServiceResponse res = License.ValidateLicense ();
				if (res.FinalState) {
					//
					// 1. Registrar Contexto de la licencia del Cliente
					//
					var LicCtx = new MobileLicenseContext ();
					LicCtx.ClientCode = License.ClientID;
					LicCtx.HaveAdmMed = License.HaveAdmMed;
					LicCtx.HaveCarMed = License.HaveCarMed;
					LicCtx.HaveDevMed = License.HaveDevMed;

					LicenseContextApp.Init (LicCtx);
					DataBaseManager.InsertContext (DataBaseManager.IDContextType.Lincence, LicenseContextApp.GetContextSerialized());

					//
					// 2. Validar la URL de exposición de los Servicios
					//
					var ServiceContext = new MobileServicesContext ();
					ServiceContext.ServicesBaseURL = editUrl.Text;
					WebServicesContextApp.Init (ServiceContext);

					var webSer = new WebServices ();
					ServiceResponse webRes = await webSer.PingUri ();

					if (webRes.FinalState) {
						DataBaseManager.InsertContext (DataBaseManager.IDContextType.Wcf, WebServicesContextApp.GetContextSerialized());
						DataBaseManager.InsertContext (DataBaseManager.IDContextType.imei, edituiDevice.Text);

						Toast.MakeText (this, "Configuración Exitosa", ToastLength.Short).Show ();

						var windows = new Intent (this, typeof(MainActivity));
						StartActivity (windows);

					} else {
						//webRes.ExceptionMessage
						Toast.MakeText (this, "No se puede activar revise el servicio o la URL.", ToastLength.Short).Show ();
					}
				} else {
					Toast.MakeText (this, res.ExceptionMessage, ToastLength.Short).Show ();
				}
			}
		}
		#endregion

		#region Events Desactivar 
		/// <summary>
		/// Buttons the desactivar click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void  BtnDesactivar_Click (object sender, EventArgs e){
			
			DataBaseManager.DeleteContext (DataBaseManager.IDContextType.Wcf);
			DataBaseManager.DeleteContext (DataBaseManager.IDContextType.Lincence);
			DataBaseManager.DeleteContext (DataBaseManager.IDContextType.imei);

			editCliente.Text = "";
			edtPassworCliente.Text = "";
			editUrl.Text = "";
			edituiDevice.Text = "";

		}
		#endregion

		#region regresar a la activity anterior 
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
			DataBaseManager.DeleteContext(DataBaseManager.IDContextType.ContextApp);
			var inten = new Intent (this, typeof(MainActivity));
			StartActivity (inten);
			Finish ();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Restores from DB context.
		/// </summary>
		void RestoreFromDBContext (){
			//Restore From DB
			string ContextLicence = DataBaseManager.GetContexts (DataBaseManager.IDContextType.Lincence);
			if (!String.IsNullOrEmpty (ContextLicence)) {
				MobileLicenseContext RestoredlicContext = LicenseContextApp.GetContextFromSerializedString (ContextLicence);
				LicenseContextApp.Init (RestoredlicContext);
				editCliente.Text = LicenseContextApp.Instance.ClientCode;
				edtPassworCliente.Text = string.Empty;
			}
			string ContextWcf = DataBaseManager.GetContexts (DataBaseManager.IDContextType.Wcf);
			if (!String.IsNullOrEmpty (ContextWcf)) {
				MobileServicesContext RestoredWcfContext = WebServicesContextApp.GetContextFromSerializedString (ContextWcf);
				WebServicesContextApp.Init (RestoredWcfContext);
				editUrl.Text = WebServicesContextApp.Instance.ServicesBaseURL;
			}
			string ContextImei = DataBaseManager.GetContexts (DataBaseManager.IDContextType.imei);
			if (!String.IsNullOrEmpty (ContextImei)) {
				edituiDevice.Text = ContextImei;
			} else {
				var telephonyManager = (TelephonyManager)GetSystemService (Context.TelephonyService);
				string deviceId = telephonyManager.DeviceId;
				if (!String.IsNullOrEmpty (deviceId)) {
					edituiDevice.Text = deviceId;
				}
			}
		}
		#endregion

		#region Methods validar Campos
		/// <summary>
		/// Validate The User Input Data
		/// </summary>
		/// <returns><c>true</c>, if input was valied, <c>false</c> otherwise.</returns>
		public bool ValiInput (){
			if (String.IsNullOrEmpty (editCliente.Text) || editCliente.Text.Trim ().Length == 0) {
				Toast.MakeText (this, Resource.String.txt_cliente, ToastLength.Short).Show ();
				editCliente.Focusable = true;
				editCliente.FocusableInTouchMode = true; 	
				editCliente.RequestFocus (FocusSearchDirection.Up);
				return false;
			} else if (String.IsNullOrEmpty (edtPassworCliente.Text)) {
				Toast.MakeText (this, Resource.String.txt_password, ToastLength.Short).Show ();	
				edtPassworCliente.Focusable = true;
				edtPassworCliente.FocusableInTouchMode = true; 	
				edtPassworCliente.RequestFocus (FocusSearchDirection.Up);
				return false;
			} else if (String.IsNullOrEmpty (editUrl.Text) || editUrl.Text.Trim ().Length == 0) {
				Toast.MakeText (this, Resource.String.txt_url, ToastLength.Short).Show ();
				editUrl.Focusable = true;
				editUrl.FocusableInTouchMode = true; 	
				editUrl.RequestFocus (FocusSearchDirection.Up);
				return false;
			}else if(String.IsNullOrEmpty (edituiDevice.Text) || edituiDevice.Text.Trim ().Length == 0){
				Toast.MakeText (this, Resource.String.txt_codigo_servicio, ToastLength.Short).Show ();
				edituiDevice.Focusable = true;
				edituiDevice.FocusableInTouchMode = true; 	
				edituiDevice.RequestFocus (FocusSearchDirection.Up);
				return false;
			}

			return true;
		}
		#endregion

	}
}