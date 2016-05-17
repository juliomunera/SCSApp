using Android.Content;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages;
using APPDroid.Framework.Context;

namespace APPDroid.AdmMed.Adapters
{		
	public class AdaPacientes : BaseAdapter
	{
		#region Variables and Controls
		readonly Context context;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.AdmMed.Adapters.AdaPacientes"/> class.
		/// </summary>
		/// <param name="c">C.</param>
		/// <param name="c">Patient.</param>
		public AdaPacientes(Context c){
			context = c;
		}
		#endregion

		#region Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public override int Count
		{
			get { return ActivitiesContext.Context.PatientsLoadedInPatientList.Count; }
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
		public override long GetItemId(int position){return position;}
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
			convertView = inflater.Inflate(Resource.Layout.item_patient, parent, false);

			var Patient = ActivitiesContext.Context.PatientsLoadedInPatientList [position];
			if (Patient != null) 
			{
				convertView.FindViewById<TextView> (Resource.Id.txtNombreP).Text = string.Format ("{0} {1} {2}", Patient.FirstName, Patient.MiddleName, Patient.LastName);      
				convertView.FindViewById<TextView>(Resource.Id.txtHistoria).Text = string.Format ("Historia. {0}", Patient.History); 
				convertView.FindViewById<TextView>(Resource.Id.txtCama).Text = string.Format ("Cama. {0}", Patient.Bed);	
			}
			return convertView;
		}
		#endregion

	}

}

