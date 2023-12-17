class Day13 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(13).Read().ToList();

    var patterns = GeneratePatterns(lines);
    var reflectionPoints = patterns.Select(p => FindReflectionPoint(p, new())).ToList();
    var reflectionPointsWithSmudges = patterns.Select((pattern, index) => FindReflectionPointsWithSmudges(pattern, reflectionPoints[index])).ToList();

    var totalPart01 = reflectionPoints.Sum(point => point.horizontal * 100 + point.vertical);
    var totalPart02 = reflectionPointsWithSmudges.Sum(point => point.horizontal * 100 + point.vertical);

    return $"""
      The summary of the reflections is {totalPart01}
      The summary of all reflections after the smudges is {totalPart02}
      """;
  }

  private (int vertical, int horizontal) FindReflectionPointsWithSmudges(List<string> pattern, (int, int) previousPoint)
  {
    var result = new List<int>();
    var newPattern = pattern.Select(line => line.ToList()).ToList();
    for (int line = 0; line < pattern.Count; line++)
    {
      for (int column = 0; column < pattern[line].Length; column++)
      {
        // Change single char
        newPattern[line][column] = newPattern[line][column] == '.' ? '#' : '.';
        var reflectionPoint = FindReflectionPoint(newPattern.Select(np => string.Join("", np)).ToList(), previousPoint);
        if (reflectionPoint.horizontal != 0 || reflectionPoint.vertical != 0)
        {
          return reflectionPoint;
        }
        // Restore the entire Line
        newPattern[line] = pattern[line].ToList();
      }
    }
    return new();
  }

  private List<string> RotatePattern(List<string> pattern)
  {
    var rotatedPattern = new List<string>();
    for (int column = 0; column < pattern[0].Length; column++)
    {
      var result = string.Join("", pattern.Select(p => p[column]));
      rotatedPattern.Add(result);
    }
    return rotatedPattern;
  }

  private (int vertical, int horizontal) FindReflectionPoint(List<string> pattern, (int vertical, int horizontal) pointsToIgnore)
  {
    // Starts with the vertical one
    var vertical = CalculateReflectionPoint(pattern, pointsToIgnore.vertical);
    var horizontal = CalculateReflectionPoint(RotatePattern(pattern), pointsToIgnore.horizontal);
    return (vertical, horizontal);
  }

  private int CalculateReflectionPoint(List<string> pattern, int pointToIgnore)
  {
    var points = Enumerable.Range(1, pattern[0].Length - 1).ToList();
    points.Remove(pointToIgnore);
    foreach (var line in pattern)
    {
      points = FindMirrorPointsInLine(line, points);
    }
    return points.FirstOrDefault();
  }

  private List<List<string>> GeneratePatterns(List<string> lines)
  {
    List<List<string>> patterns = new();

    var currentList = new List<string>();

    foreach (var line in lines)
    {
      if (string.IsNullOrEmpty(line))
      {
        patterns.Add(currentList);
        currentList = new();
        continue;
      }

      currentList.Add(line);
    }

    if (currentList.Count > 0)
    {
      patterns.Add(currentList);
    }
    return patterns;
  }

  public List<int> FindMirrorPointsInLine(string line, List<int> pointsToCheck)
  {
    var mirrorPoints = new List<int>();

    foreach (var i in pointsToCheck)
    {
      var distanceToCheck = Math.Min(i, line.Length - i);
      var string1 = line[(i - distanceToCheck)..i];
      var string2 = string.Join("", line[i..(i + distanceToCheck)].Reverse());
      if (string1 == string2)
      {
        mirrorPoints.Add(i);
      }
    }
    return mirrorPoints;
  }
}