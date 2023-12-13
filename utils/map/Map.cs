enum NodeDirection 
{
  NORTH = 'N',
  SOUTH = 'S',
  WEST = 'W',
  EAST = 'E',
  NORTH_WEST = 'F',
  NORTH_EAST = '7',
  SOUTH_WEST = 'L',
  SOUTH_EAST = 'J',
}

class Map<T>
{
  public HashSet<Node<T>> Nodes { get; init; } = new();

  public Map(IEnumerable<IEnumerable<Node<T>>> map, bool considerDiagonals = false)
  {
    // Set the adjacent ones
    var mapAsArray = map.Select(line => line.ToArray()).ToArray();
    for (int lineIndex = 0; lineIndex < mapAsArray.Count(); lineIndex++)
    {
      for (int columnIndex = 0; columnIndex < mapAsArray[lineIndex].Count(); columnIndex++)
      {
        var currentNode = mapAsArray[lineIndex][columnIndex];
        Nodes.Add(currentNode);
        if (lineIndex - 1 >= 0) currentNode.AdjacentNodes.Add(NodeDirection.NORTH, mapAsArray[lineIndex - 1][columnIndex]);
        if (lineIndex + 1 < map.Count()) currentNode.AdjacentNodes.Add(NodeDirection.SOUTH, mapAsArray[lineIndex + 1][columnIndex]);
        if (columnIndex - 1 >= 0) currentNode.AdjacentNodes.Add(NodeDirection.WEST, mapAsArray[lineIndex][columnIndex - 1]);
        if (columnIndex + 1 < mapAsArray[lineIndex].Count()) currentNode.AdjacentNodes.Add(NodeDirection.EAST, mapAsArray[lineIndex][columnIndex + 1]);

        if (considerDiagonals)
        {
          if (lineIndex - 1 >= 0 && columnIndex - 1 >= 0) currentNode.AdjacentNodes.Add(NodeDirection.NORTH_WEST, mapAsArray[lineIndex - 1][columnIndex - 1]);
          if (lineIndex - 1 >= 0 && columnIndex + 1 < mapAsArray[lineIndex].Count()) currentNode.AdjacentNodes.Add(NodeDirection.SOUTH_EAST, mapAsArray[lineIndex - 1][columnIndex + 1]);
          if (lineIndex + 1 < map.Count() && columnIndex - 1 >= 0) currentNode.AdjacentNodes.Add(NodeDirection.SOUTH_WEST, mapAsArray[lineIndex + 1][columnIndex - 1]);
          if (lineIndex + 1 < map.Count() && columnIndex + 1 < mapAsArray[lineIndex].Count()) currentNode.AdjacentNodes.Add(NodeDirection.SOUTH_EAST, mapAsArray[lineIndex + 1][columnIndex + 1]);
        }
      }
    }
  }

  public (List<Node<T>> path, long cost) GetOptimalPath(Node<T> start, Node<T> end)
  {
    var alreadyVisitedNodes = new HashSet<Node<T>>();
    var paths = new PriorityQueue<(long costSum, Node<T>[] path), long>();

    paths.Enqueue((0, new Node<T>[] { start }), 0);

    while (paths.Count > 0)
    {
      var currentPath = paths.Dequeue();
      var currentNode = currentPath.path.Last();
      if (currentNode == end)
      {
        return (currentPath.path.ToList(), currentPath.costSum);
      }
      if (alreadyVisitedNodes.Contains(currentNode))
      {
        continue;
      }
      alreadyVisitedNodes.Add(currentNode);

      var nextInLine = currentNode.AdjacentNodes.Where(n => !alreadyVisitedNodes.Contains(n.Value));
      var withCost = nextInLine.Select(adjNode =>
      {
        var newCost = currentPath.costSum + currentNode.GetTravelCost(currentNode, adjNode.Value);
        return ((newCost, currentPath.path.Append(adjNode.Value).ToArray()), newCost);
      });

      paths.EnqueueRange(withCost);
    }

    return new(); // No solution
   }    
}