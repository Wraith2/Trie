namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Wraith.Collections.Generic.Specialized;


	public enum TrieOptions
	{
		None = 0,
		AllowDuplicateEntries = 0x1
	}

	public class Trie<T>
	{
		private ITrieNode<T> root;
		private ITrieRangeStrategy<T> rangeStategy;
		private ITrieNodeStrategy<T> nodeStrategy;
		internal long version;

		[DebuggerStepThrough]
		public Trie(ITrieRangeStrategy<T> rangeStrategy, ITrieNodeStrategy<T> nodeStrategy)
		{
			this.rangeStategy = rangeStrategy;
			this.nodeStrategy = nodeStrategy;
			//this.root=new TrieNode<T>(null);
			this.root = this.nodeStrategy.Create(null);
			//this.options=TrieOptions.AllowDuplicateEntries;
		}

		public bool Lookup(T container, int start, int length)
		{
			//return this.RecurseLookup(container,start,length,0);
			//return this.SerialLookup(container,start,length);
			ITrieNode<T> closest = null;
			int matched = 0;
			closest = this.LookupClosestNode(container, start, length, out matched);
			return (
					matched == length &&
					closest != null &&
					closest.IsTerminator
				);
		}

		public void Insert(IRange<T> range)
		{
			if (range == null)
			{
				throw new ArgumentNullException("range", "range is null");
			}
			if (range.Length <= 0)
			{
				throw new ArgumentException("range.Length is invalid", "range");
			}
			this.Insert(range.Container, range.Start, range.Length);
		}
		public void Insert(T container, int start, int length)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container", "container is null");
			}
			if (length <= 0)
			{
				throw new ArgumentException("length is invalid", "length");
			}
			int matchedLength = -1;
			ITrieNode<T> closest = this.LookupClosestNode(container, start, length, out matchedLength);

			if (matchedLength == length)
			{
				//closest.TerminiatorCount+=1;
				int count = closest.TerminatorCount;
				if (
					count == 0
					||
					//(this.options&TrieOptions.AllowDuplicateEntries)==TrieOptions.AllowDuplicateEntries
					(this.nodeStrategy.Options & TrieOptions.AllowDuplicateEntries) == TrieOptions.AllowDuplicateEntries

				)
				{
					count += 1;
					closest.TerminatorCount = count;
					unchecked { this.version += 1; }
				}
			}
			else
			{
				int partialLength = 0;
				ITrieNode<T> partial = this.LongestSharingChild(container, (start + matchedLength), (length - matchedLength), closest, out partialLength);
				if (partialLength == 0)
				{
					IRange<T> childRange = this.RangeStategy.Create(
						container,
						(start + matchedLength),
						(length - matchedLength)
					);
					//ITrieNode<T> childNode=new TrieNode<T>(childRange);
					ITrieNode<T> childNode = this.nodeStrategy.Create(childRange);
					closest.Add(childNode);
					unchecked { this.version += 1; }
				}
				else
				{
					ITrieNode<T> parentNode = null;
					ITrieNode<T> childNode = null;

					closest.Remove(partial);

					IRange<T> parentRange = null;
					IRange<T> childRange = null;
					this.RangeStategy.Split(
						partial.Range,
						partialLength,
						out parentRange,
						out childRange
					);
					//childNode=new TrieNode<T>(childRange,partial.Children,partial.TerminiatorCount);
					childNode = this.nodeStrategy.Create(childRange, partial.Children, partial.TerminatorCount);
					partial.Clear();
					parentNode = partial;
					partial = null;
					parentNode.Range = parentRange;
					parentNode.TerminatorCount = 0;
					parentNode.Add(childNode);

					closest.Add(parentNode);

					if ((partialLength + matchedLength) == length)
					{
						//parentNode.TerminiatorCount+=1;
						int count = parentNode.TerminatorCount;
						if (
							count == 0
							||
							//(this.options&TrieOptions.AllowDuplicateEntries)==TrieOptions.AllowDuplicateEntries
							(this.nodeStrategy.Options & TrieOptions.AllowDuplicateEntries) == TrieOptions.AllowDuplicateEntries
						)
						{
							count += 1;
							parentNode.TerminatorCount = count;
							unchecked { this.version += 1; }
						}
					}
					else
					{
						childRange = this.RangeStategy.Create(
							container,
							start + (matchedLength + partialLength),
							length - (matchedLength + partialLength)
						);
						//childNode=new TrieNode<T>(childRange);
						childNode = this.nodeStrategy.Create(childRange);
						parentNode.Add(childNode);
						unchecked { this.version += 1; }
					}
					//unchecked { this.version+=1; }
				}
			}
		}

		public void Delete(IRange<T> range)
		{
			if (range == null)
			{
				throw new ArgumentNullException("range", "range is null");
			}
			if (range.Length <= 0)
			{
				throw new ArgumentException("range.Length is invalid", "range");
			}
			this.Delete(range.Container, range.Start, range.Length);
		}
		public void Delete(T container, int start, int length)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container", "container is null");
			}
			if (length <= 0)
			{
				throw new ArgumentException("length is invalid", "length");
			}

			int consumed = -1;
			//Stack<ITrieNode<T>> path = this.LookupPath(container,start,length,this.root,out consumed);
			StackNode<ITrieNode<T>> path = this.LookupPath(container, start, length, this.root, out consumed);
			if (consumed == length)
			{
				//ITrieNode<T> child=path.Pop();
				//ITrieNode<T> parent=path.Pop();

				ITrieNode<T> child = StackNode<ITrieNode<T>>.Pop(ref path);
				ITrieNode<T> parent = StackNode<ITrieNode<T>>.Pop(ref path);

				//if (!child.IsTerminator)
				//{
				//    throw new InvalidOperationException("child is not terminator");
				//}
				if (child.IsTerminator)
				{
					child.TerminatorCount -= 1;

					if (!child.IsTerminator)
					{
						if (child.IsBranch)
						{
							if (child.Count == 1)
							{
								ITrieNode<T> onlyChild = child.GetFirstChild();

								IRange<T> mergedRange = this.RangeStategy.Merge(child.Range, onlyChild.Range);//this.MergeRanges(child.Range,onlyChild.Range);
								if (mergedRange != null)
								{
									ITrieNode<T> mergedNode = this.nodeStrategy.Create(
										mergedRange,
										onlyChild.Children,
										onlyChild.TerminatorCount
									);
									child.Remove(onlyChild);
									parent.Remove(child);
									parent.Add(mergedNode);
								}
							}
						}
						else
						{
							parent.Remove(child);
							if (
								!parent.IsTerminator &&
								parent.Count == 1
							)
							{
								ITrieNode<T> containerNode = (path != null) ? path.Peek() : this.root;
								ITrieNode<T> onlyChild = containerNode.GetFirstChild();
								IRange<T> mergedRange = this.RangeStategy.Merge(parent.Range, onlyChild.Range);
								if (mergedRange != null)
								{
									ITrieNode<T> mergedNode = this.nodeStrategy.Create(
										mergedRange,
										onlyChild.Children,
										onlyChild.TerminatorCount
									);
									parent.Remove(onlyChild);
									containerNode.Remove(parent);
									containerNode.Add(mergedNode);
								}

							}
						}
					}
					unchecked { this.version += 1; }
				}
			}
		}


		public virtual ITrieNode<T> Root
		{
			get
			{
				return this.root;
			}
			set
			{
				this.root = value;
			}
		}
		public virtual ITrieRangeStrategy<T> RangeStategy
		{
			get
			{
				return this.rangeStategy;
			}
			protected set
			{
				this.rangeStategy = value;
			}
		}
		public virtual ITrieNodeStrategy<T> NodeStrategy
		{
			get
			{
				return this.nodeStrategy;
			}
			protected set
			{
				this.nodeStrategy = value;
			}
		}

		protected virtual ITrieNode<T> LookupClosestNode(T container, int start, int length, out int matched)
		{
			ITrieNode<T> current = this.Root;
			IEnumerator<ITrieNode<T>> enumerator = null;
			ITrieNode<T> closest = null;

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
							enumerator = current.Children.GetEnumerator();
							if (enumerator != null && enumerator.MoveNext())
							{
								current = enumerator.Current as ITrieNode<T>;
							}
							else
							{
								current = null;
							}
						}
						else
						{
							enumerator = null;
							current = null;
						}
					}
				}
				else
				{
					if (enumerator != null)
					{
						if (enumerator.MoveNext())
						{
							current = enumerator.Current as ITrieNode<T>;
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
		protected virtual ITrieNode<T> LongestSharingChild(T container, int start, int length, ITrieNode<T> node, out int sharedLength)
		{
			ITrieNode<T> retval = null;
			ITrieNode<T> bestNode = null;
			int bestLength = 0;
			int shared = 0;
			if (node.IsBranch)
			{
				foreach (ITrieNode<T> child in node.Children)
				{
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
		protected virtual StackNode<ITrieNode<T>> LookupPath(T container, int start, int length, ITrieNode<T> root, out int matchedLength)
		{
			ITrieNode<T> current = root;
			ITrieNode<T> closest = null;
			IEnumerator<ITrieNode<T>> enumerator = null;
			//Stack<ITrieNode<T>> path=new Stack<ITrieNode<T>>();
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
					//path.Push(current);
					StackNode<ITrieNode<T>>.Push(ref stack, current);
					matched += currentLength;
					if (matched < (length))
					{
						if (current.IsBranch)
						{
							if (enumerator != null)
							{
								enumerator.Dispose();
								enumerator = null;
							}
							enumerator = current.Children.GetEnumerator();
							if (enumerator != null && enumerator.MoveNext())
							{
								current = enumerator.Current as ITrieNode<T>;
							}
							else
							{
								current = null;
							}
						}
						else
						{
							enumerator = null;
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
					if (enumerator != null)
					{
						if (enumerator.MoveNext())
						{
							current = enumerator.Current as ITrieNode<T>;
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
			if (enumerator != null)
			{
				enumerator.Dispose();
			}

			matchedLength = matched;
			//return path;
			return stack;
		}


		protected static void Traverse(Trie<T> trie, Action<ITrieNode<T>> action)
		{
			if (trie != null)
			{
				if (action == null)
				{
					throw new ArgumentNullException("action", "action is null");
				}

				StackNode<IPair<ITrieNode<T>, IEnumerator<ITrieNode<T>>>> stack = null;
				ITrieNode<T> current = trie.Root;
				IEnumerator<ITrieNode<T>> enumerator = null;
				while (current != null)
				{
					action(current);

					if (current.IsBranch)
					{
						enumerator = current.Children.GetEnumerator();
						if (!enumerator.MoveNext())
						{
							throw new InvalidOperationException("branch with no children");
						}
						StackNode<IPair<ITrieNode<T>, IEnumerator<ITrieNode<T>>>>.Push(
							ref stack,
							new Pair<ITrieNode<T>, IEnumerator<ITrieNode<T>>>(
								current,
								enumerator
							)
						);
						current = enumerator.Current;
					}
					else if (stack != null)
					{
						enumerator = stack.Value.Second;
						if (enumerator.MoveNext())
						{
							current = enumerator.Current;
						}
						else
						{
							StackNode<IPair<ITrieNode<T>, IEnumerator<ITrieNode<T>>>>.Pop(ref stack);
						}
					}
					else
					{
						current = null;
					}
				}
			}
		}
	}

}
