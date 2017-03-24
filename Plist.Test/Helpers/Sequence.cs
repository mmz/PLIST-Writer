using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Castle.Components.DictionaryAdapter;

namespace Plist.Test.Helpers
{
	class Sequence : IDisposable
	{
		internal class IndexedCall
		{
			public Expression Expression;
			public int Index;

			public IndexedCall(int index, Expression expression)
			{
				Index = index;
				Expression = expression;
			}
		}

		[ThreadStatic]
		private static Sequence _active;

		public static Sequence Active
		{
			get
			{
				if (_active == null)
					throw new SequenceNotExists();
				return _active;
			}
		}

		public static void AssertCompleted()
		{
			if (_active == null)
				throw new SequenceNotExists();
			if (_active.Complete)
				return;
			throw new SequenceNotCompletedException(_active);
		}

		public bool Complete
		{
			get { return _exprCallIndex.All(i => i.Count == 0); }
		}

		public IndexedCall LastCall { get; set; }
		private int _current;
		private int _waitFor;

		private readonly Dictionary<Expression, int> _exprIndex = new Dictionary<Expression, int>(ExpressionComparer.Default);
		private readonly List<List<int>> _exprCallIndex = new EditableList<List<int>>();

		public Sequence()
		{
			if (_active != null)
				throw new SequenceExists();
			_active = this;
			_current = 0;
			_waitFor = 0;
		}

		public void Dispose()
		{
			_active = null;
		}

		public Action Next(Expression expression)
		{
			int c = _current;
			int index;

			_current++;
			var ckvp = _exprIndex.FirstOrDefault(kvp => ExpressionComparer.Default.Equals(kvp.Key, expression));
			if (ckvp.Key != null)
			{
				index = ckvp.Value;
				_exprCallIndex[index].Add(c);
				return null;
			}

			index = _exprCallIndex.Count;
			_exprIndex[expression] = index;
			_exprCallIndex.Add(new List<int> { c });


			return () =>
			{
				var callIndex = _exprCallIndex[index];
				//Todo: decide what to do with unexpected calls of registred expresions
				//if (!callIndex.Any())
				//	return;
				if (callIndex[0] != _waitFor)
					throw new SequenceBrokenExtension(_exprIndex.FirstOrDefault(kvp => kvp.Value == index).Key, callIndex[0], _waitFor);
				LastCall = new IndexedCall(index, _exprIndex.FirstOrDefault(kvp => kvp.Value == index).Key);
				_exprCallIndex[index].Remove(_waitFor);
				_waitFor++;
			};
		}
	}

	class SequenceNotCompletedException : Exception
	{
		public SequenceNotCompletedException(Sequence sequence) : base(CreateMessage(sequence))
		{

		}

		private static string CreateMessage(Sequence sequence)
		{
			var lastCall = sequence.LastCall;
			return lastCall == null
				? "Sequence is not completed, no cals were made."
				: $@"Sequence is not completed, last step was:
{lastCall.Index} {lastCall.Expression}";
		}
	}

	internal class SequenceBrokenExtension : Exception
	{

		public SequenceBrokenExtension(Expression expr, int supposed, int called) : base($"Step '{expr}' is supposed to be called {supposed + 1}-th, but called {called + 1}-th.")
		{
		}
	}

	internal class SequenceNotExists : Exception
	{
		public SequenceNotExists() : base("Sequence not initialized.")
		{

		}
	}

	internal class SequenceExists : Exception
	{
		public SequenceExists() : base("Sequence already initialized and not disposed.")
		{

		}
	}
}