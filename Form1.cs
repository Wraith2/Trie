using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using System.IO;
using System.Diagnostics;
using Wraith.Collections.Generic;
using Wraith.Collections;

namespace TrieTest
{
	public partial class Form1 : Form
	{
		//private TrieNode root;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender,EventArgs e)
		{

			/*
			TrieNode root = new TrieNode(Range.Empty);
			string[] words = new string[] {
				"florida",
				"flamed",
				"flame",
				"flamer"
			};
			foreach (string word in words)
			{
				root.Insert(word);
			}
			root.Delete("flamer");
			root.Delete("flame");
			

			bool flame = root.Lookup("flamer",0,5);

			TreeNode rootNode = new TreeNode("root");
			DrawNode(rootNode,root);
			this.treeView1.Nodes.Add(rootNode);
			*/

			//TrieNode root = new TrieNode(null);
			StringTrie trie = new StringTrie();
		
			//trie.Insert("aaaaaaa");
			//trie.Insert("bbbbbbb");
			//trie.Delete("aaaaaaa");
			//trie.Delete("bbbbbbb");

			trie.Insert("aaaaaaa");
			trie.Insert("aaabbbb");
			trie.Delete("aaaaaaa");
			trie.Delete("aaabbbb");

			//this.DoFile();

			//this.EnumStrings();

			//SplitStrings();
		}

		private void DoFile()
		{
			string[] words = null;
			string fileName = "bill.txt";
			if (File.Exists(fileName))
			{
				using (StreamReader reader = new StreamReader(fileName))
				{
					string text = reader.ReadToEnd();
					if (text!=null)
					{
						words=text.Split(new char[] { ' ','\t','\r','\n' },StringSplitOptions.RemoveEmptyEntries);
					}
				}
			}


			//TrieNode root = new TrieNode(Range.Empty);
			//TrieNode root = new TrieNode(null);
			StringTrie root = new StringTrie();
			/*
			List<string> list = new List<string>(words);
			Random random = new Random();
			while (list.Count>0)
			{
				int index = random.Next(0,list.Count-1);
				root.Insert(list[index]);
				list.RemoveAt(index);
			}

			list = new List<string>(words);
			while (list.Count>0)
			{
				int index = random.Next(0,list.Count);
				root.Delete(list[index]);
				list.RemoveAt(index);
			}
			*/

			//return;

			/*
			foreach (string word in words)
			{
				root.Insert(word);
			}

			foreach (string word in words)
			{
				root.Delete(word);
			}
			*/
			//return;
			
			
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (words!=null)
			{
				foreach (string word in words)
				{
					root.Insert(word);
				}
			}
			stopwatch.Stop();
			Debug.Print(string.Format("added {0:D} in {1}",words.Length,stopwatch.Elapsed.ToString()));

			bool matched = false;
			int failCount=0;
			TimeSpan[] times = new TimeSpan[words.Length];
			for (int index=0;index<words.Length;index++)
			{
				matched=false;
				stopwatch.Reset();
				stopwatch.Start();
				matched=root.Lookup(words[index],0,words[index].Length);
				stopwatch.Stop();
				times[index]=stopwatch.Elapsed;
				if (!matched)
				{
					failCount+=1;
					root.Lookup(words[index],0,words[index].Length);
				}
			}

			bool failed = root.Lookup("moron",0,5);

			//root.Delete("2001");

			for (int index=0;index<words.Length;index++)
			{
				root.Delete(words[index]);
			}
			
			TimeSpan average = TimeSpan.Zero;
			decimal total=0;
			unchecked
			{
				foreach (TimeSpan time in times)
				{
					total+=(decimal)time.TotalMilliseconds;
				}
				average=new TimeSpan((Int64)(total/(Decimal)words.Length));
			}

			TimeSpan maximum=TimeSpan.MinValue;
			TimeSpan minimum=TimeSpan.MaxValue;
			TimeSpan threshold = TimeSpan.FromMilliseconds(1);
			int overThreshold=0;
			new List<TimeSpan>(times).ForEach(
				delegate(TimeSpan time)
				{
					if (time>maximum)
					{
						maximum=time;
					}
					if (time<minimum)
					{
						minimum=time;
					}
					if (time>threshold)
					{
						overThreshold+=1;
					}
				}
			);

			Debug.Print(string.Format("average lookup {0}",average.ToString()));
			Debug.Print(string.Format("maximum lookup {0}",maximum.ToString()));
			Debug.Print(string.Format("minimum lookup {0}",minimum.ToString()));

			TreeNode rootNode = new TreeNode("()");
			DrawNode(rootNode,root.Root);
			this.treeView1.Nodes.Add(rootNode);
			rootNode.ExpandAll();
			
		}

