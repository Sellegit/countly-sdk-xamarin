using System;
using System.Collections.Generic;
using System.Collections;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Countly
{
	public class PersistantQueue<T> : IEnumerable<T>
	{
		Queue<T> queue;
		public PersistantQueue ()
		{
			queue = new Queue<T> ();
			LoadState ();
		}

		
		private void LoadState()
		{
			try
			{
				using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var stream = myIsolatedStorage.OpenFile("countly-state", FileMode.OpenOrCreate))
					{
						{
							var formatter = new BinaryFormatter();
							queue = stream.Length > 0 ? (Queue<T>) formatter.Deserialize(stream) : new Queue<T>();
							Console.WriteLine("Items to be synced:{0}", queue.Count);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				queue = new Queue<T>();
			}
		}

		private void SaveState()
		{
			try
			{
				using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var stream = myIsolatedStorage.OpenFile("countly-state", FileMode.Create))
					{
						var formatter = new BinaryFormatter();
						formatter.Serialize(stream, queue);
						stream.Close();
					}
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex);
			}
		}

		#region IEnumerable implementation

		public IEnumerator<T> GetEnumerator ()
		{
			return queue.GetEnumerator();
		}

		#endregion
	
		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator ()
		{
			throw new NotImplementedException ();
		}

		#endregion

		public int Count 
		{
			get{ return queue.Count;}
		}

		public void Enqueue(T item)
		{
			queue.Enqueue (item);
			SaveState ();
		}
		public T Peek()
		{
			return queue.Peek();
		}
		public T Dequeue()
		{
			var item = queue.Dequeue ();
			//save
			SaveState ();
			return item;
		}
	}
}

