namespace ANMappings {
	using System.Reflection;

	/// <summary>
	/// Mapping class that represents a true one-to-one mapping.
	/// </summary>
	public class HasOneMapping<T, TReference> : AssociationMapping<T, TReference>, IHasOneMapping {
		/// <summary>
		/// Creates a new instance of the HasOneMapping class.
		/// </summary>
		/// <param name="property">The property that represents the association</param>
		public HasOneMapping(MemberInfo property) : base(property) {}
	}
}