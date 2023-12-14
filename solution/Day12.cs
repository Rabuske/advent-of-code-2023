
using System.Text.RegularExpressions;

class Day12 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(12).Read().ToList();

    var springsAndConditionRecordsPart01 = lines.Select(line =>
    {
      var split = line.Split(" ");
      var springs = split[0];
      var records = split[1].Split(',').Select(int.Parse).ToList();
      return (springs, records);
    });

    var springsAndConditionRecordsPart02 = springsAndConditionRecordsPart01.Select(spRec => (
      springs: string.Join("", Enumerable.Repeat(spRec.springs, 5)), 
      records: Enumerable.Repeat(spRec.records, 5).SelectMany(i => i).ToList()
    )); 

    var numberOfValidCombinationsPart01 = springsAndConditionRecordsPart01.Select(spRec => ProcessSprings(spRec.springs, spRec.records));
    var numberOfValidCombinationsPart02 = springsAndConditionRecordsPart02.Select(spRec => ProcessSprings(spRec.springs, spRec.records));

    return $"""
      The sum of all permutations is {1}
      The sum of all permutations is when unfolded is {numberOfValidCombinationsPart02.Sum()}
      """;
  }

  private int ProcessSprings(string springs, List<int> records)
  {
    // First, generate all the possible combinations of spring
    var springPermutations = new List<string>{ " " };
    foreach(var spring in springs)
    {
      springPermutations = springPermutations.SelectMany(permutation => {
        var newPermutations = new List<string>();
        if(spring == '?')
        {
          newPermutations.Add(permutation + '#');
          newPermutations.Add(permutation + '.');
        }
        else 
        {
          newPermutations.Add(permutation + spring);
        }
        return newPermutations;
      }).ToList();
    }
    
    // Generate a Regex based on the records
    string regexString = $@"^[.]*{string.Join("", records.Select(record => string.Join("", Enumerable.Repeat('#', record)) + @"[.]+"))}";
    regexString = regexString[..^1] + "*$";

    var regex = new Regex(regexString);
    return springPermutations.Where(permutation => regex.IsMatch(permutation.Trim())).Count();
  }
}