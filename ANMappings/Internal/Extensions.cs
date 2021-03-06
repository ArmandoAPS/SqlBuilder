namespace ANMappings.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// Extension methods
	/// </summary>
	public static class Extensions {
		internal static void Guard(this object obj, string message) {
			if (obj == null) {
				throw new ArgumentNullException(message);
			}
		}

		internal static void Guard(this string str, string message) {
			if (string.IsNullOrEmpty(str)) {
				throw new ArgumentNullException(message);
			}
		}

		/// <summary>
		/// Gets the property name from a member expression.
		/// </summary>
		public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> expression) {
			var memberExp = RemoveUnary(expression.Body);

			if (memberExp == null) {
				throw new InvalidOperationException(string.Format("Expected a member expression for expression: {0}", expression));
			}
            // hack: para retornar nombres compuestos ejemplo Topic.Name
            string strMemberExp = memberExp.Expression.ToString();
            if(strMemberExp.IndexOf(".") > 0)
            {
                
                int pLen = ((MemberExpression)memberExp.Expression).Expression.ToString().Length;
                return memberExp.ToString().Substring(pLen + 1);
            }
            else
			    return memberExp.Member.Name;
		}

		public static Type ReturnType(this MemberInfo member) {
			if (member is PropertyInfo) {
				return ((PropertyInfo)member).PropertyType;
			}

			if (member is FieldInfo) {
				return ((FieldInfo)member).FieldType;
			}

			return null;
		}

		public static MemberInfo GetMember<T, TMember>(this Expression<Func<T, TMember>> expression) {
			var memberExp = RemoveUnary(expression.Body);

			if(memberExp == null) {
				return null;
			}

			return memberExp.Member;
		}

		public static object GetValue(this MemberInfo member, object obj) {
			if(member is PropertyInfo) {
				return ((PropertyInfo)member).GetValue(obj, null);
			}

			if(member is FieldInfo) {
				return ((FieldInfo)member).GetValue(obj);
			}

			return null;
		}

		public static void SetValue(this MemberInfo member, object obj, object value) {
			if (member is PropertyInfo) {
				((PropertyInfo)member).SetValue(obj, value, null);
			}
			else if (member is FieldInfo) {
				((FieldInfo)member).SetValue(obj, value);
			}
		}

		/// <summary>
		/// Gets the PropertyInfo from a member expression
		/// </summary>
		public static PropertyInfo GetProperty<T, TProperty>(this Expression<Func<T, TProperty>> expression) {
			var memberExp = RemoveUnary(expression.Body);

			if (memberExp == null) {
				return null;
			}

			return memberExp.Member as PropertyInfo;
		}

		/// <summary>
		/// Gets the MethodInfo from a Lambda expression.
		/// </summary>
		public static MethodInfo GetMethod(this LambdaExpression expression) {

            if (!(expression.Body is MethodCallExpression methodCall))
            {
                return null;
            }

            return methodCall.Method;
		}

		private static MemberExpression RemoveUnary(Expression toUnwrap) {
			if (toUnwrap is UnaryExpression) {
				return (MemberExpression)((UnaryExpression)toUnwrap).Operand;
			}

			return toUnwrap as MemberExpression;
		}

		internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source) {
				action(item);
			}
		}
	}
}