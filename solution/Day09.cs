
class Day09 : IDayCommand
{
  public string Execute()
  {

    // Parsing shenanigans
    var lines = new FileReader(09).Read().ToList();

    var sequences = lines.Select(line => line.Split(" ").Select(long.Parse).ToList()).ToList();
    var extrapolatedValuesRight = sequences.Select(GenerateExtrapolatedValue);
    var sumOfExtrapolatedValuesRight = extrapolatedValuesRight.Sum();
    var extrapolatedValuesLeft = sequences.Select(seq => Enumerable.Reverse(seq).ToList()).Select(GenerateExtrapolatedValue);
    var sumOfExtrapolatedValuesLeft = extrapolatedValuesLeft.Sum();

    return $"""
    The sum of the right extrapolated histories for all sequences is {sumOfExtrapolatedValuesRight}
    The sum of the left extrapolated histories for all sequences is {sumOfExtrapolatedValuesLeft}
    """;
  }

  public long GenerateExtrapolatedValue(List<long> sequence)
  {
    var sequenceHistory = new List<List<long>>{sequence.Select(n => n).ToList()};
    var currentSequence = sequence;
    while(!currentSequence.All(n => n == 0))
    {
      var newSequence = currentSequence.Take(currentSequence.Count - 1).Select((item, index) => currentSequence[index + 1] - item).ToList();
      sequenceHistory.Add(newSequence);
      currentSequence = newSequence;
    }

    // calculate extrapolated values (except for the last one, which will be zero anyways)
    sequenceHistory.Reverse();
    return sequenceHistory.Select(l => l.Last()).Sum();
  } 

}
