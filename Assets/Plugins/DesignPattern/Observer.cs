using System.Collections.Generic;

namespace UnityBase.DesignPattern
{
	public class Observer : Singleton<Observer>
	{
		public delegate void CallbackObserverParam(params object[] data);
		public delegate void CallbackObserverVoid();

		private readonly Dictionary<EventID, HashSet<CallbackObserverParam>> callbackPara = new Dictionary<EventID, HashSet<CallbackObserverParam>>();
		private readonly Dictionary<EventID, HashSet<CallbackObserverVoid>>  callbackVoid = new Dictionary<EventID, HashSet<CallbackObserverVoid>>();

		public void AddObserver(EventID topicName, CallbackObserverVoid callback)
		{
			CreateListObserverForTopicVoid(topicName).Add(callback);
		}
		
		public void AddObserver(EventID topicName, CallbackObserverParam callbackObserverParam)
		{
			CreateListObserverForTopicParam(topicName).Add(callbackObserverParam);
		}

		public void RemoveObserver(EventID topicName, CallbackObserverVoid callback)
		{
			var listObserver = CreateListObserverForTopicVoid(topicName);
			listObserver.Remove(callback);
		}
		
		public void RemoveObserver(EventID topicName, CallbackObserverParam callbackObserverParam)
		{
			var listObserver = CreateListObserverForTopicParam(topicName);
			listObserver.Remove(callbackObserverParam);
		}

		public void RemoveAllObserver()
		{
			callbackPara.Clear();
			callbackVoid.Clear();
		}

		public void Notify(EventID topicName, params object[] data)
		{
			foreach (var observer in CreateListObserverForTopicParam(topicName))
				observer(data);
			foreach (var observer in CreateListObserverForTopicVoid(topicName))
				observer(); 
		}

		public void Notify(EventID topicName)
		{
			foreach (var observer in CreateListObserverForTopicParam(topicName))
				observer();
			foreach (var observer in CreateListObserverForTopicVoid(topicName))
				observer(); 
		}

		private HashSet<CallbackObserverParam> CreateListObserverForTopicParam(EventID topicName)
		{
			if (!callbackPara.ContainsKey(topicName))
				callbackPara.Add(topicName, new HashSet<CallbackObserverParam>());
			return callbackPara[topicName];
		}
		
		private HashSet<CallbackObserverVoid> CreateListObserverForTopicVoid(EventID topicName)
		{
			if (!callbackVoid.ContainsKey(topicName))
				callbackVoid.Add(topicName, new HashSet<CallbackObserverVoid>());
			return callbackVoid[topicName];
		}
	}
}
