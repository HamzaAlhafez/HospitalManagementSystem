namespace HospitalManagementSystem.Middlewares
{
    public class RateLimtRequest
    {
        private readonly RequestDelegate _next;
        private static int _counter;
        private static DateTime _lastdatetime = DateTime.Now;

        public RateLimtRequest(RequestDelegate next)
        {
            _next = next;



        }
        public async Task InvokeAsync(HttpContext context)
        {
            _counter++;
            if (DateTime.Now.Subtract(_lastdatetime).Seconds > 10)
            {
                _counter = 1;
                _lastdatetime = DateTime.Now;
                await _next(context);


            }
            else
            {
                if (_counter > 5)
                {
                    _lastdatetime = DateTime.Now;
                    await context.Response.WriteAsync("Rate limt Exceedded");
                }
                else
                {
                    _lastdatetime = DateTime.Now;
                    await _next(context);
                }
            }




        }

    }
}
