class Day01 : IDayCommand
{
  public string Execute()
  {
    var numberMappingPart01 = new Dictionary<string, int> {
      {"1", 1},
      {"2", 2},
      {"3", 3},
      {"4", 4},
      {"5", 5},
      {"6", 6},
      {"7", 7},
      {"8", 8},
      {"9", 9},
    };

    var numberMappingPart02 = numberMappingPart01.Concat(new Dictionary<string, int>{
      {"one", 1},
      {"two", 2},
      {"three", 3},
      {"four", 4},
      {"five", 5},
      {"six", 6},
      {"seven", 7},
      {"eight", 8},
      {"nine", 9},
    }).ToDictionary(pair => pair.Key, pair => pair.Value);


    var lines = new FileReader(01).Read();
    var calibrationValuesPart01 = lines.Select(line => ExtractCalibrationValue(line, numberMappingPart01));
    var calibrationValuesPart02 = lines.Select(line => ExtractCalibrationValue(line, numberMappingPart02));
    return $"""
               Sum of calibration values Par01: {calibrationValuesPart01.Sum()} 
               Sum of calibration values Par02: {calibrationValuesPart02.Sum()}
            """;
  }

  private int ExtractCalibrationValue(string input, Dictionary<string, int> mapping)
  {
    int calibrationValue = 0;

    for(int i = 0; i < input.Length; i ++)
    {    
      string substring = input[i..];
      foreach (var number in mapping)
      {
        if(substring.StartsWith(number.Key))
        {
          calibrationValue += 10 * number.Value;
          i = input.Length;
          break;
        }
      }  
    }

    for(int i = input.Length - 1; i >= 0; i--)
    {
      string substring = input[i..];
      foreach (var number in mapping)
      {
        if(substring.StartsWith(number.Key))
        {
          calibrationValue += number.Value;
          i = -1;
          break;
        }
      }  
    }

    return calibrationValue;

  }

}