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
        bool HasProp<T>() where T : struct;
        T GetProp<T>() where T : struct;
        ref T GetRefProp<T>() where T : struct;
        IActor AddProp<T>(ref T property) where T : struct;
        IActor AddProp<T>(T property = default) where T : struct;
        IActor ReplaceProp<T>(ref T property) where T : struct;
        IActor ReplaceProp<T>(T property) where T : struct;
        IActor RemoveProp<T>() where T : struct;
        void Release();


        /// <summary>
        /// Don't use it. It's needed only for apply property from unity component or restore from binary
        /// </summary>
        /// <param name="property">serialized property</param>
        /// <returns>actor instance</returns>
        IActor RestorePropFromObject(object property);
    }
}