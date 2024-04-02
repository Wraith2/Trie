using System;
using System.Collections.Generic;
using System.Text;
using Wraith.Collections.Generic.Specialized;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Wraith.Collections.Generic.Serialization
{
	public abstract class ATrieSerializer<T>
	{
		public delegate void Move(Instruction instruction,ITrieNode<T> node);
		public enum Instruction : int
		{
			Start=-1,
			Up=-2,
			Down=-3,
			End=-4
		}

		private IList<int> list;
		private IDictionary<IRange<T>,IPair<int,int>> map;

		protected ATrieSerializer()
		{
			//this.ranges=new List<IPair<int,int>>();
			//this.list=new List<int>(256);
		}

		//public void TraverseTrie(Trie<T> trie,Action<IRange<T>> action)
		//{
		//    StackNode<TrieTraverseNode> stack=null;
		//    ITrieNode<T> current=trie.Root;
		//    IEnumerator<ITrieNode<T>> enumerator=null;
		//    while (current!=null)
		//    {
		//        action(current.Range);

		//        if (current.IsBranch)
		//        {
		//            enumerator=current.GetEnumerator();
		//            if (!enumerator.MoveNext())
		//            {
		//                throw new InvalidOperationException("branch with no children");
		//            }
		//            StackNode<TrieTraverseNode>.Push(ref stack,new TrieTraverseNode(current,enumerator));
		//            current=enumerator.Current;
		//        }
		//        else if (stack!=null)
		//        {
		//            enumerator=stack.Value.Enumerator;
		//            if (enumerator.MoveNext())
		//            {
		//                current=enumerator.Current;
		//            }
		//            else
		//            {
		//                StackNode<TrieTraverseNode>.Pop(ref stack);
		//            }
		//        }
		//        else
		//        {
		//            current=null;
		//        }
		//    }

		//}

		protected virtual void Serialize(Trie<T> trie)
		{
			this.Begin();

			StackNode<IPair<ITrieNode<T>,IEnumerator<ITrieNode<T>>>> stack = null;
			ITrieNode<T> current=trie.Root;
			IEnumerator<ITrieNode<T>> enumerator=null;
			IRange<T> range=null;
			IPair<int,int> position=null;
			while (current!=null)
			{
				position=null;
				range=current.Range;
			    if (range!=null && !map.TryGetValue(range,out position))
			    {
			        position=this.AppendValue(range);
			        map.Add(range,position);
			    }
			    this.AppendPosition(position);

				if (current.IsBranch)
				{
					enumerator=current.Children.GetEnumerator();
					if (!enumerator.MoveNext())
					{
						throw new InvalidOperationException("branch with no children");
					}
					this.AppendInstruction(Instruction.Down,current.TerminatorCount);
					StackNode<IPair<ITrieNode<T>,IEnumerator<ITrieNode<T>>>>.Push(
						ref stack,
						new Pair<ITrieNode<T>,IEnumerator<ITrieNode<T>>>(
							current,
							enumerator
						)
					);
					current=enumerator.Current;
				}
				else if (stack!=null)
				{
					enumerator=stack.Value.Second;
					if (enumerator.MoveNext())
					{
						current=enumerator.Current;
					}
					else
					{
						this.AppendInstruction(Instruction.Up,1);
						StackNode<IPair<ITrieNode<T>,IEnumerator<ITrieNode<T>>>>.Pop(ref stack);
					}
				}
				else
				{
					current=null;
				}
			}

			this.End();

		}
		protected virtual void Remap(Trie<T> trie)
		{

		}

		protected virtual IList<int> List
		{
			get
			{
				return this.list;
			}
		}
		protected virtual IDictionary<IRange<T>,IPair<int,int>> Map
		{
			get
			{
				return this.map;
			}
		}

		protected virtual void Begin()
		{
			this.list=new List<int>(256);
			this.map = new Dictionary<IRange<T>,IPair<int,int>>();

		}
		protected virtual void End()
		{
			this.map.Clear();
		}

		protected virtual void AppendInstruction(Instruction instruction,int value)
		{
			this.List.Add((int)instruction);
			this.List.Add(value);
		}
		protected virtual void AppendPosition(IPair<int,int> position)
		{
			if (position!=null)
			{
				this.List.Add(position.First);
				this.List.Add(position.Second);
			}
			else
			{
				this.List.Add(0);
				this.List.Add(0);
			}
		}

		protected abstract IPair<int,int> AppendValue(IRange<T> range);
		protected abstract T Value
		{
			get;
		}
	}

	//public class ArrayTrieSerializer<T> : TrieSerializer<T[]>
	//{
	//}

	public class StringTrieSerializer : ATrieSerializer<string>
	{
		[NonSerialized]
		private StringBuilder buffer;
		private string value;

		protected override void Begin()
		{
			base.Begin();
			this.buffer=new StringBuilder(256);
		}
		protected override void End()
		{
			this.value=this.buffer.ToString();
			this.buffer.Remove(0,this.buffer.Length);
			this.buffer=null;
			base.End();
		}

		protected override IPair<int,int> AppendValue(IRange<string> range)
		{
			int start = this.buffer.Length;
			this.buffer.Append(range.Container,range.Start,range.Length);
			return new Pair<int,int>(start,range.Length);
		}
		protected override string Value
		{
			get
			{
				if (this.buffer!=null)
				{
					throw new InvalidOperationException("cannot get value while coalescing");
				}
				return this.value;
			}
		}
	}

}
