namespace Wraith.Collections.Generic
{
	using System;
	using Wraith.Collections.Generic.Specialized;

	public class LinkTrie<T> : Trie<T>
	{
		public LinkTrie(ITrieRangeStrategy<T> rangeStrategy, TrieOptions options)
			: base(rangeStrategy, new LinkTrieNodeStrategy<T>(options))
		{
		}

		protected override ITrieNode<T> LongestSharingChild(T container, int start, int length, ITrieNode<T> node, out int sharedLength)
		{
			ITrieNode<T> retval = null;
			ITrieNode<T> bestNode = null;
			int bestLength = 0;
			int shared = 0;
			if (node.IsBranch)
			{
				//foreach (ITrieNode<T> child in node.Children)
				for (
					LinkTrieNode<T>.Node<ITrieNode<T>> current = (LinkTrieNode<T>.Node<ITrieNode<T>>)node.Children;
					current != null;
					current = (LinkTrieNode<T>.Node<ITrieNode<T>>)current.Next
				)
				{
					ITrieNode<T> child = current.Value;
					if (child.Range == null || child.Range.Length == 0)
					{
						throw new InvalidOperationException("root node cannot be a child");
					}
					shared = this.RangeStategy.Shared(container, start, length, child.Range.Container, child.Range.Start, child.Range.Length);
					if (shared > bestLength)
					{
						bestLength = shared;
						bestNode = child;
					}
					if (bestLength == length)
					{
						break;
					}
				}
			}
			sharedLength = bestLength;
			if (bestLength > 0)
			{
				retval = bestNode;
			}
			return retval;
		}
		protected override ITrieNode<T> LookupClosestNode(T container, int start, int length, out int matched)
		{
			ITrieNode<T> current = this.Root;
			ITrieNode<T> closest = null;
			LinkTrieNode<T>.Node<ITrieNode<T>> link = null;
			matched = 0;
			int currentLength = 0;

			while (
				(matched < length) &&
				current != null
				)
			{
				if (
					(
						current.Range == null
						||
						(currentLength = current.Range.Length) == 0
					)
					||
					(
						this.RangeStategy.Match(
							current.Range.Container,
							current.Range.Start,
							currentLength,
							container,
							(start + matched),
							currentLength
						)
						&&
						currentLength <= length
					)
				)
				{
					closest = current;
					matched += currentLength;
					if (matched < length)
					{
						if (current.IsBranch)
						{
							link = (LinkTrieNode<T>.Node<ITrieNode<T>>)current.Children;
							if (link != null)
							{
								current = link.Value;
							}
							else
							{
								current = null;
								link = null;
							}
						}
						else
						{
							link = null;
							current = null;
						}
					}
				}
				else
				{
					if (link != null)
					{
						if ((link = link.Next) != null)
						{
							current = link.Value;
						}
						else
						{
							current = null;
						}
					}
					else
					{
						current = null;
					}
				}
			}

			return closest;
		}
		protected override StackNode<ITrieNode<T>> LookupPath(T container, int start, int length, ITrieNode<T> root, out int matchedLength)
		{
			ITrieNode<T> current = root;
			ITrieNode<T> closest = null;
			LinkTrieNode<T>.Node<ITrieNode<T>> link = null;
			StackNode<ITrieNode<T>> stack = null;
			int matched = 0;
			int currentLength = -1;

			while (
				(matched < length) &&
				current != null
				)
			{
				currentLength = 0;
				if (
					(
						current.Range == null
						||
						(currentLength = current.Range.Length) == 0
					)
					||
					(
						this.RangeStategy.Match(
							current.Range.Container,
							current.Range.Start,
							currentLength,
							container,
							(start + matched),
							currentLength
						)
						&&
						currentLength <= length
					)
				)
				{
					StackNode<ITrieNode<T>>.Push(ref stack, current);
					matched += currentLength;
					if (matched < (length))
					{
						if (current.IsBranch)
						{
							link = (LinkTrieNode<T>.Node<ITrieNode<T>>)current.Children;
							if (link != null)
							{
								current = link.Value;
							}
							else
							{
								current = null;
								link = null;
							}
						}
						else
						{
							link = null;
							current = null;
						}
					}
					else
					{
						closest = current;
					}
				}
				else
				{
					if (link != null)
					{
						if ((link = link.Next) != null)
						{
							current = link.Value;
						}
						else
						{
							current = null;
						}
					}
					else
					{
						current = null;
					}
				}
			}

			matchedLength = matched;
			return stack;
		}
	}
}
