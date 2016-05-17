
using Android.App;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;

namespace APPDroid.CarMed.Adapters
{		
	public class ExpandableCarPatien : BaseExpandableListAdapter
	{
		#region Variables and Controls
		readonly Activity Context;
		int position;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.CarMed.Adapters.ExpandableCarPatien"/> class.
		/// </summary>
		/// <param name="newContext">New context.</param>
		/// <param name="position">Position.</param>
		public ExpandableCarPatien(Activity newContext, int position) : base()
		{
			Context = newContext;
			this.position = position;
		}
		#endregion

		#region GetGroupView
		/// <Docs>the position of the group for which the View is
		///  returned</Docs>
		/// <summary>
		/// Gets the group view.
		/// </summary>
		/// <returns>The group view.</returns>
		/// <param name="groupPosition">Group position.</param>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			View header = convertView ?? Context.LayoutInflater.Inflate (Resource.Layout.app_nombre_paciente_car, null);

			TextView nombrePrincipal = header.FindViewById<TextView> (Resource.Id.txtDataHeaderCar);
			nombrePrincipal.Text = string.Format("{0} {1} {2} {3}", 
				ActivitiesContext.Context.listPatients [position].FirstPatient, ActivitiesContext.Context.listPatients [position].SecondPatient,
				ActivitiesContext.Context.listPatients [position].Surname, ActivitiesContext.Context.listPatients [position].SecondSurname);

			return header;			
		}
		#endregion

		#region GetChildView
		/// <Docs>the position of the group that contains the child</Docs>
		/// <param name="isLastChild">Whether the child is the last child within the group</param>
		/// <summary>
		/// Gets the child view.
		/// </summary>
		/// <returns>The child view.</returns>
		/// <param name="groupPosition">Group position.</param>
		/// <param name="childPosition">Child position.</param>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			View row = convertView;
			if (row == null) {
				row = Context.LayoutInflater.Inflate (Resource.Layout.layout_car_detalle_paciente, null);

				row.FindViewById<TextView> (Resource.Id.txtHistoria).Text = string.Format ("Historia: {0} - {1}", ActivitiesContext.Context.listPatients [position].HistoryPatient, ActivitiesContext.Context.listPatients [position].NumberEntryEntry);
				row.FindViewById<TextView> (Resource.Id.txtUbicacion).Text = string.Format ("{0}", ActivitiesContext.Context.servicesCar);
				row.FindViewById<TextView> (Resource.Id.txtIdentificacion).Text = string.Format ("{0}. {1}", ActivitiesContext.Context.listPatients [position].TypeEntity, ActivitiesContext.Context.listPatients [position].NumberEntity);
				row.FindViewById<TextView> (Resource.Id.txtEdad).Text = string.Format ("Edad: {0}", ActivitiesContext.Context.listPatients [position].Age);
				row.FindViewById<TextView> (Resource.Id.txtUbicacion2).Text = string.Format ("Ubi: {0}", ActivitiesContext.Context.NumberUbic);
				row.FindViewById<TextView> (Resource.Id.txtCantidad).Text = string.Format ("Cantidad de órdenes: {0}", ActivitiesContext.Context.listPatients [position].ordersQuantity);
			}

			return row;
		}
		#endregion

		#region GetChildrenCount
		/// <Docs>the position of the group for which the children
		///  count should be returned</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Gets the number of children in a specified group.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the children count.
		/// </summary>
		/// <param name="groupPosition">Group position.</param>
		public override int GetChildrenCount (int groupPosition)
		{
			return 1;
		}
		#endregion

		#region GroupCount
		/// <summary>
		/// Gets the group count.
		/// </summary>
		/// <value>The group count.</value>
		public override int GroupCount {
			get {
				return 1;
			}
		}
		#endregion

		#region implemented abstract members of BaseExpandableListAdapter
		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return null;	
			//throw new NotImplementedException ();
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
			//throw new NotImplementedException ();
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			//throw new NotImplementedException ();
			return true;
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}
		#endregion

	}
}

