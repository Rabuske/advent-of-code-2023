using System.Text.RegularExpressions;

class Day12 : IDayCommand
{
  private static Dictionary<string, long> Memory = new();

  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(12).Read().ToList();

    var springsAndConditionRecordsPart01 = lines.Select(line =>
    {
      var split = line.Split(" ");
      var springs = split[0];
      var records = split[1].Split(',').Select(sequence => int.Parse(sequence)).ToArray();
      return (springs, records);
    });

    var springsAndConditionRecordsPart02 = springsAndConditionRecordsPart01.Select(spRec => {
      var multipleSprings = string.Join("?", Enumerable.Repeat(spRec.springs, 5));
      var multipleRecords = Enumerable.Repeat(spRec.records, 5).SelectMany(i => i).ToArray();
      return (springs: multipleSprings, records: multipleRecords);
    }).ToList();

    var numberOfValidCombinationsPart01 = springsAndConditionRecordsPart01.Select(spRec =>
      CalculateNumberOfAllowedConfigurations(Regex.Replace(spRec.springs, @"\.+", ".") + '.', spRec.records)).ToList();
    var numberOfValidCombinationsPart02 = springsAndConditionRecordsPart02.Select(spRec =>
      CalculateNumberOfAllowedConfigurations(Regex.Replace(spRec.springs, @"\.+", ".") + '.', spRec.records)).ToList();


    return $"""
      The sum of all permutations is {numberOfValidCombinationsPart01.Sum()}
      The sum of all permutations is when unfolded is {numberOfValidCombinationsPart02.Sum()}
      """;
  }

  private long CalculateNumberOfAllowedConfigurations(string springs, int[] groups)
  {
    var stringRepresentation = springs + string.Join(",", groups);
    if (Memory.ContainsKey(stringRepresentation))
    {
      return Memory[stringRepresentation];
    }

    if (springs.Length == 0)
    {
      if (groups.Length == 0)
      {
        Memory.Add(stringRepresentation, 1);
        return 1;
      }
      else
      {
        Memory.Add(stringRepresentation, 0);
        return 0;
      }
    }

    if (groups.Length == 0)
    {
      if (springs.All(s => s == '.' || s == '?'))
      {
        Memory.Add(stringRepresentation, 1);
        return 1;
      }
      else
      {
        Memory.Add(stringRepresentation, 0);
        return 0;
      }
    }

    if (springs[0] == '.')
    {
      var result = CalculateNumberOfAllowedConfigurations(springs[1..], groups);
      Memory.Add(stringRepresentation, result);
      return result;
    }

    var numberOfSprings = groups[0];
    if (springs[0] == '#')
    {
      if (numberOfSprings + 1 > springs.Length)
      {
        Memory.Add(stringRepresentation, 0);
        return 0;
      }
      if (springs[0..numberOfSprings].All(s => s != '.') && springs[numberOfSprings] != '#')
      {
        var result = CalculateNumberOfAllowedConfigurations(springs[(numberOfSprings + 1)..], groups[1..]);
        Memory.Add(stringRepresentation, result);
        return result;
      }
      else
      {
        Memory.Add(stringRepresentation, 0);
        return 0;
      }
    }

    if (springs[0] == '?')
    {
      var result = CalculateNumberOfAllowedConfigurations('.' + springs[1..], groups) +
                   CalculateNumberOfAllowedConfigurations('#' + springs[1..], groups);
      Memory.Add(stringRepresentation, result);
      return result;
    }

    Memory.Add(stringRepresentation, 0);
    return 0;
  }
}