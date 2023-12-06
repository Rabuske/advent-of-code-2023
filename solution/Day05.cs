using RangeMap = System.Collections.Generic.List<(long sourceStart, long destinationOffset, long sourceEnd)>;

class Day05 : IDayCommand
{
  public string Execute()
  {
    // Parsing shenanigans
    var lines = new FileReader(05).Read().ToList();
    var SeedToSoil = new RangeMap();
    var SoilToFertilizer = new RangeMap();
    var FertilizerToWater = new RangeMap();
    var WaterToLight = new RangeMap();
    var LightToTemperature = new RangeMap();
    var TemperatureToHumidity = new RangeMap();
    var HumidityToLocation = new RangeMap();
    var Seeds = new List<long>();

    var currentMap = SeedToSoil;

    foreach(var line in lines)
    {
      if(line.StartsWith("seeds")) 
      {
        Seeds = line.Split(":")[1].Trim().Split(" ").Select(long.Parse).ToList();
        continue;
      }

      if(string.IsNullOrEmpty(line))
      {
        continue;
      }

      if(!char.IsDigit(line[0]))
      {
        currentMap = line.Split(" ")[0] switch
        {
          "seed-to-soil" => SeedToSoil,
          "soil-to-fertilizer" => SoilToFertilizer,
          "fertilizer-to-water" => FertilizerToWater,
          "water-to-light" => WaterToLight,
          "light-to-temperature" => LightToTemperature,
          "temperature-to-humidity" => TemperatureToHumidity,
          "humidity-to-location" => HumidityToLocation,
          _ => currentMap
        };
        continue;
      }

      var mapping = line.Split(" ").Select(long.Parse).ToList();
      currentMap.Add((mapping[1], mapping[0] - mapping[1], mapping[1] + mapping[2] - 1));
    }

    var locations = Seeds
      .Select(s => ApplyMap(s, SeedToSoil))
      .Select(s => ApplyMap(s, SoilToFertilizer))
      .Select(s => ApplyMap(s, FertilizerToWater))
      .Select(s => ApplyMap(s, WaterToLight))
      .Select(s => ApplyMap(s, LightToTemperature))
      .Select(s => ApplyMap(s, TemperatureToHumidity))
      .Select(s => ApplyMap(s, HumidityToLocation));


    List<(long start, long end)> SeedRanges = Enumerable.Range(0, Seeds.Count / 2)
      .Select(i => (Seeds[i * 2], Seeds[i * 2] + Seeds[i * 2 + 1] - 1))
      .ToList();

    var locationsRanges = SeedRanges;
    locationsRanges = ApplyMapToRange(locationsRanges, SeedToSoil);
    locationsRanges = ApplyMapToRange(locationsRanges, SoilToFertilizer);
    locationsRanges = ApplyMapToRange(locationsRanges, FertilizerToWater);
    locationsRanges = ApplyMapToRange(locationsRanges, WaterToLight);
    locationsRanges = ApplyMapToRange(locationsRanges, LightToTemperature);
    locationsRanges = ApplyMapToRange(locationsRanges, TemperatureToHumidity);
    locationsRanges = ApplyMapToRange(locationsRanges, HumidityToLocation);

    return $"""
      The lowest location is {locations.Min()}
      The lowest location considering ranges is {locationsRanges.Min().start}
      """;
  }

  public long ApplyMap(long input, RangeMap map)
  {
    foreach(var entry in map)
    {
      if(input >= entry.sourceStart && input <= entry.sourceEnd)
      {
        return input + entry.destinationOffset;
      }
    }
    return input;
  }

  public List<(long start, long end)> ApplyMapToRange(List<(long start, long end)> sources, RangeMap conversionMap)
  {
    return sources.SelectMany(source => {
      var toBeEvaluated = new List<(long start, long end)>();
      toBeEvaluated.AddRange(conversionMap
        .Where(cm => cm.sourceStart <= source.end && cm.sourceEnd >= source.start)
        .Select(cm => (cm.sourceStart < source.start? source.start : cm.sourceStart, cm.sourceEnd > source.end? source.end : cm.sourceEnd))
      );
      
      if(toBeEvaluated.Count == 0)
      {
        // If there is no conversion map that affects this range the return is itself
        return new List<(long start, long end)>(){};
      }
      toBeEvaluated.Sort();
      
      //Adjust first and last entries (gap adjustment is not necessary)
      var firstEntry = toBeEvaluated[0];
      var lastEntry = toBeEvaluated.Last();
      if(firstEntry.start > source.start)
      {
        toBeEvaluated.Add((source.start, firstEntry.start -1));
      }

      if(lastEntry.end < source.end)
      {
        toBeEvaluated.Add((lastEntry.end + 1, source.end));
      }      

      return toBeEvaluated.Select(tb => (ApplyMap(tb.start, conversionMap), ApplyMap(tb.end, conversionMap))).ToList();
    }).OrderBy(r => r.Item1).ToList(); 
  }

}

