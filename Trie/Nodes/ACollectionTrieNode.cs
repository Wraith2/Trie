namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	[DebuggerStepThrough]
	[DebuggerDisplay("ACNode({Range,nq},{((IsBranch)?\"B\":string.Empty),nq}{((TerminiatorCount>0)?\"T\":string.Empty),nq})")]
	public abstract class ACollectionTrieNode<T> : ATrieNode<T>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ICollection<ITrieNode<T>> children;

		protected ACollectionTrieNode(IRange<T> range, IEnumerable<ITrieNode<T>> children, int terminationCount)
			: base(range, terminationCount)
		{
			this.children = this.CreateCollection(children);
		}
		protected ACollectionTrieNode(IRange<T> range)
			: base(range)
		{
		}

		public override IEnumerable<ITrieNode<T>> Children
		{
			get
			{
				return this.children;
			}

		}

		public override bool IsBranch
		{
			get
			{
				return (this.children != null && this.children.Count > 0);
			}
		}

		public override void Add(ITrieNode<T> child)
		{
			if (child == null)
			{
				throw new ArgumentNullException("child", "child is null");
			}
			if (this.children == null)
			{
				this.children = this.CreateCollection();
			}
			this.children.Add(child);
		}
		public override void Remove(ITrieNode<T> child)
		{
			if (child == null)
			{
				throw new ArgumentNullException("child", "child is null");
			}
			if (this.children != null)
			{
				this.children.Remove(child);
				if (this.children.Count == 0)
				{
					this.children = null;
				}
			}
		}

		public override int Count
		{
			get
			{
				int retval = 0;
				if (this.children != null)
				{
					retval = this.children.Count;
				}
				return retval;
			}
		}

		protected abstract ICollection<ITrieNode<T>> CreateCollection();
		protected abstract ICollection<ITrieNode<T>> CreateCollection(IEnumerable<ITrieNode<T>> enumerable);

		protected override void ClearChildren()
		{
			this.children = null;
		}

		#region IEnumerable<ITrieNode<T>> Members

		//public override IEnumerator<ITrieNode<T>> GetEnumerator()
		//{
		//    return (this.children!=null)?this.children.GetEnumerator():null;
		//}

		#endregion

	}
}
