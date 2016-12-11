using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class EventController : MonoBehaviour {

	private static EventController instance;
	public static EventController Instance
	{
		get
		{
			if (instance == null)
				instance = GameObject.FindGameObjectWithTag ("EventController").GetComponent<EventController> () as EventController;

			return instance;
		}
	}

	private static Dictionary<string, object> publishers = new Dictionary<string, object>();

	public static void Subscribe<T>(ISubscription<T> subscription) 
	{
		var publisher = GetOrCreatePublisher<T> (subscription.EventName);

		var subscriber = new Subscriber<T> (publisher);
		subscriber.Publisher.DataPublisher += subscription.Callback;
	}

	public static void Unsubscribe<T>(ISubscription<T> subscription)
	{
		var publisher = GetPublisher<T> (subscription.EventName);

		if (publisher == null)
			return;

		publisher.DataPublisher -= subscription.Callback;
	}

	public static void Publish<T>(string eventName, T payload)
	{
		var publisher = GetPublisher<T>(eventName);
		publisher.PublishData (payload);
	}

	private static Publisher<T> GetOrCreatePublisher<T>(string eventName)
	{
		var publisher = GetPublisher<T>(eventName);

		if(publisher == null) 
		{
			publisher = CreatePublisher<T> (eventName);
		}

		return publisher;
	}

	private static Publisher<T> CreatePublisher<T>(string eventName)
	{
		var publisher = new Publisher<T>();
		publishers.Add (eventName, publisher);

		return publisher;
	}

	private static Publisher<T> GetPublisher<T>(string eventName)
	{
		Publisher<T> publisher = null;

		if (publishers.ContainsKey(eventName))
			publisher = publishers [eventName] as Publisher<T>;

		return publisher;
	}
}

public interface ISubscription<T> {
	string EventName { get; }
	EventHandler<EventMessage<T>> Callback { get; }
}

public class Subscription<T> : ISubscription<T> {
	public string EventName { get; }
	public EventHandler<EventMessage<T>> Callback { get; }

	public Subscription(string eventName, EventHandler<EventMessage<T>> callback)
	{
		EventName = eventName;
		Callback = callback;
	}
}

public class EventMessage<T> : EventArgs  
{  
	public T Payload { get;  set; }  
	public EventMessage(T payload)  
	{  
		Payload = payload;  
	}
} 

public interface IPublisher<T>  
{  
	event EventHandler<EventMessage<T>> DataPublisher;  
	void OnDataPublisher(EventMessage<T> args);  
	void PublishData(T data);  
}  

/// <summary>
/// A subscriber gets it's Publisher,
/// Calls PublishData with the payload,
/// Which calls OnDataPublisher and Invokes 'this' with payload.
/// </summary>
public class Publisher<T> : IPublisher<T>  
{  
	//Defined datapublisher event  
	public event EventHandler<EventMessage<T>> DataPublisher;  

	public void OnDataPublisher(EventMessage<T> args)  
	{  
		var handler = DataPublisher;  

		if (handler != null)  
			handler(this, args);
	}  

	/// <summary>
	/// Create a message and publish the data.
	/// </summary>
	/// <param name="data">Data.</param>
	public void PublishData(T data)  
	{  
		EventMessage<T> message = (EventMessage<T>)Activator.CreateInstance(typeof(EventMessage<T>), new object[] { data });  
		OnDataPublisher(message);  
	}  
}  

/// <summary>
/// A subscriber has a publisher.
/// </summary>
public class Subscriber<T>  
{  
	public IPublisher<T> Publisher { get; private set; }  

	public Subscriber(IPublisher<T> publisher)  
	{  
		Publisher = publisher;  
	}  
}  