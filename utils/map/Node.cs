class Node<T>
{
  public T Value { get; set; }
  public Dictionary<NodeDirection, Node<T>> AdjacentNodes { get; init; } = new();
  public Func<Node<T>, Node<T>, long> GetTravelCost { get; set; } = DefaultGetTravelCost;
  public Node(T value)
  {
    Value = value;
  }

  private static long DefaultGetTravelCost(Node<T> current, Node<T> adjacent)
  {
    return 1L;
  }
  private int ValueAsInt => Convert.ToInt32(Value);

}