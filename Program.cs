using System;
using System.Collections.Generic;
using System.IO;
using Wraith.Collections.Generic;
using Wraith.Collections.Generic.Serialization;

namespace TrieTest
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		//[STAThread]
		//static void Main()
		//{
		//    Application.EnableVisualStyles();
		//    Application.SetCompatibleTextRenderingDefault(false);
		//    Application.Run(new Form1());
		//}


		[STAThread]
		public static void Main(string[] args)
		{
			LoadWords();
			Trie<string> trie = 
				new StringLinkTrie();
				//new StringArrayTrie();
				//new StringTrie();

			InsertWords(trie);
			CheckEnumerator(trie);
			CheckLookup(trie);
			RemoveWords(trie);


			//trie = new StringTrie();
			//InsertWords(trie);
			//Serialize(trie);
		}



		private static string[] words;
		private static string[] unique;

		public static void LoadWords()
		{
			string fileName = "bill.txt";
			string text=null;
			if (File.Exists(fileName))
			{
				using (StreamReader reader = new StreamReader(fileName))
				{
					text = reader.ReadToEnd();
				}
			}

			words=text.Split(
				new char[] { ' ','\t','\r','\n' },
				int.MaxValue,
				StringSplitOptions.RemoveEmptyEntries
			);

			List<string> list = new List<string>(words.Length);
			Dictionary<string,object> dictionary = new Dictionary<string,object>(StringComparer.Ordinal);
			foreach (string word in words)
			{
				if (!dictionary.ContainsKey(word))
				{
					dictionary.Add(word,null);
					list.Add(word);
				}
			}
			unique=list.ToArray();
			list.Clear();
			dictionary.Clear();
			GC.Collect();
		}

		public static void InsertWords(Trie<string> trie)
		{
			foreach (string word in words)
			{
				trie.Insert(word,0,word.Length);
			}
		}
		public static void RemoveWords(Trie<string> trie)
		{
			foreach (string word in words)
			{
				trie.Delete(word,0,word.Length);
			}
		}

		public static void CheckLookup(Trie<string> trie)
		{
			foreach (string word in words)
			{
				if (!trie.Lookup(word,0,word.Length))
				{
					throw new InvalidOperationException("trie does not contain word");
				}
			}
		}

		public static void CheckEnumerator(Trie<string> trie)
		{
			List<string> list = new List<string>(2000);
			IEnumerator<string> enumerator = (trie as IEnumerable<string>).GetEnumerator();
			int moved = 0;
			if (enumerator!=null)
			{
				while (enumerator.MoveNext())
				{
					moved+=1;
					list.Add(enumerator.Current);
				}
			}
			if (list.Count!=unique.Length)
			{
				throw new InvalidOperationException("trie does not contain all unique items");
			}
			foreach (string word in words)
			{
				if (!list.Contains(word))
				{
					throw new InvalidOperationException("trie does not contain word");
				}
			}
		}

		public static void Serialize(Trie<string> trie)
		{
			StringTrieSerializer serializer = new StringTrieSerializer();
			//serializer.Serialize(trie);
		}

	}
}