using System.Collections.Immutable;

class Day10Node 
{
  public required char Shape;
  public required Point2D<int> Position;
  public Day10Node? North;
  public Day10Node? South;
  public Day10Node? West;
  public Day10Node? East;

  public Day10Node? Navigate(Day10Node from)
  {
    return Shape switch
    {
      '|' => from == South? North : South,
      '-' => from == West? East : West,
      'L' => from == North? East : North,
      'J' => from == North? West : North,
      '7' => from == South? West : South,
      'F' => from == South? East : South,
      _ => null
    };
  }

  public void FindConnectingPipes(ImmutableDictionary<Point2D<int>, Day10Node> map, Point2D<int> currentPosition)
  {
    switch(Shape)
    {
      case '|':
        South = map.GetValueOrDefault(currentPosition + new Point2D<int>(1, 0));
        North = map.GetValueOrDefault(currentPosition + new Point2D<int>(-1, 0));
        break;
      case '-':
        East = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, 1));
        West = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, -1));
        break;
      case 'L':
        North = map.GetValueOrDefault(currentPosition + new Point2D<int>(-1, 0));
        East = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, 1));
        break;
      case 'J':
        North = map.GetValueOrDefault(currentPosition + new Point2D<int>(-1, 0));
        West = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, -1));
        break;
      case '7':
        South = map.GetValueOrDefault(currentPosition + new Point2D<int>(1, 0));
        West = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, -1));
        break;
      case 'F':
        South = map.GetValueOrDefault(currentPosition + new Point2D<int>(1, 0));
        East = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, 1));
        break;
      case 'S':
        South = map.GetValueOrDefault(currentPosition + new Point2D<int>(1, 0));
        North = map.GetValueOrDefault(currentPosition + new Point2D<int>(-1, 0));
        East = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, 1));
        West = map.GetValueOrDefault(currentPosition + new Point2D<int>(0, -1));
        break;
      default:
        return;
    } 
  }
}

class Day10 : IDayCommand
{
  public string Execute()
  {

    // Parsing shenanigans
    var lines = new FileReader(10).Read().ToList();
    var allNodes = lines.SelectMany((line, indexLine) => line
                           .Select((column, indexColumn) => {
                              var position = new Point2D<int>(indexLine, indexColumn);
                              return KeyValuePair.Create(position,new Day10Node(){Shape = column, Position = position});
                          }));

    var pipes = allNodes.Where(kv => kv.Value.Shape != '.').ToImmutableDictionary();
    var tiles = allNodes.Where(kv => kv.Value.Shape == '.').ToImmutableDictionary();

    // Find all the connecting pipes for 
    foreach (var pair in pipes)
    {
      pair.Value.FindConnectingPipes(pipes, pair.Key);
    }

    // Find Loop
    var startingNode = pipes.Where(kv => kv.Value.Shape == 'S').First().Value;    
    var largestLoop = FindLargestLoop(startingNode, startingNode.North);
    var largestLoopSouth = FindLargestLoop(startingNode, startingNode.South);
    var largestLoopEast = FindLargestLoop(startingNode, startingNode.East);
    var largestLoopWest = FindLargestLoop(startingNode, startingNode.West);
    
    largestLoop = largestLoop.Count > largestLoopSouth.Count ? largestLoop : largestLoopSouth;
    largestLoop = largestLoop.Count > largestLoopEast.Count ? largestLoop : largestLoopEast;
    largestLoop = largestLoop.Count > largestLoopWest.Count ? largestLoop : largestLoopWest;

    AdjustStartingNode(startingNode, largestLoop);
    var isInsideLoopMap = GenerateIsInsideMap(largestLoop, lines.Count, lines[0].Length);
    var tilesInsideCount = isInsideLoopMap.Values.Where(v => v == 'I').Count();
    
    return $"""
    Steps taken to get the farthest is {largestLoop.Count / 2 }
    The number of tiles inside the loop is {tilesInsideCount}
    """;
  }

  private void AdjustStartingNode(Day10Node startingNode, HashSet<Day10Node> largestLoop)
  {
    bool hasSouth = startingNode.South is not null && largestLoop.Contains(startingNode.South) && "|JL".Contains(startingNode.South.Shape);
    bool hasNorth = startingNode.North is not null && largestLoop.Contains(startingNode.North) && "|F7".Contains(startingNode.North.Shape);
    bool hasEast = startingNode.East is not null && largestLoop.Contains(startingNode.East) && "-J7".Contains(startingNode.East.Shape); 
    bool hasWest = startingNode.West is not null && largestLoop.Contains(startingNode.West) && "-LF".Contains(startingNode.West.Shape);

    if(hasSouth && hasNorth) startingNode.Shape = '|';
    if(hasSouth && hasEast) startingNode.Shape = 'F';
    if(hasSouth && hasWest) startingNode.Shape = '7';
    if(hasNorth && hasWest) startingNode.Shape = 'J';
    if(hasNorth && hasEast) startingNode.Shape = 'L';
    if(hasEast && hasWest) startingNode.Shape = '-';
  }

  private Dictionary<Point2D<int>, char> GenerateIsInsideMap(HashSet<Day10Node> largestLoop, int lines, int columns)
  {
    var isInsideLoopMap = new Dictionary<Point2D<int>, char>();
    var largestLoopDictionary = largestLoop.ToDictionary(item => item.Position, item => item);

    var previousBend = ' ';

    for (int lineIndex = 0; lineIndex < lines; lineIndex++)
    {
      var isInside = false;
      for (int columnIndex = 0; columnIndex < columns; columnIndex++)
      {
        var position = new Point2D<int>(lineIndex, columnIndex);
        if(largestLoopDictionary.ContainsKey(position))
        {
          var loopNode = largestLoopDictionary[position];
          isInsideLoopMap.Add(position, loopNode.Shape);
          switch (loopNode.Shape)
          {
            case '|':
              isInside = !isInside;
              break;
            case 'L':
              previousBend = 'L';
              break;
            case 'J':
              if(previousBend == 'F') isInside = !isInside;
              break;
            case '7':
              if(previousBend == 'L') isInside = !isInside;
              break;
            case 'F':
              previousBend = 'F';
              break;
            default:
              break;
          }
        }
        else
        {
          isInsideLoopMap.Add(position, isInside? 'I' : 'O');
        }
      }
    }

    return isInsideLoopMap;
  }

  public HashSet<Day10Node> FindLargestLoop(Day10Node startingNode, Day10Node? nextNode)
  {
    var visited = new HashSet<Day10Node>();
    if(nextNode == null) return visited;
    FindLoop(startingNode, nextNode, visited);
    if(visited.Contains(startingNode))
    {
      return visited;
    }
    return new HashSet<Day10Node>();
  }

  public void FindLoop(Day10Node previousNode, Day10Node? currentNode, HashSet<Day10Node> visited)
  {
    if(currentNode == null)
    {
      return;
    }

    if(visited.Contains(currentNode))
    {
      return;
    }

    visited.Add(currentNode);

    var toNavigate = currentNode.Navigate(previousNode);
    FindLoop(currentNode, toNavigate, visited);
  }
}