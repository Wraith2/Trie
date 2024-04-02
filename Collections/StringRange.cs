namespace Wraith.Collections
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
	using System.Diagnostics;

	using Wraith.Collections.Generic;

	[DebuggerStepThrough]
	[DebuggerDisplay("..{ToString(),nq}..", Type = "StringRange")]
	//[DebuggerTypeProxy(typeof(StringRangeTypeProxy))]
	public sealed partial class StringRange :
		IRange<string>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string container;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int start;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int length;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mutable;

		[DebuggerStepThrough]
		public StringRange(string container)
			: this(container, 0, ((container != null) ? container.Length : 0), false)
		{
		}
		[DebuggerStepThrough]
		public StringRange(string container, int start, int length)
			: this(container, start, length, false)
		{
		}
		[DebuggerStepThrough]
		public StringRange(string container, int start, int length, bool mutable)
		{
			this.container = container;
			this.start = start;
			this.length = length;
			this.mutable = mutable;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string Value
		{
			[DebuggerStepThrough]
			get
			{
				return this.ToString();
			}
		}

		#region IRange<string> Members

		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public int Start
		{
			[DebuggerStepThrough]
			get
			{
				return this.start;
			}
			[DebuggerStepThrough]
			set
			{
				//this.CheckMutable();
				if (!this.mutable)
				{
					throw new InvalidOperationException("cannot assign properties of immutable StringRange instances");
				}
				this.start = value;
			}
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
		public int Length
		{
			[DebuggerStepThrough]
			get
			{
				return this.length;
			}
			set
			{
				//this.CheckMutable();
				if (!this.mutable)
				{
					throw new InvalidOperationException("cannot assign properties of immutable StringRange instances");
				}
				this.length = value;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string Container
		{
			get
			{
				return this.container;
			}
			set
			{
				this.container = value;
			}
		}

		#endregion

		#region IEquatable<string> Members

		public bool Equals(string other)
		{
			return (
				!object.Equals(other, null) &&
				this.CompareTo(other) == 0
			);
		}

		#endregion

		#region IComparable<string> Members

		public int CompareTo(string other)
		{
			return this.CompareTo(other, 0, ((other != null) ? other.Length : 0));
			/*
			int consumed=1;
			if (!object.Equals(other,null))
			{
				consumed= string.Compare(
					this.container,this.yStart,
					other,0,
					Math.Max(this.Length,other.Length),
					StringComparison.Ordinal
				);
			}
			return consumed;
			*/
		}

		#endregion

		public int CompareTo(string other, int start, int length)
		{
			return this.CompareTo(other, start, length, StringComparison.Ordinal);
			//int retval=0;
			//if (
			//    object.ReferenceEquals(this.container,other) &&
			//    this.Start==start
			//    )
			//{
			//    if (other!=null)
			//    {
			//        retval=(this.Length-length);
			//    }
			//}
			//else
			//{
			//    retval=string.Compare(
			//        this.Container,this.Start,
			//        other,start,
			//        Math.Max(this.Length,length),
			//        StringComparison.Ordinal
			//    );
			//}

			//return retval;
		}
		public int CompareTo(string other, int start, int length, StringComparison comparison)
		{
			int retval = 0;
			if (
				object.ReferenceEquals(this.container, other) &&
				this.Start == start
				)
			{
				if (other != null)
				{
					retval = (this.Length - length);
				}
			}
			else
			{
				retval = string.Compare(
					this.Container, this.Start,
					other, start,
					Math.Max(this.Length, length),
					comparison
				);
			}

			return retval;
		}

		#region IEquatable<IRange<string>> Members

		public bool Equals(IRange<string> other)
		{
			return (
				!object.Equals(other, null) &&
				(
					object.ReferenceEquals(this, other)
					||
					this.CompareTo(other) == 0
				)
			);
		}

		#endregion

		#region IComparable<IRange<string>> Members

		public int CompareTo(IRange<string> other)
		{
			int retval = 0;
			if (!object.ReferenceEquals(this, other))
			{
				if (
					object.ReferenceEquals(this.container, other.Container) &&
					this.Start == other.Start
					)
				{
					if (this.container != null)
					{
						retval = (this.Length - other.Length);
					}
				}
				else
				{
					retval = string.Compare(
						this.Container, this.Start,
						other.Container, other.Start,
						Math.Max(this.Length, other.Length),
						StringComparison.Ordinal
					);
				}
			}
			return retval;
		}

		#endregion

		#region IEquatable<StringRange> Members

		public bool Equals(StringRange other)
		{
			return this.Equals((IRange<string>)other);
			//return (
			//    !object.Equals(other,null) &&
			//    (
			//        object.ReferenceEquals(this,other)
			//        ||
			//        this.CompareTo(other)==0
			//    )
			//);

		}

		#endregion

		#region IComparable<StringRange> Members

		public int CompareTo(StringRange other)
		{
			return this.CompareTo(other);

			//int consumed=0;
			//if (!object.ReferenceEquals(this,other))
			//{
			//    if (
			//        object.ReferenceEquals(this.container,other.Container) &&
			//        this.Start==other.Start
			//        )
			//    {
			//        consumed=(this.length-other.Length);
			//    }
			//    else
			//    {
			//        consumed=string.Compare(
			//            this.Container,this.Start,
			//            other.Container,other.Start,
			//            Math.Max(this.Length,other.Length),
			//            StringComparison.Ordinal
			//        );
			//    }
			//}
			//return consumed;
		}

		#endregion



		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool Mutable
		{
			get
			{
				return this.mutable;
			}
		}

		public override string ToString()
		{
			return ((this.container != null) ? this.container.Substring(this.start, this.length) : string.Empty);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return (
					((this.container != null) ? this.container.GetHashCode() : 0) ^
					(this.start.GetHashCode() << 24) ^
					(this.length.GetHashCode() << 16) ^
					(this.mutable.GetHashCode() << 8)
					);
			}
		}
		public override bool Equals(object obj)
		{
			return (
				object.ReferenceEquals(this, obj) ||
				(
					(
						(obj is string) && this.Equals((string)obj)
					)
					||
					(
						(obj is StringRange) && this.Equals((StringRange)obj)
					)
				)
			);
		}

		#region IRange Members

		object IRange.Object
		{
			get
			{
				return this.container;
			}
		}

		#endregion

		#region IEquatable<IRange> Members

		public bool Equals(IRange other)
		{
			return (
				!object.Equals(other, null) &&
				(
					object.ReferenceEquals(this, other)
					||
					(
						this.Start == other.Start &&
						this.Length == other.Length &&
						object.Equals(this.Container, other.Object)
					)
				)
			);
		}

		#endregion



		public static bool operator ==(StringRange lhs, IRange<string> rhs)
		{
			bool retval = object.ReferenceEquals(lhs, rhs);
			if (!retval)
			{
				bool rhsNull = object.Equals(rhs, null);
				if (object.Equals(lhs, null))
				{
					retval = rhsNull;
				}
				else
				{
					if (!rhsNull)
					{
						retval = lhs.Equals(rhs);
					}
				}
			}
			return retval;
		}
		public static bool operator ==(IRange<string> lhs, StringRange rhs)
		{
			return (rhs == lhs);
		}
		public static bool operator !=(StringRange lhs, IRange<string> rhs)
		{
			return !(lhs == rhs);
		}
		public static bool operator !=(IRange<string> lhs, StringRange rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(StringRange lhs, string rhs)
		{
			bool retval = false;
			bool lhsNull = object.Equals(lhs, null);
			bool rhsNull = object.Equals(rhs, null);

			if (lhsNull)
			{
				retval = rhsNull;
			}
			else
			{
				if (!rhsNull)
				{
					retval = lhs.Equals(rhs);
				}
			}
			return retval;
		}
		public static bool operator ==(string lhs, StringRange rhs)
		{
			bool retval = false;
			bool rhsNull = object.Equals(rhs, null);
			bool lhsNull = object.Equals(lhs, null);

			if (rhsNull)
			{
				retval = lhsNull;
			}
			else
			{
				if (!lhsNull)
				{
					retval = rhs.Equals(lhs);
				}
			}
			return retval;
		}
		public static bool operator !=(StringRange lhs, string rhs)
		{
			return !(lhs == rhs);
		}
		public static bool operator !=(string lhs, StringRange rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator string(StringRange slice)
		{
			return ((slice.Equals((StringRange)null)) ? null : slice.Container.Substring(slice.Start, slice.Length));
		}
		public static implicit operator StringRange(string value)
		{
			return new StringRange(value, 0, ((value != null) ? value.Length : 0), false);
		}

	}

}
