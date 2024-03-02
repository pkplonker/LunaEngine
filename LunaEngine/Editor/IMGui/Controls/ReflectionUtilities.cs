using System.Linq.Expressions;

namespace Editor.Controls;

public static class ReflectionUtils
{
	public static string GetMemberName<T>(Expression<Func<T, object>> expression)
	{
		if (expression.Body is MemberExpression member)
		{
			return member.Member.Name;
		}
		else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
		{
			return operand.Member.Name;
		}

		throw new ArgumentException("Expression is not a MemberExpression", nameof(expression));
	}
}