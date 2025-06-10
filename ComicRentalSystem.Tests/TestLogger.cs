using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic;

namespace ComicRentalSystem.Tests
{
    public class TestLogger : ILogger
    {
        public List<string> Messages { get; } = new();

        public void Log(string message) => Messages.Add(message);

        public void Log(string message, Exception ex) => Messages.Add($"{message} EX:{ex.Message}");

        public void LogError(string message, Exception? ex = null) => Messages.Add(ex == null ? message : $"{message} EX:{ex.Message}");

        public void LogWarning(string message) => Messages.Add(message);

        public void LogInformation(string message) => Messages.Add(message);

        public void LogDebug(string message) => Messages.Add(message);
    }
}
