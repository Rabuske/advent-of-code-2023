using SymbolSet = System.Collections.Generic.HashSet<(Point2D<int> position, char symbol)>;

record PartNumbers(int Number, SymbolSet Symbols);
class Day03 : IDayCommand
{
  public string Execute()
  {
   
    // Parsing shenanigans
    var lines = new FileReader(03).Read().ToList();
    var numbers = new List<PartNumbers>();
    var possibleGears = new List<Point2D<int>>();

    for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
    {
      string currentNumber = "";
      SymbolSet adjacentSymbols = new();
      for (int columnIndex = 0; columnIndex < lines[lineIndex].Length; columnIndex++)
      {
        var character = lines[lineIndex][columnIndex];
        if(char.IsDigit(character))
        {
          currentNumber += character;
          EvaluateAdjacentSymbol(lines, lineIndex-1, adjacentSymbols, columnIndex-1);
          EvaluateAdjacentSymbol(lines, lineIndex-1, adjacentSymbols, columnIndex);
          EvaluateAdjacentSymbol(lines, lineIndex-1, adjacentSymbols, columnIndex+1);
          EvaluateAdjacentSymbol(lines, lineIndex, adjacentSymbols, columnIndex-1);
          EvaluateAdjacentSymbol(lines, lineIndex, adjacentSymbols, columnIndex+1);
          EvaluateAdjacentSymbol(lines, lineIndex+1, adjacentSymbols, columnIndex-1);
          EvaluateAdjacentSymbol(lines, lineIndex+1, adjacentSymbols, columnIndex);
          EvaluateAdjacentSymbol(lines, lineIndex+1, adjacentSymbols, columnIndex+1);
        }
        else
        {
          if(character == '*')
          {
            possibleGears.Add(new Point2D<int>(lineIndex, columnIndex));
          }
          if(!string.IsNullOrEmpty(currentNumber))
          {
            numbers.Add(new (int.Parse(currentNumber), adjacentSymbols));
            currentNumber = "";
            adjacentSymbols = new();
          }
        }
      }    
      if(!string.IsNullOrEmpty(currentNumber))
      {
        numbers.Add(new (int.Parse(currentNumber), adjacentSymbols));
      }
    }

    var partNumbersNextToGears = possibleGears.Select(possibleGear => numbers
                                              .Where(number => number.Symbols.Contains((possibleGear, '*'))))
                                              .Where(numbers => numbers.Count() == 2);

    var gearRatios = partNumbersNextToGears.Select(numbers => numbers.First().Number * numbers.Last().Number);                                   

    var partNumbersSum = numbers.Where(number => number.Symbols.Count > 0).Sum(number => number.Number);    
 
    return $"""
      The sum the part numbers is {partNumbersSum}
      The sum of gear ratios is {gearRatios.Sum()}
      """;
  }

    private static void EvaluateAdjacentSymbol(List<string> lines, int lineIndex, SymbolSet adjacentSymbols, int columnIndex)
    {
      if(lineIndex < 0 || lineIndex >= lines.Count) return;
      if(columnIndex < 0 || columnIndex >= lines[lineIndex].Length) return;
      if (lines[lineIndex][columnIndex] != '.' && !char.IsDigit(lines[lineIndex][columnIndex]))
      {
        adjacentSymbols.Add((new Point2D<int>(lineIndex, columnIndex), lines[lineIndex][columnIndex]));
      }
    }
}