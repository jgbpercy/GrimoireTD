using System.Collections;
using System.Collections.Generic;
using System;

public class CallbackList<T> {

    private List<T> list;

    private Action<T> OnAdd;
    private Action<T> OnRemove;

    public List<T> List
    {
        get
        {
            return list;
        }
    }

    public CallbackList()
    {
        list = new List<T>();
    }

    public void Add(T item)
    {
        list.Add(item);

        if ( OnAdd != null )
        {
            OnAdd(item);
        }
    }

    public bool TryRemove(T item)
    {
        if ( !list.Contains(item) )
        {
            return false;
        }

        list.Remove(item);

        if( OnRemove != null )
        {
            OnRemove(item);
        }

        return true;
    }

    public void RegisterForAdd(Action<T> callback)
    {
        OnAdd += callback;
    }

    public void DeregisterForAdd(Action<T> callback)
    {
        OnAdd -= callback;
    }

    public void RegisterForRemove(Action<T> callback)
    {
        OnRemove += callback;
    }

    public void DeregisterForRemove(Action<T> callback)
    {
        OnRemove -= callback;
    }
}
