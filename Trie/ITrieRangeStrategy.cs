namespace Wraith.Collections.Generic
{
	using System;
	using System.Collections.Generic;

	public interface ITrieRangeStrategy<T>
	{
		IRange<T> Create(T container,int start,int length);
		IRange<T> Merge(IRange<T> lhs,IRange<T> rhs);

		void Split(IRange<T> range,int length,out IRange<T> lhs,out IRange<T> rhs);

		bool Match(T lhs,int lhsStart,int lhsLength,T rhs,int rhsStart,int rhsLength);
		[Obsolete]
		int Shared(IRange<T> lhs,int lhsOffset,IRange<T> rhs,int rhsOffset);

		int Shared(T lhs,int lhsStart,int lhsLength,T rhs,int rhsStart,int rhsLength);
	}
}
