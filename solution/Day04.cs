
using System.Text.RegularExpressions;

class Card 
{
  public int Id {get; set;}
  public required HashSet<int> WinningNumbers {get; set;}
  public required HashSet<int> OwnNumbers {get; set;}

  private long worthiness = -1;

  public int CalculateTotalMatchingNumbers()
  {
    return OwnNumbers.Intersect(WinningNumbers).Count();
  }

  public long CalculateWorthiness()
  {
    if(worthiness != -1) return worthiness;
    var matchingNumbers = CalculateTotalMatchingNumbers();
    worthiness = (long) Math.Pow(2, matchingNumbers - 1);
    return worthiness;
  }

  public static int CalculateCardDistribution(List<Card> cards)
  {
    // We start with a single copy of each card
    var totalCards = cards.ToDictionary(c => c, c => 1);
    foreach (var card in cards)
    {
      var numberOfCurrentCard = totalCards[card];
      var worthiness = card.CalculateTotalMatchingNumbers();
      for(int i = 0; i < worthiness; i++)
      {
        var nextCard = card.Id + i;
        if(nextCard < cards.Count)
        {
          totalCards[cards[nextCard]] += numberOfCurrentCard;
        }
      }
    }

    return totalCards.Select(tc => tc.Value).Sum();
  }
}

class Day04 : IDayCommand
{
  public string Execute()
  {
   
    // Parsing shenanigans
    var lines = new FileReader(04).Read().ToList();
    var cards = lines.Select(line => Regex.Replace(line, @"\s+", " ")).Select(line => {
      var cardIdAndRest = line.Split(":");
      var cardId = int.Parse(cardIdAndRest[0].Split(" ")[1]);
      var winningNumbersAndMyNumbers = cardIdAndRest[1].Split("|");
      var winningNumbers = winningNumbersAndMyNumbers[0].Trim().Split(" ").Select(int.Parse).ToHashSet();
      var myNumbers = winningNumbersAndMyNumbers[1].Trim().Split(" ").Select(int.Parse).ToHashSet();
      return new Card{Id = cardId, OwnNumbers = myNumbers, WinningNumbers = winningNumbers};
    }).ToList();    

    var totalPoints = cards.Select(c => c.CalculateWorthiness()).Sum();
    var totalInstances = Card.CalculateCardDistribution(cards);

   return $"""
      The sum of the points is {totalPoints}
      The total number of cards is {totalInstances}
      """;
  }

}