		private void DrawNode(TreeNode parent,ITrieNode<string> item)
		{
			if (parent!=null && item!=null)
			{
				string text = "<EMPTY>";
				if (item.Range!=null && item.Range.Container!=null)
				{
					text= item.Range.Container.Substring(
						 item.Range.Start,
						 item.Range.Length
					 );
				}
				else
				{
					Debug.WriteLine("warning: empty entry found");
				}
				TreeNode node = new TreeNode(text);
				if (item.IsTerminator)
				{
					node.ForeColor=Color.Red;
				}
				if (item.IsBranch)
				{
					foreach (ITrieNode<string> child in item.Children)
					{
						DrawNode(node,child);
					}
				}
				parent.Nodes.Add(node);
			}
		}

		public void EnumStrings()
		{
			string[] words = null;
			string fileName = "bill.txt";
			if (File.Exists(fileName))
			{
				using (StreamReader reader = new StreamReader(fileName))
				{
					string text = reader.ReadToEnd();
					if (text!=null)
					{
						words=text.Split(new char[] { ' ','\t','\r','\n' },StringSplitOptions.RemoveEmptyEntries);
					}
				}
			}

			StringTrie trie = new StringTrie();
			if (words!=null)
			{
			    foreach (string word in words)
			    {
			        trie.Insert(word);
			    }
			}

			List<string> list = new List<string>(1000);
			IEnumerator<string> enumerator = trie.GetEnumerator();
			if (enumerator!=null)
			{
				while (enumerator.MoveNext())
				{
					list.Add(enumerator.Current);
				}
			}

			foreach (string word in words)
			{
				if (!list.Contains(word))
				{
					throw new InvalidOperationException("trie does not contain word");
				}
			}
		}



		//public void SplitStrings()
		//{
		//    string[] words = null;
		//    string fileName = "bill.txt";
		//    string text=null;
		//    if (File.Exists(fileName))
		//    {
		//        using (StreamReader reader = new StreamReader(fileName))
		//        {
		//            text = reader.ReadToEnd();
		//            if (text!=null)
		//            {
		//                words=text.Split(new char[] { ' ','\t','\r','\n' },StringSplitOptions.RemoveEmptyEntries);
		//            }
		//        }
		//    }

		//    StringTrie trie = new StringTrie();
		//    StringTrie.Split(
		//        new StringRange(text),
		//        trie,
		//        new char[] { ' ','\t','\r','\n' },
		//        StringSplitOptions.RemoveEmptyEntries
		//    );


		//    List<string> list = new List<string>(2000);
		//    IEnumerator<string> enumerator = trie.GetEnumerator();
		//    int moved = 0;
		//    if (enumerator!=null)
		//    {
		//        while (enumerator.MoveNext())
		//        {
		//            //string current = enumerator.Current;
		//            //Debug.WriteLine(current);
		//            moved+=1;
		//            list.Add(enumerator.Current);
		//        }
		//    }

		//    foreach (string word in words)
		//    {
		//        if (!list.Contains(word))
		//        {
		//            throw new InvalidOperationException("trie does not contain word");
		//        }
		//    }
		//}
	}

}