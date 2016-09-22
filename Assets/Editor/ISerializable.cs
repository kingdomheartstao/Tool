#region Assembly mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\Unity Subset v3.5\mscorlib.dll
#endregion

using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
    [ComVisible(true)]
    public interface ISerializable
    {
        void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}