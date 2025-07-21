using ChessAnywhere.Models;
using CommunityToolkit.Mvvm.Input;

namespace ChessAnywhere.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}