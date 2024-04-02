namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Collections;

	public abstract class ATrieEnumerator<T> : IEnumerator<T>
	{
		[DebuggerStepThrough]
		public sealed class TrieEnumeratorNode
		{
			public readonly ITrieNode<T> Node;
			public readonly IEnumerator<ITrieNode<T>> Enumerator;

			public TrieEnumeratorNode(ITrieNode<T> Node,IEnumerator<ITrieNode<T>> Enumerator)
			{
				this.Node=Node;
				this.Enumerator=Enumerator;
			}
		}

		private Trie<T> trie;
		private List<TrieEnumeratorNode> path;
		private ITrieNode<T> current;
		private bool start;
		private long verison;

		[DebuggerStepThrough]
		protected ATrieEnumerator(Trie<T> trie)
		{
			this.trie=trie;
			this.verison=this.trie.version;
			this.path=new List<TrieEnumeratorNode>();
			this.Reset();
		}

		#region IEnumerator<T> Members

		public virtual T Current
		{
			[DebuggerStepThrough]
			get
			{
				this.CheckVersion();
				return this.ValueFromPath(this.path);
			}
		}

		public virtual bool MoveNext()
		{
			this.CheckVersion();

			ITrieNode<T> current=this.current;
			if (this.start)
			{
				current=this.trie.Root;
				this.start=false;
			}
			ITrieNode<T> start = this.current;
			IEnumerator<ITrieNode<T>> enumerator=null;
			while (
				start==current
				||
				(
					current!=null &&
					!current.IsTerminator
				)
			)
			{
				if (current.IsBranch)
				{
					enumerator=current.Children.GetEnumerator();
					if (!enumerator.MoveNext())
					{
						throw new InvalidOperationException("branch with no children");
					}
					this.path.Add(new TrieEnumeratorNode(current,enumerator));
					current=enumerator.Current;
				}
				else if (this.path.Count>0)
				{
					enumerator=this.path[this.path.Count-1].Enumerator;
					if (enumerator.MoveNext())
					{
						current=enumerator.Current;
					}
					else
					{
						this.path.RemoveAt(this.path.Count-1);
					}
				}
				else
				{
					current=null;
				}
			}
			this.current=current;

			return (this.current!=null);

		}
		[DebuggerStepThrough]
		public virtual void Reset()
		{
			if (this.path.Count>0)
			{
				this.path.Clear();
			}
			this.current=null;
			this.start=true;
		}

		#endregion

		#region IEnumerator Members

		object IEnumerator.Current
		{
			[DebuggerStepThrough]
			get
			{
				return this.Current;
			}
		}

		#endregion

		#region IDisposable Members
		[DebuggerStepThrough]
		public virtual void Dispose()
		{
			this.verison=0;
			if (this.trie!=null)
			{
				this.trie=null;
			}
			if (this.path!=null)
			{
				this.path.Clear();
				this.path=null;
			}
			this.current=null;
		}

		#endregion


		protected ITrieNode<T> CurrentNode
		{
			get
			{
				return this.current;
			}
		}

		[DebuggerStepThrough]
		protected abstract T ValueFromPath(List<TrieEnumeratorNode> path);

		[DebuggerStepThrough]
		private void CheckVersion()
		{
			if (this.verison!=this.trie.version)
			{
				throw new InvalidOperationException("trie has changed");
			}
		}
	}
}
