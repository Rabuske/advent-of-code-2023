class Day17 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(17).Read().ToList();

    var nodes = lines.Select(line => line.Select(c => new Node<int>((int)Char.GetNumericValue(c)){
      GetTravelCost = (Node<int> from, Node<int> to) => to.Value
    }));
    var map = new Map<int>(nodes);    

    var leastHeatCost = GetOptimalHeatLoss(map.Nodes.First(), map.Nodes.Last(), 0, 3);
    var leastHeatCostUltraCrucibles = GetOptimalHeatLoss(map.Nodes.First(), map.Nodes.Last(), 4, 10);

    return $"""
      The least heat cost is {leastHeatCost}
      The least heat cost with ultra crucibles is {leastHeatCostUltraCrucibles}
      """;
  }

  public long GetOptimalHeatLoss(Node<int> start, Node<int> end, int minConsecutive, int maxConsecutive)
  {
    var alreadyVisitedNodes = new HashSet<(string, Node<int>)>();
    var paths = new PriorityQueue<(long costSum, (Node<int> node, NodeDirection direction)[] path), long>();

    paths.Enqueue((0, new (Node<int> node, NodeDirection direction)[] { (start, NodeDirection.NORTH_WEST) }), 0);

    while (paths.Count > 0)
    {
      var currentPath = paths.Dequeue();
      var currentNode = currentPath.path.Last();
      if (currentNode.node == end)
      {
        return currentPath.costSum;
      }
      var lastDirections = string.Join("", currentPath.path[^(Math.Min(currentPath.path.Length, maxConsecutive))..].Select(d => d.direction.ToString()));
      if (alreadyVisitedNodes.Contains((lastDirections, currentNode.node)))
      {
        continue;
      }
      alreadyVisitedNodes.Add((lastDirections, currentNode.node));

      // Cannot go back
      var nextInLine = currentNode.node.AdjacentNodes.Where(c => currentPath.path.Length <= 2 || c.Value != currentPath.path[^2].node);
      
      // Removes the direction in case the last maxConsecutive moved in the same direction
      if(currentPath.path.Length >= maxConsecutive)
      {
        var directions = currentPath.path[^maxConsecutive..].Select(c => c.direction).Distinct();
        if(directions.Count() == 1)
        {
          var directionToAvoid = directions.First();
          nextInLine = nextInLine.Where(c => c.Key != directionToAvoid);
        }
      } 

      // Enforce the same direction in case it has not moved in a minDirectionWay
      if(minConsecutive > 0)
      {
        var minDirections = currentPath.path[^(Math.Min(currentPath.path.Length, minConsecutive))..].Select(d => d.direction).Distinct();
        if(minDirections.Count() > 1)
        {
          nextInLine = nextInLine.Where(d => d.Key == currentNode.direction);
        }
      }

      var withCost = nextInLine.Select(adjNode =>
      {
        var newCost = currentPath.costSum + currentNode.node.GetTravelCost(currentNode.node, adjNode.Value);
        return ((newCost, currentPath.path.Append((adjNode.Value, adjNode.Key)).ToArray()), newCost);
      });
      
      paths.EnqueueRange(withCost);
    }

    return new(); // No solution
   }    
}  