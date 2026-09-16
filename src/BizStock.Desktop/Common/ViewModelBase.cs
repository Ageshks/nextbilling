using BizStock.Application.Common.Exceptions;
using BizStock.Domain.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace BizStock.Desktop.Common;

/// <summary>
/// Base class for every screen and dialog view model. Centralises busy state, the
/// user-facing error banner, and the single place where application exceptions are
/// translated into messages a shopkeeper can act on.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    private bool _isBusy;
    private string? _errorMessage;
    private string? _statusMessage;

    /// <summary>Logger for the derived view model.</summary>
    protected ILogger Logger { get; }

    /// <summary>Creates the view model.</summary>
    protected ViewModelBase(ILogger logger) => Logger = logger;

    /// <summary>Screen title shown in the shell header.</summary>
    public virtual string Title => string.Empty;

    /// <summary>True while a long-running operation is in flight; disables input.</summary>
    public bool IsBusy
    {
        get => _isBusy;
        protected set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
    }

    /// <summary>Convenience inverse of <see cref="IsBusy"/> for binding to IsEnabled.</summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>Error banner text; null when there is nothing to report.</summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        protected set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>Whether an error banner should be visible.</summary>
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    /// <summary>Transient success/information text.</summary>
    public string? StatusMessage
    {
        get => _statusMessage;
        protected set
        {
            if (SetProperty(ref _statusMessage, value))
            {
                OnPropertyChanged(nameof(HasStatus));
            }
        }
    }

    /// <summary>Whether the status line should be visible.</summary>
    public bool HasStatus => !string.IsNullOrWhiteSpace(StatusMessage);

    /// <summary>
    /// Runs an operation with busy/error handling applied. Business failures surface as a
    /// readable message; unexpected failures are logged in full and reported generically.
    /// </summary>
    protected async Task RunAsync(Func<Task> operation, string? successMessage = null, CancellationToken ct = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            await operation();
            if (successMessage is not null)
            {
                StatusMessage = successMessage;
            }
        }
        catch (OperationCanceledException)
        {
            // User-initiated cancellation is not an error.
        }
        catch (InsufficientStockException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogWarning(ex, "Stock rule rejected an operation.");
        }
        catch (AppValidationException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogWarning(ex, "Validation rejected an operation.");
        }
        catch (AuthorizationException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogWarning(ex, "Authorization rejected an operation: {Permission}", ex.PermissionKey);
        }
        catch (NotFoundException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogWarning(ex, "Record not found: {Entity}", ex.EntityName);
        }
        catch (DomainException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogWarning(ex, "Domain rule rejected an operation.");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Something went wrong. The operation was not completed. Details have been written to the application log.";
            Logger.LogError(ex, "Unhandled failure in {ViewModel}.", GetType().Name);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Clears the error and status banners.</summary>
    protected void ClearMessages()
    {
        ErrorMessage = null;
        StatusMessage = null;
    }

    /// <summary>Called each time the shell navigates to this screen.</summary>
    public virtual Task ActivateAsync() => Task.CompletedTask;
}