using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;

namespace ComicRentalSystem_14Days.Tests.Mocks
{
    public class MockLogger : ILogger
    {
        public List<string> LoggedMessages { get; } = new List<string>();
        public List<(string message, Exception? ex)> LoggedErrors { get; } = new List<(string, Exception?)>();
        public List<string> LoggedWarnings { get; } = new List<string>();
        public List<string> LoggedInformations { get; } = new List<string>();
        public List<string> LoggedDebugs { get; } = new List<string>();

        public void Log(string message)
        {
            LoggedMessages.Add(message);
        }

        public void Log(string message, Exception ex)
        {
            LoggedMessages.Add($"{message} Exception: {ex?.Message}");
            // Optionally, store the exception too if needed for assertions
        }

        public void LogError(string message, Exception? ex = null)
        {
            LoggedErrors.Add((message, ex));
        }

        public void LogWarning(string message)
        {
            LoggedWarnings.Add(message);
        }

        public void LogInformation(string message)
        {
            LoggedInformations.Add(message);
        }

        public void LogDebug(string message)
        {
            LoggedDebugs.Add(message);
        }

        // Helper method to clear logs between tests
        public void ClearAllLogs()
        {
            LoggedMessages.Clear();
            LoggedErrors.Clear();
            LoggedWarnings.Clear();
            LoggedInformations.Clear();
            LoggedDebugs.Clear();
        }
    }
}
