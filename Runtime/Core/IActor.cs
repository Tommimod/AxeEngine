using System;
using System.Collections.Generic;

namespace AxeEngine
{
    public interface IActor
    {
        #region internal

        internal IReadOnlyList<Type> GetAllProperties();
        internal void Restore();

        #endregion

        int Id { get; }
        bool IsAlive { get; }

        Action<IActor, Type> OnPropertyAdded { get; set; }
        Action<IActor, Type> OnPropertyReplaced { get; set; }
        Action<IActor, Type> OnPropertyRemoved { get; set; }
        /// <summary>
        /// Check if actor has property
        /// </summary>
        /// <typeparam name="T">type for check</typeparam>
        /// <returns>this actor</returns>
        bool HasProp<T>() where T : struct;
        /// <summary>
        /// Return reference to property. Not safe. Check HasProp first
        /// </summary>
        /// <typeparam name="T">type for return</typeparam>
        /// <returns>this actor</returns>
        ref T GetProp<T>() where T : struct;
        object GetPropObject(Type type);
        /// <summary>
        /// Create or remove property by flag. Useful for properties without fields
        /// </summary>
        /// <param name="isEnabled">should be created or removed</param>
        /// <typeparam name="T">struct</typeparam>
        /// <returns>this actor</returns>
        IActor SetPropertyEnabled<T>(bool isEnabled) where T : struct;
        /// <summary>
        /// Add reference to property to actor. If exist, it will be replaced
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T">type for add</typeparam>
        /// <returns>this actor</returns>
        IActor AddProp<T>(ref T property) where T : struct;
        /// <summary>
        /// Add property to actor. If exist, it will be replaced
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T">type for add</typeparam>
        /// <returns>this actor</returns>
        IActor AddProp<T>(T property = default) where T : struct;
        /// <summary>
        /// Replace reference to property to actor
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T">type for replace</typeparam>
        /// <returns>this actor</returns>
        IActor ReplaceProp<T>(ref T property) where T : struct;
        /// <summary>
        /// Replace property to actor
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T">type for replace</typeparam>
        /// <returns>this actor</returns>
        IActor ReplaceProp<T>(T property) where T : struct;
        /// <summary>
        /// Remove property from actor
        /// </summary>
        /// <typeparam name="T">type for remove</typeparam>
        /// <returns>this actor</returns>
        IActor RemoveProp<T>() where T : struct;
        /// <summary>
        /// Clear actor. Called when actor should return to the pool
        /// </summary>
        /// <returns></returns>
        void Release();

        /// <summary>
        /// Don't use it. It's needed only for apply property from unity component or restore from binary
        /// </summary>
        /// <param name="property">serialized property</param>
        /// <returns>actor instance</returns>
        IActor RestorePropFromObject(object property);
    }
}