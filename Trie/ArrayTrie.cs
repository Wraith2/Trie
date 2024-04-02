namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;
	using System.Collections;
	using Wraith.Collections.Generic.Specialized;

	public class ArrayTrie<T> : Trie<T>
	{
		public ArrayTrie(ITrieRangeStrategy<T> rangeStrategy,TrieOptions options)
			: base(rangeStrategy,new ArrayTrieNodeStrategy<T>(options))
		{
		}

		public override ITrieNodeStrategy<T> NodeStrategy
		{
			get
			{
				return base.NodeStrategy;
			}
			protected set
			{
				throw new NotSupportedException("cannot set the NodeStrategy of an ArrayTrie");
			}
		}

		protected override ITrieNode<T> LookupClosestNode(T container,int start,int length,out int matched)
		{
			ITrieNode<T> current = this.Root;
			//IEnumerator enumerator=null;
			ITrieNode<T> closest=null;

			IList<ITrieNode<T>> list=null;
			//int arrayLength = -1;
			int listIndex=-1;

			matched = 0;
			int currentLength=0;

			while (
				(matched<length) &&
				current!=null
				)
			{
				if (
					(
						current.Range==null 
						||
						(currentLength=current.Range.Length)==0
					)
					||
					(
						this.RangeStategy.Match(
							current.Range.Container,
							current.Range.Start,
							currentLength,
							container,
							(start+matched),
							currentLength
						) 
						&&
						currentLength<=length
					)
				)
				{
					closest=current;
					matched+=currentLength;
					if (matched<length)
					{
						if (current.IsBranch)
						{
							list=(IList<ITrieNode<T>>)current.Children;
							if (list!=null && list.Count>0)
							{
								listIndex=0;
								current=list[listIndex];
							}
							else
							{
								list=null;
								listIndex=-1;
								current=null;
							}
						}
						else
						{
							list=null;
							listIndex=-1;
							current=null;
						}
					}
				}
				else
				{
					if (list!=null && listIndex<(list.Count-1))
					{
						listIndex+=1;
						current=list[listIndex];
					}
					else
					{
						list=null;
						listIndex=-1;
						current=null;
					}
				}
			}

			return closest;
		}
		protected override StackNode<ITrieNode<T>> LookupPath(T container,int start,int length,ITrieNode<T> root,out int matchedLength)
		{
			ITrieNode<T> current = root;
			ITrieNode<T> closest=null;
			StackNode<ITrieNode<T>> stack = null;
			int matched = 0;
			int currentLength=-1;

			IList<ITrieNode<T>> list=null;
			int listIndex=-1;

			while ((matched<length) && current!=null)
			{
				currentLength=0;
				if (
					(
						current.Range==null 
						||
						(currentLength=current.Range.Length)==0
					)
					||
			        (
						this.RangeStategy.Match(
							current.Range.Container,
							current.Range.Start,
							currentLength,
							container,
							(start+matched),
							currentLength
						) 
			            &&
			            currentLength<=length
					)
				)
				{
					StackNode<ITrieNode<T>>.Push(ref stack,current);
					matched+=currentLength;
					if (matched<(length))
					{
						if (current.IsBranch)
						{
							list=(IList<ITrieNode<T>>)current.Children;
							if (list!=null && list.Count>0)
							{
								listIndex=0;
								current=list[listIndex];
							}
							else
							{
								list=null;
								listIndex=-1;
								current=null;
							}
						}
						else
						{
							list=null;
							listIndex=-1;
							current=null;
						}
					}
					else
					{
						closest=current;
					}
				}
				else
				{
					if (list!=null && listIndex<(list.Count-1))
					{
						listIndex+=1;
						current=list[listIndex];
					}
					else
					{
						list=null;
						listIndex=-1;
						current=null;
					}
				}
			}

			matchedLength=matched;
			return stack;
		}
		protected override ITrieNode<T> LongestSharingChild(T container,int start,int length,ITrieNode<T> node,out int sharedLength)
		{
			ITrieNode<T> retval = null;
			ITrieNode<T> bestNode = null;
			int bestLength=0;
			int shared = 0;
			if (node.IsBranch)
			{
				IList<ITrieNode<T>> children = (IList<ITrieNode<T>>)node.Children;
				for (int index=0;index<children.Count;index++)
				{
					ITrieNode<T> child = children[index];
					if (child.Range==null || child.Range.Length==0)
					{
						throw new InvalidOperationException("root node cannot be a child");
					}
					shared=this.RangeStategy.Shared(container,start,length,child.Range.Container,child.Range.Start,child.Range.Length);
					if (shared>bestLength)
					{
						bestLength=shared;
						bestNode=child;
					}
					if (bestLength==length)
					{
						break;
					}
				}
			}
			sharedLength=bestLength;
			if (bestLength>0)
			{
				retval=bestNode;
			}
			return retval;
		}
	}
}
