namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	[DebuggerDisplay("ALNode({Range,nq},{((IsBranch)?\"B\":string.Empty),nq}{((TerminiatorCount>0)?\"T\":string.Empty),nq})")]
	public class ArrayTrieNode<T> : ACollectionTrieNode<T>
	{
		//[DebuggerStepThrough]
		internal ArrayTrieNode(IRange<T> range, IEnumerable<ITrieNode<T>> children, int terminationCount)
			: base(range, children, terminationCount)
		{
		}
		//[Obsolete]
		[DebuggerStepThrough]
		public ArrayTrieNode(IRange<T> range)
			: base(range)
		{
		}

		public override ITrieNode<T> GetFirstChild()
		{
			IList<ITrieNode<T>> children = this.Children as IList<ITrieNode<T>>;
			if (children == null)
			{
				throw new InvalidOperationException("node has no children");
			}
			if (children.Count == 0)
			{
				throw new InvalidOperationException("node has no children");
			}
			return children[0];
		}

		protected override ICollection<ITrieNode<T>> CreateCollection(IEnumerable<ITrieNode<T>> enumerable)
		{
			IList<ITrieNode<T>> list = enumerable as IList<ITrieNode<T>>;
			if (list == null)
			{
				if (enumerable != null)
				{
					list = new List<ITrieNode<T>>(enumerable);
					if (list.Count == 0)
					{
						list = null;
					}
				}
			}
			return list;
		}
		protected override ICollection<ITrieNode<T>> CreateCollection()
		{
			return new List<ITrieNode<T>>();
		}
	}



	public class ArrayTrieNodeStrategy<T> : ITrieNodeStrategy<T>
	{
		private readonly TrieOptions options;

		public ArrayTrieNodeStrategy(TrieOptions options)
		{
			this.options = options;
		}

		#region ITrieNodeStrategy<T> Members

		public ITrieNode<T> Create(IRange<T> range)
		{
			return new ArrayTrieNode<T>(range, null, ((range != null && range.Length > 0) ? 1 : 0));
		}

		public ITrieNode<T> Create(IRange<T> range, IEnumerable<ITrieNode<T>> children, int terminationCount)
		{
			return new ArrayTrieNode<T>(range, children, terminationCount);
		}

		public TrieOptions Options
		{
			get
			{
				return this.options;
			}
		}

		#endregion
	}
}
