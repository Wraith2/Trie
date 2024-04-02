using System;
using System.Collections.Generic;

namespace Wraith.Collections.Generic.Specialized
{
	public sealed class StackNode<T>
	{
		public T Value;
		public StackNode<T> Previous;

		public StackNode()
			: this(null, default(T))
		{
		}
		public StackNode(T value)
			: this(null, value)
		{
		}
		public StackNode(StackNode<T> previous)
			: this(previous, default(T))
		{

		}
		public StackNode(StackNode<T> previous, T value)
		{
			this.Value = value;
			this.Previous = previous;
		}

		public bool Contains(T value)
		{
			bool retval = false;
			StackNode<T> previous = this;
			while (previous != null)
			{
				if (retval = object.Equals(value, previous.Value))
				{
					break;
				}
				previous = previous.Previous;
			}
			return retval;
		}
		public bool Contains(T value, IEqualityComparer<T> equality)
		{
			if (equality == null)
			{
				throw new ArgumentNullException("equality", "equality is null");
			}
			bool retval = false;
			for (StackNode<T> node = this; node != null; node = node.Previous)
			{
				if (retval = equality.Equals(value, node.Value))
				{
					break;
				}
			}
			return retval;
		}
		public bool ContainsStack(StackNode<T> list)
		{
			bool retval = false;
			for (
				StackNode<T> previous = this;
				previous != null;
				previous = previous.Previous
			)
			{
				if (retval = object.Equals(list, previous))
				{
					break;
				}
			}
			return retval;
		}
		public void CopyTo(T[] array, int index)
		{
			long count = 0;
			StackNode<T> previous = this;
			for (; previous != null; previous = previous.Previous)
			{
				count += 1;
			}
			for (previous = this, count -= 1; previous != null; --count, previous = previous.Previous)
			{
				array.SetValue(previous.Value, index + count);
			}
		}
		public T[] ToArray()
		{
			long count = 0;
			StackNode<T> previous = this;
			for (; previous != null; previous = previous.Previous)
			{
				count += 1;
			}
			T[] array = new T[count];
			for (previous = this, count -= 1; previous != null; --count, previous = previous.Previous)
			{
				array.SetValue(previous.Value, count);
			}
			return array;
		}

		public T Peek()
		{
			return this.Value;
		}

		public int Count
		{
			get
			{
				return GetCount(this);
			}
		}

		public override string ToString()
		{
			return string.Format("[{0}]", this.Value);
		}

		public static int GetCount(StackNode<T> stack)
		{
			int count = 0;
			for (
				StackNode<T> current = stack;
				current != null;
				current = current.Previous
			)
			{
				count += 1;
			}
			return count;
		}
		public static T Pop(ref StackNode<T> stack)
		{
			if (stack == null)
			{
				throw new ArgumentNullException("stack is empty");
			}
			T retval = default(T);
			StackNode<T> head = stack;
			stack = stack.Previous;
			retval = head.Value;
			head.Value = default(T);
			head.Previous = null;
			return retval;
		}
		public static void Push(ref StackNode<T> stack, T value)
		{
			stack = new StackNode<T>(stack, value);
		}
	}
}
