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
    locationsRanges = ApplyMapToRanges(locationsRanges, SeedToSoil);
    locationsRanges = ApplyMapToRanges(locationsRanges, SoilToFertilizer);
    locationsRanges = ApplyMapToRanges(locationsRanges, FertilizerToWater);
    locationsRanges = ApplyMapToRanges(locationsRanges, WaterToLight);
    locationsRanges = ApplyMapToRanges(locationsRanges, LightToTemperature);
    locationsRanges = ApplyMapToRanges(locationsRanges, TemperatureToHumidity);
    locationsRanges = ApplyMapToRanges(locationsRanges, HumidityToLocation);

    return $"""
      The lowest location is {locations.Min()}
      The lowest location considering ranges is {locationsRanges[0].start}
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

  public List<(long start, long end)> ApplyMapToRanges(List<(long start, long end)> sourceRanges, RangeMap conversionMap)
  {
    return sourceRanges.SelectMany(sourceRange => {      
      // Find all the intersections
      var intersections = conversionMap.Where(
        conversionEntry => !(conversionEntry.sourceStart > sourceRange.end || conversionEntry.sourceEnd < sourceRange.end)
      ).OrderBy(conversionEntry => conversionEntry.sourceStart)
       .ToList();

      // In case there are no intersections, just return the range itself
      if(intersections.Count() == 0)
      {
        return new List<(long start, long end)>{sourceRange};
      }

      // Cap the start and end in case they "overflow"
      intersections = intersections.Select(intersection => {
        var startSource = intersection.sourceStart < sourceRange.start? sourceRange.start : intersection.sourceStart;
        var endSource = intersection.sourceEnd > sourceRange.end ? sourceRange.end : intersection.sourceEnd;
        return (startSource, intersection.destinationOffset, endSource);
      }).OrderBy(intersection => intersection.startSource)
        .ToList();

      // Add all gaps as conversion sources that map 1:1
      var intersectionsWithNoGap = new RangeMap();
      var currentStart = sourceRange.start;
      foreach (var intersection in intersections)
      {
        if(currentStart < intersection.sourceStart)
        {
          intersectionsWithNoGap.Add((currentStart, 0, intersection.sourceStart - 1));
        }
        intersectionsWithNoGap.Add(intersection);
        currentStart = intersection.sourceEnd + 1;
      }
      var lastIntersection = intersectionsWithNoGap.Last();
      var lastPosition = lastIntersection.sourceEnd + 1;
      if(lastPosition < sourceRange.end)
      {
        intersectionsWithNoGap.Add((lastPosition, 0, sourceRange.end));       
      }

      // Now, convert all the intersections to the destinations based on the range            
      return intersectionsWithNoGap.Select(intersection => (ApplyMap(intersection.sourceStart, intersectionsWithNoGap), ApplyMap(intersection.sourceEnd, intersectionsWithNoGap)));
    }).OrderBy(item => item.Item1).ToList();
  }

}

