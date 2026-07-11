namespace Content.Server._ECHO.Computer;

public struct ComputerUserData
{
    [ViewVariables]
    public string Username = "";

    [ViewVariables]
    public string Password = "";

    public ComputerUserData(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
