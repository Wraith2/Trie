namespace Wraith.Collections.Generic
{
#if DEBUG
	using System.Diagnostics;
#endif

#if DEBUG
	[DebuggerStepThrough]
	[DebuggerDisplay(
	   "[ {((first!=null)?first.ToString():string.Empty),nq}, {((second!=null)?second.ToString():string.Empty),nq} ]",
		Type = "Pair<{typeof(TFirst).Name,nq},{typeof(TSecond).Name,nq}>")
	]
#endif
	public sealed class Pair<TFirst, TSecond> : IPair<TFirst, TSecond>
	{
		private TFirst _first;
		private TSecond _second;

		public Pair()
			: this(default, default)
		{
		}
		public Pair(TFirst first, TSecond second)
		{
			_first = first;
			_second = second;
		}

		public TFirst First
		{
			get => _first;
			set => _first = value;
		}
		public TSecond Second
		{
			get => _second;
			set => _second = value;
		}

		public override int GetHashCode()
		{
			return (
				(((_first != null) ? _first.GetHashCode() : 0)) ^
				(((_second != null) ? _second.GetHashCode() : 0) >> 8)
			);
		}
	}
}