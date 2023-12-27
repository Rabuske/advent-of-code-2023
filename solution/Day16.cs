

using System.Text;

record Beam(Point2D<int> Position, Point2D<int> Direction);

interface Tile
{
  public List<Beam> AdvanceBeam(Beam beam);
}

class EmptySpace : Tile
{
  public List<Beam> AdvanceBeam(Beam beam)
  {
    // Just continue in the direction
    var newPosition = beam.Position + beam.Direction;
    return new List<Beam> { beam with { Position = newPosition } };
  }
}

class HorizontalSplitter : Tile
{
  public List<Beam> AdvanceBeam(Beam beam)
  {
    // If the beam is moving right or left, just treat this tile as empty space
    if (beam.Direction.x != 0)
    {
      return new EmptySpace().AdvanceBeam(beam);
    }
    // Otherwise, they move into two different directions
    return new List<Beam>{
       beam with { Direction = new Point2D<int>(1, 0) },
       beam with { Direction = new Point2D<int>(-1, 0) },
      };
  }
}

class VerticalSplitter : Tile
{
  public List<Beam> AdvanceBeam(Beam beam)
  {
    // If the beam is moving up or down, just treat this tile as empty space
    if (beam.Direction.y != 0)
    {
      return new EmptySpace().AdvanceBeam(beam);
    }
    // Otherwise, they move into two different directions
    return new List<Beam>{
      beam with { Direction = new Point2D<int>(0, 1) },
      beam with { Direction = new Point2D<int>(0, -1)},
    };
  }
}

class MirrorDownUp : Tile // /
{
  public List<Beam> AdvanceBeam(Beam beam)
  {
    var newBeam = beam;
    // Beam is going up
    if (beam.Direction.y == -1)
    {
      newBeam = beam with { Direction = new Point2D<int>(1, 0) };
    }
    // Beam is going down
    if (beam.Direction.y == 1)
    {
      newBeam = beam with { Direction = new Point2D<int>(-1, 0) };
    }
    // Beam is going left
    if (beam.Direction.x == -1)
    {
      newBeam = beam with { Direction = new Point2D<int>(0, 1) };
    }
    // Beam is going right
    if (beam.Direction.x == 1)
    {
      newBeam = beam with { Direction = new Point2D<int>(0, -1) };
    }
    // Advance beam
    return new EmptySpace().AdvanceBeam(newBeam);
  }
}

class MirrorUpDown : Tile // /
{
  public List<Beam> AdvanceBeam(Beam beam)
  {
    var newBeam = beam;
    // Beam is going up
    if (beam.Direction.y == -1)
    {
      newBeam = beam with { Direction = new Point2D<int>(-1, 0) };
    }
    // Beam is going down
    if (beam.Direction.y == 1)
    {
      newBeam = beam with { Direction = new Point2D<int>(1, 0) };
    }
    // Beam is going left
    if (beam.Direction.x == -1)
    {
      newBeam = beam with { Direction = new Point2D<int>(0, -1) };
    }
    // Beam is going right
    if (beam.Direction.x == 1)
    {
      newBeam = beam with { Direction = new Point2D<int>(0, 1) };
    }
    // Advance beam
    return new EmptySpace().AdvanceBeam(newBeam);
  }
}

class TileFactory
{
  public static Tile Make(char type)
  {
    return type switch
    {
      '-' => new HorizontalSplitter(),
      '|' => new VerticalSplitter(),
      '.' => new EmptySpace(),
      '\\' => new MirrorUpDown(),
      '/' => new MirrorDownUp(),
      _ => new EmptySpace(),
    };
  }
}

class Day16 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(16).Read().ToList();
    var map = lines.SelectMany((line, lineIndex) => line.Select((column, columnIndex) => (new Point2D<int>(columnIndex, lineIndex), TileFactory.Make(column))))
                                                        .ToDictionary(kv => kv.Item1, kv => kv.Item2);

    HashSet<Point2D<int>> totalVisitedPart01 = ProcessBeam(map, new(new(0, 0), new(1, 0)));

    var starterBeams = new List<Beam>();
    for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
    {
      starterBeams.Add(new (new(0, lineIndex), new (1 , 0)));
      starterBeams.Add(new (new(lines[lineIndex].Length - 1, lineIndex), new (-1 , 0)));
    }

    for (int columnIndex = 0; columnIndex < lines[0].Length; columnIndex++)
    {
      starterBeams.Add(new (new(columnIndex, 0), new (0 , 1)));
      starterBeams.Add(new (new(columnIndex, lines.Count - 1), new (0 , -1)));
    }

    var bestConfig = starterBeams.Select(beam => ProcessBeam(map, beam).Count).Max();

    return $"""
      The number of Energized tiles is {totalVisitedPart01.Count}
      The best config energizes {bestConfig} tiles
      """;
  }

  private static HashSet<Point2D<int>> ProcessBeam(Dictionary<Point2D<int>, Tile> map, Beam initialBeam)
  {
    var visited = new HashSet<Beam>();
    var totalVisited = new HashSet<Point2D<int>>();
    // The thing is: if a beam enters any tile in a given direction, we can stop processing it as it will not change the outcome

    Queue<Beam> beamsToProcess = new();
    beamsToProcess.Enqueue(initialBeam);

    while (beamsToProcess.Count > 0)
    {
      var nextBeam = beamsToProcess.Dequeue();
      if (visited.Contains(nextBeam))
      {
        continue;
      }

      // Out of bounds
      if (!map.ContainsKey(nextBeam.Position))
      {
        continue;
      }

      totalVisited.Add(nextBeam.Position);
      visited.Add(nextBeam);

      var nextTile = map[nextBeam.Position];
      var toProcess = nextTile.AdvanceBeam(nextBeam);
      toProcess.ForEach(beamsToProcess.Enqueue);
    }

    return totalVisited;
  }
}