namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;

	public interface ITrieNodeStrategy<T>
	{
		ITrieNode<T> Create(IRange<T> range);
		ITrieNode<T> Create(IRange<T> range,IEnumerable<ITrieNode<T>> children,int terminationCount);

		TrieOptions Options
		{
			get;
		}

	}
}
