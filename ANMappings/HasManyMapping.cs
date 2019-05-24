
namespace ANMappings {
	using System.Reflection;

	/// <summary>
	/// Mapping class that represents a one-to-many mapping.
	/// </summary>
	public class HasManyMapping<T, TElement> : AssociationMapping<T, TElement>, IHasManyMapping {
		/// <summary>
		/// Creates a new instance of the HasManyMapping class.
		/// </summary>
		/// <param name="property">The property that represents the association</param>
		public HasManyMapping(MemberInfo property) : base(property) {}
	}
}