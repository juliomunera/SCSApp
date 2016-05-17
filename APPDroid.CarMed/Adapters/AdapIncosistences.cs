
using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages.Entities;
using Android.Graphics;
using APPDroid.Framework.Services;
using System.Net.Http;
using Newtonsoft.Json;
using APPDroid.CarMed.Activities;

namespace APPDroid.CarMed.Adapters
{		
	public class AdapIncosistences : BaseAdapter
	{
		#region Variables and Controls
		readonly Activity context;
		List<Incosistence> data;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.CarMed.Adapters.AdapIncosistences"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		public AdapIncosistences(Activity context, List<Incosistence> data){
			this.context = context;
			this.data = data;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count{
			get { return data.Count;}
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
		/// <Docs>The position of the item within the adapter's data set whose row id we want.</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Get the row id associated with the specified position in the list.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the item identifier.
		/// </summary>
		/// <param name="position">Position.</param>
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
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
			convertView = inflater.Inflate(Resource.Layout.item_incosistences, parent, false);

			TextView epi = convertView.FindViewById<TextView>(Resource.Id.txtEpi);
			epi.Text = String.Format("EPI: {0}", data[position].Episode);

			TextView patient = convertView.FindViewById<TextView>(Resource.Id.txtPatient);
			patient.Text = String.Format("Paciente: {0}", data [position].Patient);

			ImageButton enviar = convertView.FindViewById<ImageButton> (Resource.Id.enviarCargo);
			enviar.Click += (sender, e) => GetIncosistencesUpdate (data [position].Sequence);

			TextView programa = convertView.FindViewById<TextView>(Resource.Id.txtPrograma);

			if (data [position].Program == "cmocarmed") {
				programa.Text = "Cargos de Medicamentos";
			} else if (data [position].Program == "cmodevmed") {
				programa.Text = "Devolución de Medicamentos";
			}
				
			LinearLayout tabla = convertView.FindViewById<LinearLayout> (Resource.Id.tblInconsistencias);

			for (int i = 0; i < data [position].Incosistences.Count; i++) {

				var currentRow = new LinearLayout(context);

				var paramsw = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

				var currentText = new TextView(context);
				currentText.Text = String.Format("{0}. {1}", i+1, data [position].Incosistences[i]);
				currentText.SetTextSize(Android.Util.ComplexUnitType.Sp, 12);
				currentText.SetTextColor(Color.White);

				currentRow.LayoutParameters = paramsw;
				currentRow.AddView(currentText);
				tabla.AddView(currentRow);
			}
				
			return convertView;
		}
		#endregion

		#region Get Incosistences Update
		/// <summary>
		/// Gets the incosistences update.
		/// </summary>
		/// <param name="sequent">Sequent.</param>
		public async void GetIncosistencesUpdate (int sequent)
		{
			using (var httpClient = WebServices.GetBaseHttpClient (SCSAPP.Framework.Context.URIType.Inconsistences)) {
				var jsonRequest = JsonConvert.SerializeObject (sequent, new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
				try{
					var result = await httpClient.PostAsync ("RetrySaveOperation", new StringContent (jsonRequest, Encoding.UTF8, "application/json"));
					await SetIncosistencesUpdate (result);
				}catch (Exception ex) {
					Toast toast = Toast.MakeText(context, String.Format ("Falló de conexión: {0}", ex.Message ), ToastLength.Long);
					toast.SetGravity(GravityFlags.Center, 0, 0);
					toast.Show();
				}
			}
		}
		#endregion

		#region Set Incosistences Update
		/// <summary>
		/// Sets the incosistences update.
		/// </summary>
		/// <returns>The incosistences update.</returns>
		/// <param name="response">Response.</param>
		public async System.Threading.Tasks.Task SetIncosistencesUpdate (HttpResponseMessage response)
		{
			try {

				if (response.IsSuccessStatusCode) {
					context.Finish ();

					var inten = new Intent (context, typeof(ActInconsistencia));
					context.StartActivity (inten);
					//string responseJsonText = await response.Content.ReadAsStringAsync ();
					//List<Incosistence> inconsistences = JsonConvert.DeserializeObject<List<Incosistence>> (responseJsonText);

				} else {
					string ExceptionMsg = await response.Content.ReadAsStringAsync ();
					if (ExceptionMsg.ToLower ().Contains ("html")) {
						Toast.MakeText (context, ExceptionMsg, ToastLength.Long).Show ();
					} else {
						string responseInstance = JsonConvert.DeserializeObject<string> (ExceptionMsg);
						Toast.MakeText (context, responseInstance, ToastLength.Long).Show ();
					}
				}

			} catch (Exception ex) {
				Toast.MakeText (context, String.Format (context.Resources.GetString (APPDroid.Framework.Resource.String.txt_error_servicio), ex.Message), ToastLength.Long).Show ();
			} finally {

			}
		}
		#endregion

	}

}

