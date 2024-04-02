namespace Wraith.Collections
{
	using System;

	public interface IRange :
		IEquatable<IRange>
	{
		object Object
		{
			get;
		}
		int Start
		{
			get;
		}
		int Length
		{
			get;
		}
	}
	namespace Generic
	{
		public interface IRange<TContainer> :
			IRange,
			IEquatable<IRange<TContainer>>,
			IComparable<IRange<TContainer>>
		{
			TContainer Container
			{
				get;
			}
			//int Start
			//{
			//    get;
			//}
			//int Length
			//{
			//    get;
			//}
		}
	}
}