using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;

namespace Plist.Test.Helpers
{
	public static class MoqExtensions
	{
		public static void SetupStep<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
		{
			var callback = Sequence.Active.Next(expression);
			if (callback != null)
				mock.Setup(expression).Callback(callback).Verifiable();
			
		}

		public static void SetupStep<T>(this Mock<T> mock, string methodName, params object[] args) where T : class
		{

			var method = GetMethod<T>(methodName, args);
			var expr = GetMethodCall<T>(method, args);

			var callback = Sequence.Active.Next(expr);
			if (callback != null)
				mock.Setup(expr).Callback(callback).Verifiable();
				
		}


		private static Expression<Func<T, TResult>> GetMethodCall<T,TResult>(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Func<T, TResult>>(Expression.Call(param, method, ToExpressionArgs(args)), param);
		}

		private static Expression<Action<T>> GetMethodCall<T>(MethodInfo method, object[] args)
		{
			var param = Expression.Parameter(typeof(T), "mock");
			return Expression.Lambda<Action<T>>(Expression.Call(param, method, ToExpressionArgs(args)), param);
		}
		private static MethodInfo GetMethod<T>(string methodName, params object[] args)
		{
			return typeof(T).GetMethod(
				methodName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				null,
				ToArgTypes(args),
				null);
		}
		private static Type[] ToArgTypes(object[] args)
		{
			if (args == null)
			{
				throw new ArgumentException("Use ItExpr.IsNull<TValue> rather than a null argument value, as it prevents proper method lookup.");
			}

			var types = new Type[args.Length];
			for (int index = 0; index < args.Length; index++)
			{
				if (args[index] == null)
				{
					throw new ArgumentException("Use ItExpr.IsNull<TValue> rather than a null argument value, as it prevents proper method lookup.");
				}

				var expr = args[index] as Expression;
				if (expr == null)
				{
					types[index] = args[index].GetType();
				}
				else if (expr.NodeType == ExpressionType.Call)
				{
					types[index] = ((MethodCallExpression)expr).Method.ReturnType;
				}
				else if (expr.NodeType == ExpressionType.MemberAccess)
				{
					var member = (MemberExpression)expr;
					switch (member.Member.MemberType)
					{
						case MemberTypes.Field:
							types[index] = ((FieldInfo)member.Member).FieldType;
							break;
						case MemberTypes.Property:
							types[index] = ((PropertyInfo)member.Member).PropertyType;
							break;
						default:
							throw new NotSupportedException(string.Format(member.Member.Name));
					}
				}
				else
				{
					var evalExpr = Evaluator.PartialEval(expr);
					if (evalExpr.NodeType == ExpressionType.Constant)
					{
						types[index] = ((ConstantExpression)evalExpr).Type;
					}
					else
					{
						types[index] = null;
					}
				}
			}

			return types;
		}

		private static Expression ToExpressionArg(object arg)
		{
			var lambda = arg as LambdaExpression;
			if (lambda != null)
			{
				return lambda.Body;
			}

			var expression = arg as Expression;
			if (expression != null)
			{
				return expression;
			}

			return Expression.Constant(arg);
		}

		private static IEnumerable<Expression> ToExpressionArgs(object[] args)
		{
			return args.Select(arg => ToExpressionArg(arg));
		}
	}
}