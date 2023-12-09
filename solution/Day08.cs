
class Day08 : IDayCommand
{
  public string Execute()
  {

    // Parsing shenanigans
    var lines = new FileReader(08).Read().ToList();
    var instructions = lines[0].ToCharArray();

    var map = lines.Skip(2).Select(line =>
    {
      var parts = line.Split("=");
      var key = parts[0].Trim();
      var pairs = parts[1].Trim().Replace("(", "").Replace(")", "").Replace(",", "").Split(" ");
      return KeyValuePair.Create(key, (left: pairs[0], right: pairs[1]));
    }).ToDictionary(k => k.Key, v => v.Value);

    int stepsPart01 = NavigatePart01(instructions, map);
    long stepsPart02 = NavigatePart02(instructions, map);

    return $"""
    The number of steps to reach "ZZZ" is {stepsPart01}
    The number of steps to reach all end nodes at the same time is {stepsPart02}
    """;
  }

  private static int NavigatePart01(char[] instructions, Dictionary<string, (string left, string right)> map)
  {
    var currentNode = "AAA";
    var steps = 0;
    var instructionIndex = 0;

    while (currentNode != "ZZZ")
    {
      steps += 1;
      currentNode = instructions[instructionIndex] == 'L' ? map[currentNode].left : map[currentNode].right;
      instructionIndex = (instructionIndex + 1) % instructions.Length;
    }

    return steps;
  }  

  private long NavigatePart02(char[] instructions, Dictionary<string, (string left, string right)> map)
  {
    // Calculates how long it takes for each node to reach the end and them calculates when the cycles will match using LCM 
    var starterNodes = map.Keys.Where(k => k.EndsWith("A")).ToList();
    var stepsToReachEnd = starterNodes.Select(node =>
    {
      var currentNode = node;
      var steps = 0L;
      var instructionIndex = 0;

      while (!currentNode.EndsWith("Z"))
      {
        steps += 1;
        currentNode = instructions[instructionIndex] == 'L' ? map[currentNode].left : map[currentNode].right;
        instructionIndex = (instructionIndex + 1) % instructions.Length;
      }
      return steps;
    }).ToList();

    var stepsPart02 = FindLcm(stepsToReachEnd);
    return stepsPart02;
  }

  private long Gcd(long a, long b)
  {
    if (b == 0) return a;
    return Gcd(b, a % b);
  }
  
  long FindLcm(List<long> input)
  {
    long answer = input[0];

    // ans contains LCM of arr[0], ..arr[i]
    // after i'th iteration,
    for (int i = 1; i < input.Count; i++)
    {
      answer = input[i] * answer / Gcd(input[i], answer);
    }
    return answer;
  }
}
