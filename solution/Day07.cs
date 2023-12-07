

using System.Reflection.Metadata;

class Day07 : IDayCommand
{
  public string Execute()
  {
    
    // Parsing shenanigans
    var lines = new FileReader(07).Read().ToList();

    var handsAndBets = lines.Select(line => {
      var parts = line.Split(" ");
      var hand = parts[0].ToCharArray();
      var bet = int.Parse(parts[1]);
      return (hand, bet);
    }).ToList();
    
    // Values for part01
    var cardValues = new Dictionary<char, int>(){
      {'A', 14},
      {'K', 13},
      {'Q', 12},
      {'J', 11},
      {'T', 10},
      {'9', 9},
      {'8', 8},
      {'7', 7},
      {'6', 6},
      {'5', 5},
      {'4', 4},
      {'3', 3},
      {'2', 2},
    };

    var handComparerPart01 = new HandComparerPart01();
    var cardComparerPart01 = new CardComparer() { Values = cardValues};
    var rankedHandsPart01 = handsAndBets.OrderBy(e => e.hand, handComparerPart01).ThenBy(e => e.hand, cardComparerPart01).ToList();
    var totalWinningsPart01 = rankedHandsPart01.Select((card, index) => card.bet * (index + 1)).Sum();

    // Part 02
    cardValues['J'] = 1;
    var handComparerPart02 = new HandComparerPart02();
    var cardComparerPart02 = new CardComparer() { Values = cardValues};
    var rankedHandsPart02 = handsAndBets.OrderBy(e => e.hand, handComparerPart02).ThenBy(e => e.hand, cardComparerPart02).ToList();
    var totalWinningsPart02 = rankedHandsPart02.Select((card, index) => card.bet * (index + 1)).Sum();

    return $"""
    The total Winnings is {totalWinningsPart01}
    The total Winnings with Joker is {totalWinningsPart02}
    """;
  }
  
}


public class HandComparerPart01 : IComparer<char[]>
{    public int Compare(char[]? x, char[]? y)
    {
      if(x is null || y is null) return 0;

      var groupedX = x.GroupBy(c => c).ToDictionary(k => k.Key, v => v.Count());
      var groupedY = y.GroupBy(c => c).ToDictionary(k => k.Key, v => v.Count());

      var valueOfX = CalculateValue(groupedX);
      var valueOfY = CalculateValue(groupedY);
      var result = valueOfX - valueOfY;
      return result;
    }

    private int CalculateValue(Dictionary<char, int> group)
    {
      if(group.Count == 1) return 8; // Five of a kind
      if(group.Count == 2) {
        if(group.First().Value == 4 || group.First().Value == 1) return 7; // Four of a kind
        return 6; // Full house
      }
      if(group.Count == 3)
      {
        if(group.Values.Any(v => v == 3)) return 5; //Three of a kind
        return 4; // Two pair
      }
      if(group.Count == 4) return 3; // One pair
      return 2; // High card
    }
}


public class HandComparerPart02 : IComparer<char[]>
{
    public IComparer<char[]> HandComparer = new HandComparerPart01();

    public static Dictionary<string, string> Memory = new();

    private char[] cards = new []{'A', 'K', 'Q', 'J', 'T','9','8','7','6','5','4','3','2'};

    public int Compare(char[]? x, char[]? y)
    {
      var highestX = CalculateHighestValue(x);
      var highestY = CalculateHighestValue(y);      

      return HandComparer.Compare(highestX, highestY);
    }

    private char[] CalculateHighestValue(char[]? x)
    {
      if(x is null) return Array.Empty<char>();
      var stringRepresentation = new string(x);
      if(Memory.ContainsKey(stringRepresentation)) return Memory[stringRepresentation].ToCharArray();

      var numberOfJokers = x.Where(c => c == 'J').Count();
      var currentList = new List<List<char>>
      {
          x.Where(c => c != 'J').ToList()
      };

      for (int i = 0; i < numberOfJokers; i++)
      {
        currentList = cards.SelectMany(card => currentList.Select(list => list.Concat(new List<char>{card}).ToList())).ToList();
      }

      var highest = currentList.Select(e => e.ToArray()).OrderBy(c => c, HandComparer).Last();
      Memory.Add(stringRepresentation, new string(highest));
      return highest;
    }
}

public class CardComparer : IComparer<char[]>
{
    public required Dictionary<char, int> Values; 
    public int Compare(char[]? x, char[]? y)
    {
       if(x is null || y is null) return 0;
        for (int i = 0; i < x.Length; i++)
        {
          var valueX = Values[x[i]];
          var valueY = Values[y[i]];
          var result = valueX - valueY;  
          if(result != 0) return result;
        }       
        return 0;
    }
}
