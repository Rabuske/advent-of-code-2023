class FileReader {

    private static string _fileName = "";

    public FileReader(int day) {
        _fileName = @$"./input/day{day.ToString("00")}.txt";
    }

    public IEnumerable<string> Read() {
        return System.IO.File.ReadLines(_fileName);
    }
}