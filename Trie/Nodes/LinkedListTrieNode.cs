namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;

	[DebuggerDisplay("LLNode({Range,nq},{((IsBranch)?\"B\":string.Empty),nq}{((TerminiatorCount>0)?\"T\":string.Empty),nq})")]
	public sealed class LinkedListTrieNode<T> : ACollectionTrieNode<T>
	{
		//[DebuggerStepThrough]
		internal LinkedListTrieNode(IRange<T> range,IEnumerable<ITrieNode<T>> children,int terminationCount)
			: base(range,children,terminationCount)
		{
		}

		[DebuggerStepThrough]
		internal LinkedListTrieNode(IRange<T> range)
			: base(range)
		{

		}

		public override ITrieNode<T> GetFirstChild()
		{
			LinkedList<ITrieNode<T>> children = this.Children as LinkedList<ITrieNode<T>>;
			if (children==null)
			{
				throw new InvalidOperationException("node has no children");
			}
			if (children.Count==0)
			{
				throw new InvalidOperationException("node has no children");
			}
			return children.First.Value;
		}

		protected override ICollection<ITrieNode<T>> CreateCollection(IEnumerable<ITrieNode<T>> enumerable)
		{
			LinkedList<ITrieNode<T>> linkCollection = enumerable as LinkedList<ITrieNode<T>>;
			if (linkCollection==null)
			{
				if (enumerable!=null)
				{
					linkCollection=new LinkedList<ITrieNode<T>>(enumerable);
					if (linkCollection.Count==0)
					{
						linkCollection=null;
					}
				}

			}
			return linkCollection;
		}
		protected override ICollection<ITrieNode<T>> CreateCollection()
		{
			return new LinkedList<ITrieNode<T>>();
		}
	}


	public class LinkedListTrieNodeStrategy<T> : ITrieNodeStrategy<T>
	{
		private readonly TrieOptions options;

		public LinkedListTrieNodeStrategy(TrieOptions options)
		{
			this.options=options;
		}

		#region ITrieNodeStrategy<T> Members

		public ITrieNode<T> Create(IRange<T> range)
		{
			return new LinkedListTrieNode<T>(range,null,((range!=null && range.Length>0)?1:0));
		}
		public ITrieNode<T> Create(IRange<T> range,IEnumerable<ITrieNode<T>> children,int terminationCount)
		{
			return new LinkedListTrieNode<T>(range,children,terminationCount);
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
