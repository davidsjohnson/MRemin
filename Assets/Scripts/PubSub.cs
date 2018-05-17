using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IPublisher<T>
{
    void Subscribe(ISubscriber<T> subscriber);
    void Unsubscribe(ISubscriber<T> subscriber);
    void SendNotifications(T message);
}

public interface ISubscriber<T>
{
    void Notify(T message);
}
