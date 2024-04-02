namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;

	public interface ITrieNode<T> //: IEnumerable<ITrieNode<T>>
	{
		IRange<T> Range
		{
			get;
			set;
		}
		int TerminatorCount
		{
			get;
			set;
		}
		IEnumerable<ITrieNode<T>> Children
		{
			get;
			//set;
		}

		bool IsTerminator
		{
			get;
		}
		bool IsBranch
		{
			get;
		}

		void Add(ITrieNode<T> child);
		void Remove(ITrieNode<T> child);
		void Clear();
		ITrieNode<T> GetFirstChild();

		int Count
		{
			get;
		}
	}
}
