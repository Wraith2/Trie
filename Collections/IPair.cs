namespace Wraith.Collections.Generic
{
	public interface IPair<TFirst, TSecond>
	{
		TFirst First
		{
			get;
		}
		TSecond Second
		{
			get;
		}
	}
}
