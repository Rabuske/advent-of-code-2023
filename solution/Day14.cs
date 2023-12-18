

using System.Text;

class Day14 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(14).Read().ToList();
    var map = lines.SelectMany((line, lineIndex) =>
        line.Select((tile, columnIndex) => KeyValuePair.Create((lineIndex, columnIndex), tile)))
      .ToDictionary(kv => kv.Key, kv => kv.Value);

    var mapPart01 = map.Select(kv => kv).ToDictionary(kv => kv.Key, kv => kv.Value);  // Modifies a copy of the dictionary for part01

    RollTheBouldersNorth(mapPart01); // Modifies a copy of the dictionary for part01
    var mapPart02 = RunTheCycle(map, 1000000000);

    var totalLoadPart01 = mapPart01.Where(kv => kv.Value == 'O').Select(kv => lines.Count - kv.Key.lineIndex).ToList();
    var totalLoadPart02 = mapPart02.Where(kv => kv.Value == 'O').Select(kv => lines.Count - kv.Key.lineIndex).ToList();

    return $"""
      The total load is {totalLoadPart01.Sum()}
      The total load after cycling is {totalLoadPart02.Sum()}
      """;
  }

  private Dictionary<(int lineIndex, int columnIndex), char> RunTheCycle(Dictionary<(int lineIndex, int columnIndex), char> map, int numberOfTimes)
  {
    Dictionary<string, (Dictionary<(int lineIndex, int columnIndex), char> map, int index)> memory = new();
    var tempMap = map.ToDictionary(kv => kv.Key, kv => kv.Value);
    for (int i = 0; i < numberOfTimes; i++)
    {
      var inputRepresentation = GenerateStringRepresentation(tempMap);
      if (memory.ContainsKey(inputRepresentation))
      {
        (tempMap, var indexOfStartOfLoop) = memory[inputRepresentation];
        var loopSize = i - indexOfStartOfLoop;
        var incrementsNeeded = (numberOfTimes - i) % loopSize;
        for (int increment = 0; increment < incrementsNeeded - 1; increment++)
        {
          tempMap = memory[GenerateStringRepresentation(tempMap)].map;
          var maxLine = tempMap.Max(kv => kv.Key.lineIndex);
        }
        return tempMap;
      }
      else
      {
        RollTheBouldersNorth(tempMap);
        RollTheBouldersWest(tempMap);
        RollTheBouldersSouth(tempMap);
        RollTheBouldersEast(tempMap);
        memory.Add(inputRepresentation, (tempMap.ToDictionary(kv => kv.Key, kv => kv.Value), i));
      }
    }
    return tempMap;
  }

  private string GenerateStringRepresentation(Dictionary<(int lineIndex, int columnIndex), char> map)
  {
    StringBuilder sb = new();
    foreach (var entry in map)
    {
      sb.Append(entry.Key.ToString() + '|' + entry.Value + '|');
    }
    return sb.ToString();
  }

  private void RollTheBouldersNorth(Dictionary<(int, int), char> map)
  {
    var maxLine = map.Max(kv => kv.Key.Item1);
    var maxColumn = map.Max(kv => kv.Key.Item2);

    for (int lineIndex = 0; lineIndex <= maxLine; lineIndex++)
    {
      for (int columnIndex = maxColumn; columnIndex >= 0; columnIndex--)
      {
        if (map[(lineIndex, columnIndex)] != 'O')
        {
          continue;
        }
        var currentLine = lineIndex;
        for (int lineToRoll = lineIndex - 1; lineToRoll >= 0; lineToRoll--)
        {
          if (map[(lineToRoll, columnIndex)] == '.')
          {
            map[(lineToRoll, columnIndex)] = 'O';
            map[(currentLine, columnIndex)] = '.';
            currentLine = lineToRoll;
          }
          else break;
        }
      }
    }
  }
  private void RollTheBouldersSouth(Dictionary<(int, int), char> map)
  {
    var maxLine = map.Max(kv => kv.Key.Item1);
    var maxColumn = map.Max(kv => kv.Key.Item2);

    for (int lineIndex = maxLine; lineIndex >= 0; lineIndex--)
    {
      for (int columnIndex = maxColumn; columnIndex >= 0; columnIndex--)
      {
        if (map[(lineIndex, columnIndex)] != 'O')
        {
          continue;
        }
        var currentLine = lineIndex;
        for (int lineToRoll = lineIndex + 1; lineToRoll <= maxLine; lineToRoll++)
        {
          if (map[(lineToRoll, columnIndex)] == '.')
          {
            map[(lineToRoll, columnIndex)] = 'O';
            map[(currentLine, columnIndex)] = '.';
            currentLine = lineToRoll;
          }
          else break;
        }
      }
    }
  }

  private void RollTheBouldersWest(Dictionary<(int, int), char> map)
  {
    var maxLine = map.Max(kv => kv.Key.Item1);
    var maxColumn = map.Max(kv => kv.Key.Item2);

    for (int lineIndex = maxLine; lineIndex >= 0; lineIndex--)
    {
      for (int columnIndex = 0; columnIndex <= maxColumn; columnIndex++)
      {
        if (map[(lineIndex, columnIndex)] != 'O')
        {
          continue;
        }
        var currentColumn = columnIndex;
        for (int columnToRoll = columnIndex - 1; columnToRoll >= 0; columnToRoll--)
        {
          if (map[(lineIndex, columnToRoll)] == '.')
          {
            map[(lineIndex, columnToRoll)] = 'O';
            map[(lineIndex, currentColumn)] = '.';
            currentColumn = columnToRoll;
          }
          else break;
        }
      }
    }
  }

  private void RollTheBouldersEast(Dictionary<(int, int), char> map)
  {
    var maxLine = map.Max(kv => kv.Key.Item1);
    var maxColumn = map.Max(kv => kv.Key.Item2);

    for (int lineIndex = maxLine; lineIndex >= 0; lineIndex--)
    {
      for (int columnIndex = maxColumn; columnIndex >= 0; columnIndex--)
      {
        if (map[(lineIndex, columnIndex)] != 'O')
        {
          continue;
        }
        var currentColumn = columnIndex;
        for (int columnToRoll = columnIndex + 1; columnToRoll <= maxColumn; columnToRoll++)
        {
          if (map[(lineIndex, columnToRoll)] == '.')
          {
            map[(lineIndex, columnToRoll)] = 'O';
            map[(lineIndex, currentColumn)] = '.';
            currentColumn = columnToRoll;
          }
          else break;
        }
      }
    }
  }
}