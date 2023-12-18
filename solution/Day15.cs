

using System.Text;

class Day15 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(15).Read().ToList();
    var steps = lines.SelectMany(line => line.Split(',')).ToList();
    var hashes = steps.Select(CalculateHash).ToList();
    var boxes = GenerateBoxes(steps);

    var focusingPower = boxes.SelectMany((box, boxIndex) => box.Select((lens, lensIndex) => (boxIndex + 1) * (lensIndex + 1) * lens.value)).ToList();

    return $"""
      The hash is {hashes.Sum()}
      The total focusing power is {focusingPower.Sum()}
      """;
  }

  private List<LinkedList<(string label, int value)>> GenerateBoxes(List<string> steps)
  {
    var boxes = Enumerable.Range(0, 256).Select(_ => new LinkedList<(string label, int value)>()).ToList();

    for (int stepIndex = 0; stepIndex < steps.Count; stepIndex++)
    {
      var step = steps[stepIndex];
      if (step.Contains('='))
      {
        var split = step.Split("=");
        var label = split[0];
        var box = boxes[CalculateHash(label)];
        var number = int.Parse(split[1]);
        if(!ReplaceFromBox(label, number, box))
        {
          box.AddLast((label, number));
        }
      }
      else
      {
        var label = step.Trim().Replace("-","");
        var box = boxes[CalculateHash(label)];
        RemoveFromBox(label, box);
      }
    };

    return boxes;
  }

  private static bool RemoveFromBox(string label, LinkedList<(string label, int value)> box)
  {
    var currentEntry = box.Where(c => c.label == label).FirstOrDefault(("", -1));
    if (currentEntry.Item1 != "")
    {
      box.Remove(currentEntry);
      return true;
    }
    return false;
  }

  private static bool ReplaceFromBox(string label, int number, LinkedList<(string label, int value)> box)
  {
    var currentEntry = box.Where(c => c.label == label).FirstOrDefault(("", -1));
    if (currentEntry.Item1 != "")
    {
      var node = box.Find(currentEntry);
      if(node is null) return false;
      box.AddAfter(node, (label, number));
      box.Remove(node);
      return true;
    }
    return false;
  }  

  public int CalculateHash(string input)
  {
    return input.Select(c => (int)c).Aggregate(0, (current, next) => ((current + next) * 17) % 256);
  }
}