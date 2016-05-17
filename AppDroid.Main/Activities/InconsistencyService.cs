
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using Android.Util;
using APPDroid.CarMed.Activities;
using SCSAPP.Framework.Context;
using APPDroid.Framework.Helpers;

namespace AppDroid.Main.Activities
{
	[Service]
	public class InconsistencyService : Service
	{
		#region Constantes
		const string notificationServiceHubHttpClientException = "NotificationService Hub HttpClientException";
		const string notificationHub = "NotificationHub";
		const string showNotification = "showNotification";
		const string notificationServiceHubException = "NotificationService HubException";
		const string serviceHasStarted = "El servicio ha comenzado";
		const string servicioDeInconsistencias = "Servicio de inconsistencias";
		const string inconsistencias = "Inconsistencias";
		const string verInconsistencias = "Ver inconsistencias.";
		#endregion

		#region Variables
		HubConnection hubConnection;
		IHubProxy clientHubProxy;
		DemoServiceBinder binder;
		#endregion

		#region métodos
		/// <summary>
		/// Raises the start command event.
		/// </summary>
		/// <param name="intent">Intent.</param>
		/// <param name="flags">Flags.</param>
		/// <param name="startId">Start identifier.</param>
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			StartServiceInForeground ();
			StartHubAsync ();
			return StartCommandResult.NotSticky;
		}

		/// <summary>
		/// Starts the hub async.
		/// </summary>
		/// <returns>The hub async.</returns>
		public async Task StartHubAsync ()
		{
			try {
				hubConnection = new HubConnection (MobileServicesContext.GetUri (URIType.Notification));

				clientHubProxy = hubConnection.CreateHubProxy (notificationHub);
				clientHubProxy.On (showNotification, ShowInconsistenciesMessage);

				await hubConnection.Start ();

				string ContextImei = DataBaseManager.GetContexts (DataBaseManager.IDContextType.imei);

				await clientHubProxy.Invoke ("Connect", new object[] {
					ContextImei,
					hubConnection.ConnectionId
				});
			} catch (HttpClientException httpClientException) {
				Log.Debug (notificationServiceHubHttpClientException, httpClientException.Message);
				throw;
			} catch (Exception exception) {
				Log.Debug (notificationServiceHubException, exception.Message);
				throw;
			}
		}

		/// <summary>
		/// Starts the service in foreground.
		/// </summary>
		void StartServiceInForeground ()
		{
			Notification.Builder builder = new Notification.Builder (this)
				.SetAutoCancel (true)
				.SetContentTitle (servicioDeInconsistencias)
				.SetContentText (serviceHasStarted)
				.SetSmallIcon (Resource.Drawable.ic_servinte);
			Notification notification = builder.Build ();
			notification.Flags = NotificationFlags.AutoCancel;

			StartForeground ((int)NotificationFlags.ForegroundService, notification);

		}

		void ShowInconsistenciesMessage ()
		{
			// Set up an intent so that tapping the notifications returns to this app:
			var intent = new Intent (this, typeof(ActInconsistencia));

			// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
			const int pendingIntentId = 0;
			PendingIntent pendingIntent = PendingIntent.GetActivity (this, pendingIntentId, intent, PendingIntentFlags.OneShot);

			// Instantiate the builder and set notification elements:
			Notification.Builder builder = new Notification.Builder (this)
				.SetContentIntent (pendingIntent)
				.SetContentTitle (inconsistencias)
				.SetContentText (verInconsistencias)
				.SetSmallIcon (Resource.Drawable.ic_servinte);


			// Device vibrate rather than play a sound
			builder.SetDefaults (NotificationDefaults.Sound | NotificationDefaults.Vibrate);

			// Build the notification:
			Notification notification = builder.Build ();

			// Turn on vibrate:
			notification.Defaults |= NotificationDefaults.Vibrate;

			//Auto cancel will remove the notification once the user touches it
			notification.Flags = NotificationFlags.AutoCancel;

			// Get the notification manager:
			var notificationManager = GetSystemService (Context.NotificationService) as NotificationManager;

			notificationManager.Notify (1, notification);
		}

		/// <summary>
		/// Raises the bind event.
		/// </summary>
		/// <param name="intent">Intent.</param>
		public override IBinder OnBind (Intent intent)
		{
			binder = new DemoServiceBinder (this);
			return binder;
		}

		/// <Docs>Called by the system to notify a Service that it is no longer used and is being removed.</Docs>
		/// <para tool="javadoc-to-mdoc">Called by the system to notify a Service that it is no longer used and is being removed. The
		///  service should clean up any resources it holds (threads, registered
		///  receivers, etc) at this point. Upon return, there will be no more calls
		///  in to this Service object and it is effectively dead. Do not call this method directly.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		public override void OnDestroy ()
		{
			base.OnDestroy ();

			if (hubConnection != null) {
				hubConnection.Dispose ();
				hubConnection = null;
			}
		}

		#endregion

	}

	#region Services
	/// <summary>
	/// Demo service binder.
	/// </summary>
	public class DemoServiceBinder : Binder
	{
		readonly InconsistencyService service;

		public DemoServiceBinder (InconsistencyService service)
		{
			this.service = service;
		}

		public InconsistencyService GetDemoService ()
		{
			return service;
		}
	}
	#endregion

}

