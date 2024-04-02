namespace Wraith.Collections.Generic
{
	using System.Collections.Generic;
	using System.Diagnostics;

	[DebuggerDisplay("ANode({Range,nq},{((IsBranch)?\"B\":string.Empty),nq}{((TerminiatorCount>0)?\"T\":string.Empty),nq})")]
	public abstract class ATrieNode<T> : ITrieNode<T> //IEnumerable<TrieNode<T>>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IRange<T> range;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int terminationCount;

		[DebuggerStepThrough]
		protected ATrieNode(IRange<T> range)
			: this(range, ((range != null && range.Length > 0) ? 1 : 0))
		{
		}
		[DebuggerStepThrough]
		protected ATrieNode(IRange<T> range, int terminationCount)
		{
			this.range = range;
			this.terminationCount = terminationCount;

		}


		public virtual IRange<T> Range
		{
			[DebuggerStepThrough]
			get
			{
				return this.range;
			}
			[DebuggerStepThrough]
			set
			{
				this.range = value;
			}
		}
		public virtual int TerminatorCount
		{
			[DebuggerStepThrough]
			get
			{
				return this.terminationCount;
			}
			[DebuggerStepThrough]
			set
			{
				this.terminationCount = value;
			}
		}

		public abstract IEnumerable<ITrieNode<T>> Children
		{
			get;
		}

		public virtual bool IsTerminator
		{
			[DebuggerStepThrough]
			get
			{
				return (this.terminationCount > 0);
			}
		}
		public abstract bool IsBranch
		{
			get;
		}

		public abstract void Add(ITrieNode<T> child);
		public abstract void Remove(ITrieNode<T> child);
		public void Clear()
		{
			this.ClearChildren();
			this.range = null;
			this.terminationCount = 0;
		}
		public abstract ITrieNode<T> GetFirstChild();
		public abstract int Count
		{
			get;
		}

		protected abstract void ClearChildren();

		#region IEnumerable<ITrieNode<T>> Members

		//public abstract IEnumerator<ITrieNode<T>> GetEnumerator();

		#endregion

		#region IEnumerable Members

		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//    return this.GetEnumerator();
		//}

		#endregion

	}

}
