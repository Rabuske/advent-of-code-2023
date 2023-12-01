class NullDay: IDayCommand {

    public string Execute() {
        return "No valid day found - or it is not implemented yet";
    }
}