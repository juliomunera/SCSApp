
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;
using Android.Graphics;
using SCSAPP.Framework.Context;
using SCSAPP.Services.Messages;

namespace APPDroid.CarMed.Adapters
{
	public class AdapCargarPatients : BaseAdapter
	{
		#region Variables and Controls
		readonly Activity context;
		readonly MasterItem colletpin;
		const string órdenesPendientes = "Órdenes pendientes: {0}";
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.CarMed.Adapters.AdapCargarPatients"/> class.
		/// </summary>
		/// <param name="actividad">Actividad.</param>
		public AdapCargarPatients(Activity actividad)
		{
			context = actividad;
			colletpin = ContextApp.Instance.SelectedEAD.ConfigurationVariables.FirstOrDefault (p => p.Code.ToUpper ().Equals ("COLLETPIN"));
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count
		{
			get { return ActivitiesContext.Context.listPatients.Count; }
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
		public override Java.Lang.Object GetItem(int position)
		{
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
		public override long GetItemId(int position)
		{
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
			convertView = inflater.Inflate(Resource.Layout.item_list_patients, parent, false);

			TextView nombre = convertView.FindViewById<TextView> (Resource.Id.txtNombrePaciente);
			nombre.Text = string.Format("{0} {1} {2} {3}",
				ActivitiesContext.Context.listPatients[position].FirstPatient, ActivitiesContext.Context.listPatients[position].SecondPatient, 
				ActivitiesContext.Context.listPatients[position].Surname, ActivitiesContext.Context.listPatients[position].SecondSurname );

			TextView historia = convertView.FindViewById<TextView> (Resource.Id.txtHistoria);
			historia.Text = string.Format ("Historia: {0} - {1}", ActivitiesContext.Context.listPatients[position].HistoryPatient, 
																  ActivitiesContext.Context.listPatients[position].NumberEntryEntry);

			TextView orden = convertView.FindViewById<TextView> (Resource.Id.txtOrdenes);
			orden.Text = string.Format (órdenesPendientes, ActivitiesContext.Context.listPatients [position].ordersQuantity);
			
			if(!ActivitiesContext.Context.listPatients[position].ClientStatus){
				nombre.SetTextColor (Color.ParseColor (colletpin.Value));
				historia.SetTextColor (Color.ParseColor (colletpin.Value));
				orden.SetTextColor (Color.ParseColor (colletpin.Value));
			}

			return convertView;

		}
		#endregion

	}

}

