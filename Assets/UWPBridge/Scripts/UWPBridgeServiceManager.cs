// Copyright(c) 2017 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using UnityEngine;


/// <summary>
///     Represents a manage of objects that cretead on UWP.
/// </summary>
public class UWPBridgeServiceManager : IUWPBridgeServiceManager
{
    protected static readonly object LockObject = new Object();
    private static IUWPBridgeServiceManager _instance;
    private static readonly Object _lockObject = new Object();

    private UWPBridgeServiceManager()
    {
    }

    /// <summary>
    ///     Gets this class instance.
    /// </summary>
    public static IUWPBridgeServiceManager Instance
    {
        get
        {
            lock (_lockObject)
            {
                if (_instance == null)
                    _instance = new UWPDefaultBridgeServiceManager();
            }
            return _instance;
        }
    }

    /// <summary>
    ///     Adds an object of <see cref="IUWPBridgeService" />
    /// </summary>
    /// <typeparam name="T">Type of the object to be acquired</typeparam>
    /// <param name="service">instance of service</param>
    public void AddService<T>(IUWPBridgeService service) where T : IUWPBridgeService
    {
        lock (LockObject)
        {
            _instance.AddService<T>(service);
        }
    }

    /// <summary>
    ///     Acquires the specified object from the registered objects.
    /// </summary>
    /// <typeparam name="T">Type of the object to be acquired</typeparam>
    /// <returns>object</returns>
    public T GetService<T>() where T : class, IUWPBridgeService
    {
        return _instance.GetService<T>();
    }
}