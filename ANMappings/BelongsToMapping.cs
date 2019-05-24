namespace ANMappings {
	using System.Reflection;

	public class BelongsToMapping<T, TReference> : AssociationMapping<T, TReference>, IBelongsToMapping {

		public BelongsToMapping(MemberInfo property) : base(property) {		
		}
	}
}