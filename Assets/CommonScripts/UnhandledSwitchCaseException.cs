public class UnhandledSwitchCaseException : System.Exception
{
    private string additionalInformation;

    public UnhandledSwitchCaseException(string message) : base(message)
    {
        additionalInformation = message;
    }

    public UnhandledSwitchCaseException() : base()
    {

    }

    public override string ToString()
    {
        return base.ToString() + "\nAdditional Information: " + additionalInformation;
    }
}
