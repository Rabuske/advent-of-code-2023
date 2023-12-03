record Game(int Id, List<Dictionary<string, int>> Sets);
class Day02 : IDayCommand
{
  public string Execute()
  {
    var bag = new Dictionary<string, int> {
      {"red", 12},
      {"green", 13},
      {"blue", 14},
    };

    // Parsing shenanigans
    var lines = new FileReader(02).Read();
    var games = lines.Select(line => {
      var gameAndGameInfo =  line.Split(":"); 
      var gameId = int.Parse(gameAndGameInfo[0].Split(" ")[1]);
      var gameSets = gameAndGameInfo[1].Split(";");
      var sets = gameSets.Select(line => line.Trim()).Select(set => set.Split(",").Select(pick => { 
        var numberAndColor = pick.Trim().Split(" ");
        return new KeyValuePair<string, int>(numberAndColor[1], int.Parse(numberAndColor[0]));
      }).ToDictionary(group => group.Key, g => g.Value));

      return new Game(gameId, sets.ToList());      
    });

    // Determine possible games
    var possibleGames = games.Where(game => game.Sets.All(set => set.All(pick => pick.Value <= bag[pick.Key]))).Select(game => game.Id);
    var sumOfPossibleGames = possibleGames.Sum();

    // Determine the power the sets of games
    var powerOfGameSets = games.Select(game => 
    {
      var maxBlue = game.Sets.Max(set => set.GetValueOrDefault("blue", 0));
      var maxRed = game.Sets.Max(set => set.GetValueOrDefault("red", 0));
      var maxGreen = game.Sets.Max(set => set.GetValueOrDefault("green", 0));
      return maxBlue * maxGreen * maxRed;
    });

    return $"""
    The sum of the ids of possible games is {sumOfPossibleGames}
    The sum of the power sets is {powerOfGameSets.Sum()}
    """;

  }

}