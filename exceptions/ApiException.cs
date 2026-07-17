


//the reason why we made it abstract is that we only want it for polymorphism.
//and we dont want anyone to be doing  "throw new ApiException("something went wrong",500);"
//thats why we marked it abstract so that only child can be thrown
public abstract class ApiException:Exception
{

    public int _statusCode {get;}
    
    protected ApiException(string message, int statuscode) : base(message)
    {
        _statusCode=statuscode;
    }
}