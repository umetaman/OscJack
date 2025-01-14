using OscJack.Annotation;

namespace OscJack.SourceGenerator.Test
{
    public class OscDataHandle
    {
        public bool GetElementAsBool(int index) => false;
        public int GetElementAsInt(int index) => 0;
        public float GetElementAsFloat(int index) => 0;
        public string GetElementAsString(int index) => "";
        public byte[] GetElementAsBlob(int index) => new byte[0];
    }

    [OscPackable]
    public partial class TestClass
    {
        [OscElementOrder(0)]
        public int Value { get; set; }
    }
}

[OscPackable]
public partial class GlobalClass
{
    [OscElementOrder(0)]
    public float Value { get; set; }
    [OscElementOrder(1)]
    public Byte[] Bytes { get; set; }
}


public class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello, world!");
        Console.ReadLine();
    }
}