namespace ANMappings {
	using System.Reflection;

	/// <summary>
	/// Defines a mapped property
	/// </summary>
	public interface IPropertyMapping{
		/// <summary>
		/// The property that is being mapped
		/// </summary>
		MemberInfo Property { get; }
	}
}