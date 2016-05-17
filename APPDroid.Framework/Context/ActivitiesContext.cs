using System;
using SCSAPP.Services.Messages;
using System.Collections.Generic;

namespace APPDroid.Framework.Context
{
	#region Main typeclass
	/// <summary>
	/// Activities context.
	/// </summary>
	public static class ActivitiesContext
	{
		/// <summary>
		/// The activities types context.
		/// </summary>
		private static ActivitiesTypes _activitiesTypesContext;

		/// <summary>
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		public static ActivitiesTypes Context {
			get { return ActivitiesContext._activitiesTypesContext; }
		}
		/// <summary>
		/// Init the specified Context Activities Types.
		/// </summary>
		/// <param name="InitialTypes">Initial types.</param>
		public static void Init (ActivitiesTypes InitialTypes)
		{
			_activitiesTypesContext = InitialTypes;
		}
	}
	#endregion
}