// Copyright(c) 2017 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php


/// <summary>
///     Represents a manage of objects that cretead on UWP.
/// </summary>
public interface IUWPBridgeServiceManager
{


    /// <summary>
    ///     Adds an object of <see cref="IUWPBridgeService" />
    /// </summary>
    /// <typeparam name="T">Type of the object to be acquired</typeparam>
    /// <param name="service">instance of service</param>
    void AddService<T>(IUWPBridgeService service) where T : IUWPBridgeService;

    /// <summary>
    ///     Acquires the specified object from the registered objects.
    /// </summary>
    /// <typeparam name="T">Type of the object to be acquired</typeparam>
    /// <returns>object</returns>
    T GetService<T>() where T : class, IUWPBridgeService;
}