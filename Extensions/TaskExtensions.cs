namespace MauiReactorPeopleInSpace.Extensions;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler? handler = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            handler?.HandleError(ex);
        }
    }
}

public interface IErrorHandler
{
    void HandleError(Exception ex);
}

public class ErrorHandler : IErrorHandler
{
    public void HandleError(Exception ex)
    {
        // Log or handle the exception as needed
        Debug.WriteLine($"Unhandled exception: {ex}");
    }
}