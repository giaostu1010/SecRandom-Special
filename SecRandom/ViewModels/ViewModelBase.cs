using CommunityToolkit.Mvvm.ComponentModel;
using SecRandom.Models;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels;

public class ViewModelBase(MainConfigHandler configHandler) : ObservableRecipient
{
    public MainConfigModel Config { get; } = configHandler.Data;
}