using System;
using System.Net;
using System.Threading.Tasks;

using SCSAPP.Framework.Context;
using SCSAPP.Framework.Services;

namespace APPDroid.Framework.Services
{
	/// <summary>
	/// Web services implementation
	/// </summary>
	public class WebServices
	{
		#region Public Methods
		/// <summary>
		/// Call Ping Method in Authentication Service
		/// </summary>
		/// <returns>The URI.</returns>
		public async Task<ServiceResponse> PingUri()
		{
			try {
				using (WebClient clientHttp = new WebClient ()) {

					Uri uri = new Uri (String.Format ("{0}/Ping", MobileServicesContext.GetUri(URIType.Authentication)));
					string Response = await clientHttp.DownloadStringTaskAsync (uri);
				
					return ServiceResponse.CreateResponse (true, Response, null);
				}
			} catch (Exception ex) {
				return ServiceResponse.CreateResponse (false, null, ex.Message);
			}
		}
		/// <summary>
		/// Gets the base http client.
		/// </summary>
		/// <returns>The base http client.</returns>
		public static System.Net.Http.HttpClient GetBaseHttpClient(URIType UriType) 
		{
			System.Net.Http.HttpClient clientHttp = new System.Net.Http.HttpClient();
			clientHttp.BaseAddress = new Uri (String.Format("{0}/", MobileServicesContext.GetUri(UriType)));
			clientHttp.DefaultRequestHeaders.Accept.Add (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue ("application/json"));

			return clientHttp;
		}

		#endregion
	}
}