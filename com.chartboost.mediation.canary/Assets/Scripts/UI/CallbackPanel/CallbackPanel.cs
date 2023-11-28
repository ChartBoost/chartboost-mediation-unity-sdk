using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// An UI Panel to hold callback UI elements
/// </summary>
public class CallbackPanel : MonoBehaviour
{
    [SerializeField]
    private CallbackItem _callbackItemPrefab;    

    private Dictionary<string, CallbackItem> _callbacks = new Dictionary<string, CallbackItem>();

    /// <summary>
    /// Adds a callback UI display element to this panel 
    /// </summary>
    /// <param name="callback"></param>
    public void AddCallback(string callback)
    {
        var callbackItem = Instantiate(_callbackItemPrefab, transform).GetComponent<CallbackItem>();
        callbackItem.name = callback;
        callbackItem.Text.text = callback;

        _callbacks.Add(callback, callbackItem);
    }

    /// <summary>
    /// Adds multiple callback UI display elements to this panel
    /// </summary>
    /// <param name="callbacks"></param>
    public void AddCallbacks(string[] callbacks)
    {
        foreach(string callback in callbacks)
        {
            AddCallback(callback);
        }
    }

    /// <summary>
    /// Removes callback UI display element from this panel, if exists
    /// </summary>
    /// <param name="callback"></param>
    public void RemoveCallback(string callback)
    {
        if (!_callbacks.ContainsKey(callback))
        {
            throw new Exception($"Cannot remove callback. Callback {callback} deos not exist");
        }

        Destroy(_callbacks[callback].gameObject);
        _callbacks?.Remove(callback);
    }

    /// <summary>
    /// Removes multiple callback UI display elements from this panel, if exists
    /// </summary>
    /// <param name="callbacks"></param>
    public void RemoveCallbacks(string[] callbacks)
    {
        foreach(string callback in callbacks)
        {
            RemoveCallback(callback);
        }
    }

    /// <summary>
    /// Sets callback UI display element in success state
    /// </summary>
    /// <param name="callback"></param>
    public void SetCallbackSuccess(string callback)
    {
        if (!_callbacks.ContainsKey(callback))
        {
            throw new Exception($"Cannot set success. Callback {callback} deos not exist");
        }

        _callbacks[callback].SetSuccessful();
    }

    /// <summary>
    /// Sets callback UI display element in error state
    /// </summary>
    /// <param name="callback"></param>
    public void SetCallbackError(string callback)
    {
        if (!_callbacks.ContainsKey(callback))
        {
            throw new Exception($"Cannot set error. Callback {callback} deos not exist");
        }

        _callbacks[callback].SetInError();
    }

    /// <summary>
    /// Checks if requested callback is available in this panel
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public bool Contains(string callback)
    {
        return _callbacks.ContainsKey(callback);
    }

    /// <summary>
    /// Sets callback UI display element in inactive state
    /// </summary>
    /// <param name="callback"></param>
    public void SetInactive(string callback)
    {
        _callbacks[callback].SetInactive();
    }

    /// <summary>
    /// Sets the state of all the callbacks to be inactive
    /// </summary>
    public void Reset()
    {
        foreach(var callback in _callbacks.Keys)
        {
            _callbacks[callback].SetInactive();
        }
    }
}
