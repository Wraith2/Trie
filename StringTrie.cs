namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;
	using Wraith.Collections;


	public class StringRangeStrategy : ITrieRangeStrategy<string>
	{
		#region ITrieRangeStrategy<string> Members

		public IRange<string> Create(string container, int start, int length)
		{
			return new StringRange(container, start, length, false);
		}
		public IRange<string> Merge(IRange<string> lhs, IRange<string> rhs)
		{
			if (lhs == null)
			{
				return rhs;
			}
			if (rhs == null)
			{
				return lhs;
			}
			if (lhs.Length == 0)
			{
				return rhs;
			}
			if (rhs.Length == 0)
			{
				return lhs;
			}

			IRange<string> merged = null;

			if (
				merged == null &&
				lhs.Container.Length >=
				(
					lhs.Start +
					lhs.Length +
					rhs.Length
				)
			)
			{
				// the lhs range.Container may include rhs range
				if (
					Match(
						lhs.Container,
						lhs.Start + lhs.Length,
						rhs.Length,
						rhs.Container,
						rhs.Start,
						rhs.Length
					)
				)
				{
					merged = Create(
							lhs.Container,
							lhs.Start,
							(lhs.Length + rhs.Length)
						);
				}
			}

			if (
				merged == null &&
				rhs.Start >= lhs.Length
				)
			{
				// the rhs range.Container may include lhs range
				if (
					Match(
						rhs.Container,
						rhs.Start - lhs.Length,
						lhs.Length,
						lhs.Container,
						lhs.Start,
						lhs.Length
					)
				)
				{
					merged = Create(
						rhs.Container,
						rhs.Start - lhs.Length,
						(lhs.Length + rhs.Length)
					);
				}
			}

			if (merged == null)
			{
				// the lhs range.Container is not followed by the rhs range
				// and the rhs range.Container is not preceeded by the lhs
				// range

				// create a new Container which contains only the required portions
				// from both lhs and rhs that are needed. this is bad and 
				// will cause fragmentation and increased memory usage.

				string newItem = string.Concat(
					lhs.Container.Substring(lhs.Start, lhs.Length),
					rhs.Container.Substring(rhs.Start, rhs.Length)
				);
				merged = Create(newItem, 0, newItem.Length);
			}

			return merged;
		}
		public void Split(IRange<string> range, int length, out IRange<string> lhs, out IRange<string> rhs)
		{
			if (range == null)
			{
				throw new ArgumentException("range is null", "range");
			}
			if (range.Length == 0)
			{
				throw new ArgumentException("range is empty", "range");
			}
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "length must be greater than zero");
			}
			if (length > range.Length)
			{
				throw new ArgumentOutOfRangeException("length", length, "length must be less than node.Range.Length");
			}

			lhs = Create(
				range.Container,
				range.Start,
				length
			);

			rhs = Create(
				range.Container,
				range.Start + length,
				range.Length - length
			);
		}
		public bool Match(string lhs, int lhsStart, int lhsLength, string rhs, int rhsStart, int rhsLength)
		{
			bool retval = false;
			int maxMatchLength = Math.Max(lhsLength, rhsLength);

			if (maxMatchLength > 0)
			{
				retval = (
					string.Compare(
							lhs, lhsStart,
							rhs, rhsStart,
							maxMatchLength,
							false
						)
					== 0
				);
			}
			return retval;
		}
		public int Shared(IRange<string> lhs, int lhsOffset, IRange<string> rhs, int rhsOffset)
		{
			int retval = 0;
			int lhsLength = -1;
			int rhsLength = -1;
			int lhsStart = -1;
			int rhsStart = -1;
			if (
				lhs != null &&
				rhs != null &&
				(lhsLength = (lhs.Length - lhsOffset)) > 0 &&
				(rhsLength = (rhs.Length - rhsOffset)) > 0
				)
			{
				rhsStart = rhs.Start + rhsOffset;
				lhsStart = lhs.Start + lhsOffset;
				int maxMatchLength = Math.Min(lhsLength, rhsLength);
				if (maxMatchLength > 0)
				{
					for (
						;
						retval < maxMatchLength;
						retval++
						)
					{
						if (
							string.Compare(
								lhs.Container,
								(lhsStart + retval),
								rhs.Container,
								(rhsStart + retval),
								1,
								false
							) != 0
						)
						{
							break;
						}
					}
				}
			}
			return retval;
		}
		public int Shared(string lhs, int lhsStart, int lhsLength, string rhs, int rhsStart, int rhsLength)
		{
			int retval = 0;
			if (
				lhs != null &&
				rhs != null &&
				lhsLength > 0 &&
				rhsLength > 0
				)
			{
				int maxMatchLength = Math.Min(lhsLength, rhsLength);
				if (maxMatchLength > 0)
				{
					for (; retval < maxMatchLength; retval++)
					{
						if (
							string.Compare(
								lhs,
								(lhsStart + retval),
								rhs,
								(rhsStart + retval),
								1,
								false
							) != 0
						)
						{
							break;
						}
					}
				}
			}
			return retval;
		}
		#endregion
	}

	public class StringTrie : Trie<string>, IEnumerable<string>
	{
		[DebuggerStepThrough]
		public StringTrie()
			: base(
				new StringRangeStrategy(),
				//new ArrayTrieNodeStrategy<string>(TrieOptions.None)
				//new LinkedListTrieNodeStrategy<string>(TrieOptions.None)
				new LinkTrieNodeStrategy<string>(TrieOptions.None)
			)
		{
		}

		public void Insert(string value)
		{
			if (value != null && value.Length > 0)
			{
				//this.Insert(this.CreateRange(container,0,container.Length));
				this.Insert(this.RangeStategy.Create(value, 0, value.Length));
			}
		}
		public void Delete(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "value is null");
			}
			this.Delete(value, 0, value.Length);
		}

		#region IEnumerable<string> Members
		[DebuggerStepThrough]
		public IEnumerator<string> GetEnumerator()
		{
			return new StringTrieEnumerator(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	public class StringArrayTrie :
		//ArrayTrie<string>,IEnumerable<string>
		LinkTrie<string>, IEnumerable<string>
	{
		public StringArrayTrie()
			: base(
				new StringRangeStrategy(),
				TrieOptions.None
			)
		{
		}

		public void Insert(string value)
		{
			if (value != null && value.Length > 0)
			{
				//this.Insert(this.CreateRange(container,0,container.Length));
				this.Insert(this.RangeStategy.Create(value, 0, value.Length));
			}
		}
		public void Delete(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "value is null");
			}
			this.Delete(value, 0, value.Length);
		}

		#region IEnumerable<string> Members
		[DebuggerStepThrough]
		public IEnumerator<string> GetEnumerator()
		{
			return new StringTrieEnumerator(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	public class StringLinkTrie : LinkTrie<string>, IEnumerable<string>
	{
		public StringLinkTrie()
			: base(
				new StringRangeStrategy(),
				TrieOptions.None
			)
		{
		}

		public void Insert(string value)
		{
			if (value != null && value.Length > 0)
			{
				//this.Insert(this.CreateRange(container,0,container.Length));
				this.Insert(this.RangeStategy.Create(value, 0, value.Length));
			}
		}
		public void Delete(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "value is null");
			}
			this.Delete(value, 0, value.Length);
		}

		#region IEnumerable<string> Members
		[DebuggerStepThrough]
		public IEnumerator<string> GetEnumerator()
		{
			return new StringTrieEnumerator(this);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}

	public class StringTrieEnumerator : ATrieEnumerator<string>
	{
		private StringBuilder buffer;

		public StringTrieEnumerator(Trie<string> trie)
			: base(trie)
		{
			this.buffer = new StringBuilder(256);
		}

		protected override string ValueFromPath(List<ATrieEnumerator<string>.TrieEnumeratorNode> path)
		{
			//StringBuilder buffer = new StringBuilder(64);
			IRange<string> range = null;
			for (int index = 0; index < path.Count; index++)
			{
				range = path[index].Node.Range;
				if (range != null)
				{
					this.buffer.Append(range.Container, range.Start, range.Length);
				}
			}
			range = this.CurrentNode.Range;
			if (range != null)
			{
				this.buffer.Append(range.Container, range.Start, range.Length);
			}
			string retval = this.buffer.ToString();
			this.buffer.Remove(0, this.buffer.Length);
			return retval;
		}

		public override void Dispose()
		{
			this.buffer = null;
			base.Dispose();
		}
	}

}