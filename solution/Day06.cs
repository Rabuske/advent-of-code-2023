using System.Text.RegularExpressions;

class Day06 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(06).Read().ToList();
    var times = Regex.Replace(lines[0].Split(":")[1], @"\s+", " ").Trim().Split(" ").Select(long.Parse).ToArray();
    var distanceRecord = Regex.Replace(lines[1].Split(":")[1], @"\s+", " ").Trim().Split(" ").Select(long.Parse).ToArray();

    var multiplicationOfResults = 1L;
    for (int index = 0; index < times.Length; index++)
    {
        var time = times[index];
        var distance = distanceRecord[index];

        long numberOfWinningCombinations = CalculateNumberOfWaysWinning(time, distance);
        multiplicationOfResults *= numberOfWinningCombinations;
    }

    var actualTime = long.Parse(lines[0].Split(":")[1].Replace(" ", ""));
    var actualDistance = long.Parse(lines[1].Split(":")[1].Replace(" ", ""));
    var actualWaysOfWinning = CalculateNumberOfWaysWinning(actualTime, actualDistance);

    return $"""
    The lowest location is {multiplicationOfResults}
    The lowest location considering ranges is {actualWaysOfWinning}
    """;
  }

    private static long CalculateNumberOfWaysWinning(long time, long distance)
    {
        // distance = velocity * travelTime
        // velocity = acceleration * accelerationTime
        // travelTime = totalTime - accelerationTime
        // distance = velocity * (totalTime - accelerationTime)
        // distance = (acceleration * accelerationTime) * (totalTime - accelerationTime)
        // distance = (acceleration * accelerationTime * totalTime) - (acceleration * accelerationTime ^2) 
        // (acceleration * accelerationTime ^2) - (acceleration * totalTime * accelerationTime) + distance = 0
        // acceleration = a
        // accelerationTime = x
        // acceleration * totalTime = b
        // distance = c
        // We can solve for the roots and the record times will be anything in between them
        var a = 1;
        var b = -(1 * time);
        var c = distance;
        var delta = b * b - 4 * a * c;
        var maxSeconds = Math.Ceiling((-b + Math.Sqrt(delta)) / 2 * a);
        var minSeconds = Math.Floor((-b - Math.Sqrt(delta)) / 2 * a);
        var numberOfWinningCombinations = maxSeconds - minSeconds - 1;
        return (long) numberOfWinningCombinations;
    }
}

