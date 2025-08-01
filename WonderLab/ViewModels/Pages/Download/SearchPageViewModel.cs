﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Services.Download;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class SearchPageViewModel : ObservableObject {
    private readonly SearchService _searchService;

    [ObservableProperty] private ReadOnlyObservableCollection<object> _resources;
    [ObservableProperty] private MinecraftVersionType _minecraftVersionType = MinecraftVersionType.Release;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCommunityResourcesFilterVisible))]
    private SearchType _searchType = SearchType.Minecraft;

    public bool IsCommunityResourcesFilterVisible => SearchType is not SearchType.Minecraft;

    public SearchPageViewModel(SearchService searchService) {
        _searchService = searchService;

        SearchType = _searchService.SearchType;
        MinecraftVersionType = _searchService.MinecraftVersionType;
    }

    [RelayCommand]
    private Task OnLoaded() => Task.Run(async () => {
        await _searchService.InitMinecraftsAsync(default);
        Resources = new(_searchService.Resources);

        PropertyChanged += OnPropertyChanged;
    });

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(MinecraftVersionType):
                _searchService.MinecraftVersionType = MinecraftVersionType;
                _searchService.FilterMinecrafts();
                break;
            case nameof(SearchType):
                _searchService.SearchType = SearchType;
                break;
        }
    }
}