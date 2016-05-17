using System;
using System.Text;

using SCSAPP.Framework.Services;

namespace APPDroid.Framework.Helpers
{
	/// <summary>
	/// License controller.
	/// </summary>
	public class LicenseController
	{
		#region Constants
		public const string STR_SERVICE_URL = "http://dmr.servinte.com.co/DMRSite/ClientCenterServices/WServiceInstaller.asmx";
		private const string STR_ERROR_CREDENTIALS_MSG = "Usuario y/o contraseña incorrectos. Por favor, verifique la información digitada.";
		private const string STR_ERROR_NO_MODULE_INFO = "El Cliente no tiene ningún módulo activo asociado a su cuenta";
		private const string STR_MODULE_ADMMED = "ADMMED";
		private const string STR_MODULE_DEVMED = "DEVMED";
		private const string STR_MODULE_CARMED = "CARMED";
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the client Identifier
		/// </summary>
		/// <value>The client I.</value>
		public string ClientID {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the client Password
		/// </summary>
		/// <value>The client PW.</value>
		public string ClientPWD {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.LicenseController"/> have the module adm med activated.
		/// </summary>
		/// <value><c>true</c> if have adm med; otherwise, <c>false</c>.</value>
		public bool HaveAdmMed {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.LicenseController"/> have the module car med activated.
		/// </summary>
		/// <value><c>true</c> if have car med; otherwise, <c>false</c>.</value>
		public bool HaveCarMed {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.LicenseController"/> have the module dev med activated.
		/// </summary>
		/// <value><c>true</c> if have dev med; otherwise, <c>false</c>.</value>
		public bool HaveDevMed {
			get;
			set;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.Framework.LicenseController"/> class.
		/// </summary>
		/// <param name="ArgClientId">Argument client identifier.</param>
		/// <param name="ArgClientPwd">Argument client pwd.</param>
		public LicenseController (string ArgClientId, string ArgClientPwd)
		{
			ClientID = ArgClientId;
			ClientPWD = ArgClientPwd;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the license.
		/// </summary>
		public ServiceResponse ValidateLicense()
		{
			try {
				WServiceInstaller.WServiceInstaller Service = new APPDroid.Framework.WServiceInstaller.WServiceInstaller ();
				Service.Url = STR_SERVICE_URL;

				bool Validated = Service.ValidateClientCredentials (Convert.ToBase64String (Encoding.UTF8.GetBytes (ClientID)), Convert.ToBase64String (Encoding.UTF8.GetBytes (ClientPWD)));

				if (!Validated) {
					return ServiceResponse.CreateResponse(false, null, STR_ERROR_CREDENTIALS_MSG);
				} else {

					var AvailableModules = Service.GetActiveModules(ClientID, 4, false, string.Empty, string.Empty);
					if (!String.IsNullOrEmpty(AvailableModules)) {
						string[] Components = AvailableModules.Split(new char[] { '|' });
						HaveAdmMed = ExistComponent(Components, STR_MODULE_ADMMED);
						HaveCarMed = ExistComponent(Components, STR_MODULE_CARMED);
						HaveDevMed = ExistComponent(Components, STR_MODULE_DEVMED);	

						return ServiceResponse.CreateResponse(true, string.Empty, string.Empty);
					}
					else {
						return ServiceResponse.CreateResponse(false, null, STR_ERROR_NO_MODULE_INFO);
					}
				}	
			} catch (Exception ex) {
				return ServiceResponse.CreateResponse (false, null, ex.Message);
			}
		}

		/// <summary>
		/// Validate If Exist Component in Array Parameter Value
		/// </summary>
		/// <param name="ComponentsArray"></param>
		/// <param name="ComponentID"></param>
		/// <returns></returns>
		private static bool ExistComponent(string[] ComponentsArray, string ComponentID)
		{
			if (ComponentsArray != null && ComponentsArray.Length > 0)
			{
				foreach (String Component in ComponentsArray)
				{
					if (!String.IsNullOrEmpty(Component) && Component.Equals(ComponentID))
						return true;
				}
			}
			return false;
		}
		#endregion
	}
}

