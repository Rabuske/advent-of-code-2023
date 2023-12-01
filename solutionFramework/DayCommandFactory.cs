class DayCommandFactory {

    public IDayCommand GetCommand(int day) {
        Type? classType = Type.GetType($"Day{day.ToString("00")}");
        if(classType == null) {
            return new NullDay();
        }

        IDayCommand? dayCommand =  (IDayCommand?) Activator.CreateInstance(classType);
        if(dayCommand == null) {
            return new NullDay();
        }
        return dayCommand;        
    }

}