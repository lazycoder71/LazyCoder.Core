#if LFRAMEWORK_MEMORYPACK
using MemoryPack;
#else
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace LazyCoder.Data
{
    public class DataSerializer
    {
#if LFRAMEWORK_MEMORYPACK
        public static byte[] Serialize<T>(T data) where T : class
        {
            return MemoryPackSerializer.Serialize<T>(data);
        }

        public static T Deserialize<T>(byte[] data) where T : class
        {
            return MemoryPackSerializer.Deserialize<T>(data);
        }
#else
        public static byte[] Serialize<T>(T data) where T : class
        {
            if (data == null) return null;

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, data);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] byteArray)
        {
            if (byteArray == null) return default(T);

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                return (T)formatter.Deserialize(stream);
            }
        }
#endif
    }
}
