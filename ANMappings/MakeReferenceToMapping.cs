namespace ANMappings {
	using System.Reflection;

	public class MakeReferenceToMapping<T, TReference> : AssociationMapping<T, TReference>, IMakeReferenceToMapping {

        public MakeReferenceToMapping(MemberInfo property)
            : base(property)
        {		
		}
	}
}