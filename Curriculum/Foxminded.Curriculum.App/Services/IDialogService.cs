using System.Windows;

namespace Foxminded.Curriculum.App.Services;

public interface IDialogService
{
    MessageBoxResult Show(string messageText);
    MessageBoxResult Show(string messageText, string caption);
    MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton);

    MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton, MessageBoxImage icon);
    MessageBoxResult ShowCustomError(string messageText);
}
