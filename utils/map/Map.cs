class Map<T>
{
  public HashSet<Node<T>> Nodes { get; init; } = new();

  public Map(IEnumerable<IEnumerable<Node<T>>> map, bool considerDiagonals = false)
  {
    // Set the adjacent ones
    var mapAsArray = map.Select(line => line.ToArray()).ToArray();
    for (int i = 0; i < mapAsArray.Count(); i++)
    {
      for (int j = 0; j < mapAsArray[i].Count(); j++)
      {
        var currentNode = mapAsArray[i][j];
        Nodes.Add(currentNode);
        if (i - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i - 1][j]);
        if (i + 1 < map.Count()) currentNode.AdjacentNodes.Add(mapAsArray[i + 1][j]);
        if (j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i][j - 1]);
        if (j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i][j + 1]);

        if (considerDiagonals)
        {
          if (i - 1 >= 0 && j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i - 1][j - 1]);
          if (i - 1 >= 0 && j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i - 1][j + 1]);
          if (i + 1 < map.Count() && j - 1 >= 0) currentNode.AdjacentNodes.Add(mapAsArray[i + 1][j - 1]);
          if (i + 1 < map.Count() && j + 1 < mapAsArray[i].Count()) currentNode.AdjacentNodes.Add(mapAsArray[i + 1][j + 1]);
        }
      }
    }
  }

  public List<Node<T>> GetOptimalPath(Node<T> start, Node<T> end)
  {
    var alreadyVisitedNodes = new HashSet<Node<T>>();
    var paths = new PriorityQueue<(int costSum, Node<T>[] path), int>();

    paths.Enqueue((0, new Node<T>[] { start }), 0);

    while (paths.Count > 0)
    {
      var currentPath = paths.Dequeue();
      var currentNode = currentPath.path.Last();
      if (currentNode == end)
      {
        return currentPath.path.ToList();
      }

      if (alreadyVisitedNodes.Contains(currentNode))
      {
        continue;
      }
      alreadyVisitedNodes.Add(currentNode);

      var nextInLine = currentNode.AdjacentNodes.Where(n => !alreadyVisitedNodes.Contains(n));
      var withCost = nextInLine.Select(adjNode =>
      {
        var newCost = currentPath.costSum + currentNode.GetTravelCost(currentNode, adjNode);
        return ((newCost, currentPath.path.Append(adjNode).ToArray()), newCost);
      });

      paths.EnqueueRange(withCost);
    }

    return new(); // No solution
   }    
}