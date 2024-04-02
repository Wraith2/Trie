namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Diagnostics;

	[DebuggerDisplay("LNode({Range,nq},{((IsBranch)?\"B\":string.Empty),nq}{((TerminiatorCount>0)?\"T\":string.Empty),nq})")]
	public class LinkTrieNode<T> : ATrieNode<T>
	{
		public sealed class Node<U> : IEnumerable<U>
		{
			public U Value;
			public Node<U> Next;

			public Node()
				: this(null,default(U))
			{
			}
			public Node(U value)
				: this(null,value)
			{
			}
			public Node(Node<U> next)
				: this(next,default(U))
			{

			}
			public Node(Node<U> next,U value)
			{
				this.Value=value;
				this.Next=next;
			}

			public override string ToString()
			{
				return string.Format("[{0}]",this.Value);
			}


			#region IEnumerable<U> Members

			public IEnumerator<U> GetEnumerator()
			{
				return new NodeEnumerator<U>(this);
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion
		}

		public sealed class NodeEnumerator<V> : IEnumerator<V>
		{
			private Node<V> first;
			private Node<V> current;

			public NodeEnumerator(Node<V> first)
			{
				this.first=first;
			}

			#region IEnumerator<T> Members

			public V Current
			{
				get
				{
					return this.current.Value;
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				this.first=null;
				this.current=null;
			}

			#endregion

			#region IEnumerator Members

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				if (this.first==null)
				{
					throw new ObjectDisposedException("NodeEnumerator");
				}
				bool moved=false;
				if (moved=(this.current==null))
				{
					this.current=first;
				}
				if (!moved && this.current!=null)
				{
					this.current=this.current.Next;
					moved=(this.current!=null);
				}
				return moved;
			}

			public void Reset()
			{
				this.current=null;
			}

			#endregion
		}

		private Node<ITrieNode<T>> first;
		private Node<ITrieNode<T>> last;
		private int count;

		public LinkTrieNode(IRange<T> range,IEnumerable<ITrieNode<T>> children,int terminationCount)
			: base(range,terminationCount)
		{
			this.CreateNodes(children);
		}

		public LinkTrieNode(IRange<T> range)
			: base(range)
		{
		}

		public override IEnumerable<ITrieNode<T>> Children
		{
			get
			{
				return this.first;
			}
		}
		public override bool IsBranch
		{
			get
			{
				return (this.count>0);
			}
		}
		public override int Count
		{
			get
			{
				return this.count;
			}
		}

		public override void Add(ITrieNode<T> child)
		{
			Node<ITrieNode<T>> newNode = new Node<ITrieNode<T>>(null,child);
			switch (this.count)
			{
				case 0:
					this.first=newNode;
					this.count=1;
					break;
				case 1:
					this.first.Next=newNode;
					this.last=newNode;
					this.count=2;
					break;
				default:
					this.last.Next=newNode;
					this.last=newNode;
					this.count+=1;
					break;
			}
		}
		public override void Remove(ITrieNode<T> child)
		{
			switch (this.count)
			{
				case 0:
					break;
				case 1:
					if (this.first.Value==child)
					{
						this.first=null;
						this.count=0;
					}
					break;
				default:
					Node<ITrieNode<T>> current=this.first;
					Node<ITrieNode<T>> previous=null;
					for (
						;
						current!=null;
						current=current.Next
					)
					{
						if (current.Value==child)
						{
							break;
						}
						previous=current;
					}
					if (current!=null)
					{
						if (previous!=null)
						{
							previous.Next=current.Next;
						}
						else
						{
							this.first=current.Next;
						}

						if (this.last==current)
						{
							if (previous==this.first)
							{
								this.last=null;
							}
							else
							{
								this.last=previous;
							}
						}
						current.Next=null;
						current.Value=null;
						this.count-=1;
					}
					break;
			}
		}
		public override ITrieNode<T> GetFirstChild()
		{
			if (this.first==null)
			{
				throw new InvalidOperationException("no first child present");
			}
			return this.first.Value;
		}

		protected override void ClearChildren()
		{
			this.count=0;
			this.first=null;
			this.last=null;
		}

		private void CreateNodes(IEnumerable<ITrieNode<T>> children)
		{
			if (children!=null)
			{
				IList<ITrieNode<T>> list = children as IList<ITrieNode<T>>;
				if (list!=null)
				{
					Node<ITrieNode<T>> next=new Node<ITrieNode<T>>(null,list[list.Count-1]);
					this.count=list.Count;
					switch (this.count)
					{
						case 0:
							break;
						case 1:
							this.first=next;
							break;
						default:
							this.last=next;
							for (
								int index=(list.Count-2);
								index>=0;
								index--
								)
							{
								next=new Node<ITrieNode<T>>(next,list[index]);
							}
							this.first=next;
							break;
					}
				}
				else
				{
					Node<ITrieNode<T>> current=null;
					Node<ITrieNode<T>> previous=null;
					foreach (ITrieNode<T> child in children)
					{
						this.count+=1;
						current=new Node<ITrieNode<T>>(null,child);
						if (previous==null)
						{
							this.first=current;
						}
						else
						{
							previous.Next=current;
						}
						previous=current;
					}
					if (this.first!=current)
					{
						this.last=current;
					}
				}
			}
		}
	}

	public class LinkTrieNodeStrategy<T> : ITrieNodeStrategy<T>
	{
		private readonly TrieOptions options;

		public LinkTrieNodeStrategy(TrieOptions options)
		{
			this.options=options;
		}

		#region ITrieNodeStrategy<T> Members

		public ITrieNode<T> Create(IRange<T> range)
		{
			return new LinkTrieNode<T>(range,null,((range!=null && range.Length>0)?1:0));
		}
		public ITrieNode<T> Create(IRange<T> range,IEnumerable<ITrieNode<T>> children,int terminationCount)
		{
			return new LinkTrieNode<T>(range,children,terminationCount);
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
