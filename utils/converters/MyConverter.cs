// Non trivial converters are available here
class MyConverter {
    public static string HexToBinary(string hexValue, int size=4)
    {
        var converted = Convert.ToString(Convert.ToInt32(hexValue, 16), 2);
        return int.Parse(converted).ToString($"D{size}");
    }


    public static string HexToBinary(char hexValue)
    {
        return HexToBinary(string.Empty + hexValue);
    }    

}