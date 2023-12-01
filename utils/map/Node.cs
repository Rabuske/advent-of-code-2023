class Node<T>
{
  public T Value { get; set; }
  public HashSet<Node<T>> AdjacentNodes { get; init; } = new();
  public Func<Node<T>, Node<T>, int> GetTravelCost { get; set; } = DefaultGetTravelCost;
  public Node(T value)
  {
    Value = value;
  }

  private static int DefaultGetTravelCost(Node<T> current, Node<T> adjacent)
  {
    return 1;
  }
  private int ValueAsInt => Convert.ToInt32(Value);

}