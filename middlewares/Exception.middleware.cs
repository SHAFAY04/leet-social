

public class ExceptionMiddleware
{

    RequestDelegate _next;
    ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next=next;        
        _logger=logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            
            context.Response.StatusCode=ex._statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                message=ex.Message
            });
        }
        catch(Exception e)
        {

            _logger.LogError(e,"Unhandled Exception");
            context.Response.StatusCode=StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                message="Something went wrong!"
            });
        }
    }
}