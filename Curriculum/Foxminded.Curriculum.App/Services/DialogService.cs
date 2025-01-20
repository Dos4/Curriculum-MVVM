using System.Windows;
using Foxminded.Curriculum.App.Resources;

namespace Foxminded.Curriculum.App.Services;

public class DialogService : IDialogService
{
    public MessageBoxResult Show(string messageText)
    {
        return MessageBox.Show(messageText);
    }

    public MessageBoxResult Show(string messageText, string caption)
    {
        return MessageBox.Show(messageText, caption);
    }

    public MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton)
    {
        return MessageBox.Show(msg, caption, btnsMessageBoxButton);
    }

    public MessageBoxResult Show(string msg, string caption, MessageBoxButton btnsMessageBoxButton, MessageBoxImage icon)
    {
        return MessageBox.Show(msg, caption, btnsMessageBoxButton, icon);
    }

    public MessageBoxResult ShowCustomError(string messageText)
    {
        return MessageBox.Show(messageText, caption: ProgramInterface.ErrorText, button: MessageBoxButton.OK, 
            icon: MessageBoxImage.Error);
    }
}
