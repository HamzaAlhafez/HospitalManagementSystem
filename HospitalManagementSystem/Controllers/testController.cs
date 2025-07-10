using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        [HttpGet("test-serilog")]
        public IActionResult TestSerilog()
        {
            // Information level logging for normal operations
            Log.Information("Testing Serilog information level logging");

            // Warning level for unusual but expected events
            Log.Warning("This is a test warning message");

            // Structured logging with parameters
            Log.Information("Testing structured logging with parameters. User: {UserId}, Action: {ActionType}",
                123, "SerilogTest");

            try
            {
                // Simulate an error condition
                throw new InvalidOperationException("Simulated exception for testing Serilog error handling");
            }
            catch (Exception ex)
            {
                // Error level logging with exception details
                Log.Error(ex, "Error occurred during Serilog testing. Additional context: {TestContext}",
                    "Integration Test");

                // Warning level for handled exceptions
                Log.Warning("Handled exception of type {ExceptionType} during test",
                    ex.GetType().Name);
            }

            // Debug level for detailed troubleshooting (won't appear in production by default)
            Log.Debug("Debug-level details: {DebugInfo}", new
            {
                Timestamp = DateTime.UtcNow,
                Service = "SerilogTestService"
            });

            return Ok(new
            {
                Status = "Success",
                Message = "Serilog test completed successfully",
                TestedFeatures = new[] {
            "Information logging",
            "Warning logging",
            "Error logging",
            "Structured logging",
            "Exception handling"
        }
            });
        }

    }
}
