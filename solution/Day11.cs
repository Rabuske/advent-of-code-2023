class Day11 : IDayCommand
{
  public string Execute()
  {

    // Parsing shenanigans
    var lines = new FileReader(11).Read().ToList();
    
    Console.WriteLine("Go take a nap. The solution takes a while to calculate 🤡");

    var linesThatExpand = lines.Select((line, index) => (index, line))
                               .Where(lineAndIndex => lineAndIndex.line
                               .All(c => c == '.'))
                               .Select(lineAndIndex => lineAndIndex.index)
                               .ToHashSet();

    var columnsThatExpand = lines[0].Select((column, index) => index)
                                    .Select(index => (index, lines.Select(line => line[index])))
                                    .Where(indexAndChars => indexAndChars.Item2.All(c => c == '.'))
                                    .Select(indexAndChars => indexAndChars.index)
                                    .ToHashSet();

    var nodeMapPart01 = GenerateNodeMap(lines, linesThatExpand, columnsThatExpand, 2);
    var nodeMapPart02 = GenerateNodeMap(lines, linesThatExpand, columnsThatExpand, 1000000);

    var map = new Map<(char id, Point2D<int> position)>(nodeMapPart02);

    var galaxiesPart01 = nodeMapPart01.SelectMany(line => line.Where(node => node.Value.id == '#')).ToList();
    var galaxyPairsPart01 = ComputePairs(map, galaxiesPart01);

    var galaxiesPart02 = nodeMapPart02.SelectMany(line => line.Where(node => node.Value.id == '#')).ToList();
    var galaxyPairsPart02 = ComputePairs(map, galaxiesPart02);

    return $"""
      The smaller distance between two galaxies is with expansion rate 2 is {galaxyPairsPart01.Values.Sum()}
      The smaller distance between two galaxies is with expansion rate 1000000 is {galaxyPairsPart02.Values.Sum()}
      """;
  }

  private static Dictionary<(Node<(char id, Point2D<int> position)> Node1, Node<(char id, Point2D<int> position)> Node2), long> ComputePairs(Map<(char id, Point2D<int> position)> map, List<Node<(char id, Point2D<int> position)>> galaxies)
  {
    var galaxyPairs = galaxies.SelectMany((node1, index1) => galaxies.Skip(index1 + 1).Select(node2 => (Node1: node1, Node2: node2))).ToDictionary(k => k, v => 0L);

    foreach (var pair in galaxyPairs)
    {
      if (pair.Value != 0)
      {
        continue;
      }
      var optimalPath = map.GetOptimalPath(pair.Key.Node1, pair.Key.Node2);
      // If there was already paths found for other galaxies, avoid calculating them
      for (int i = 0; i < optimalPath.path.Count - 1; i++)
      {
        var cost = 0L;
        if (optimalPath.path[i].Value.id != '#') continue;
        for (int j = i + 1; j < optimalPath.path.Count; j++)
        {
          cost += optimalPath.path[j - 1].GetTravelCost(optimalPath.path[j - 1], optimalPath.path[j]);
          if (galaxyPairs.ContainsKey((optimalPath.path[i], optimalPath.path[j])))
          {
            galaxyPairs[(optimalPath.path[i], optimalPath.path[j])] = cost;
          }
        }
      }
    }

    return galaxyPairs;
  }

  private static List<List<Node<(char id, Point2D<int> position)>>> GenerateNodeMap(List<string> lines, HashSet<int> linesThatExpand, HashSet<int> columnsThatExpand, int expansionRate)
  {
    return lines.Select((line, lineIndex) => line.Select((column, columnIndex) =>
    {
      var node = new Node<(char id, Point2D<int> position)>((column, new Point2D<int>(lineIndex, columnIndex)))
      {
        GetTravelCost = (Node<(char id, Point2D<int> position)> n1, Node<(char id, Point2D<int> position)> n2) =>
            {
              if (linesThatExpand.Contains(n2.Value.position.x))
              {
                if ((n1.AdjacentNodes.ContainsKey(NodeDirection.SOUTH) && n2 == n1.AdjacentNodes[NodeDirection.SOUTH])
                    || (n1.AdjacentNodes.ContainsKey(NodeDirection.NORTH) && n2 == n1.AdjacentNodes[NodeDirection.NORTH]))
                {
                  return expansionRate;
                }
              }
              if (columnsThatExpand.Contains(n2.Value.position.y))
              {
                if ((n1.AdjacentNodes.ContainsKey(NodeDirection.EAST) && n2 == n1.AdjacentNodes[NodeDirection.EAST])
                    || (n1.AdjacentNodes.ContainsKey(NodeDirection.WEST) && n2 == n1.AdjacentNodes[NodeDirection.WEST]))
                {
                  return expansionRate;
                }
              }
              return 1;
            }
      };
      return node;
    }).ToList()).ToList();
  }
}