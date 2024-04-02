using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NUnit;
using NUnit.Core;
using NUnit.Framework;




namespace Trie
{
	/*
	[TestFixture]
	public class Tests
	{
		private string[] words;

		[TestFixtureSetUp]
		public void SetUp()
		{
			string fileName = "bill.txt";
			if (File.Exists(fileName))
			{
				using (StreamReader reader = new StreamReader(fileName))
				{
					string text = reader.ReadToEnd();
					if (text!=null)
					{
						this.words=text.Split(new char[] { ' ','\t','\r','\n' },StringSplitOptions.RemoveEmptyEntries);
					}
				}
			}
			else
			{
				throw new InvalidOperationException(Path.Combine(Environment.CurrentDirectory,fileName));
			}

		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			words=null;
		}

		[Test(Description="SerialInsertRemove")]
		public void SerialInsertRemove()
		{
			TrieNode root = new TrieNode(null);

			foreach (string word in words)
			{
				root.Insert(word);
			}
			
			foreach (string word in words)
			{
				root.Delete(word);
			}

			Assert.IsTrue(root.Range.Length==0,"root length not 0");

			Assert.IsFalse(root.IsBranch,"root has children");
			
		}

		[Test(Description="RandomInsertRemove")]
		public void RandomInsertRemove()
		{
			TrieNode root = new TrieNode(null);

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

			Assert.IsTrue(root.Range.Length==0,"root length not 0");

			Assert.IsFalse(root.IsBranch,"root has children");
		}
	}

	*/
}
