using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class XmlDataLoader
{
    // Define the method as generic with a type parameter T
    public T LoadFromXml<T>(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            return (T)serializer.Deserialize(stream);
        }
    }
}
