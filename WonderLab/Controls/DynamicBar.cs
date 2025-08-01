﻿using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions;
using WonderLab.Media.Transitions;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[PseudoClasses(":press")]
[StyledProperty(typeof(BarState), "BarState", BarState.Collapsed)]
public partial class DynamicBar : ContentControl {
    private double _startX;
    private double _offsetX;
    private bool _canOpenPanel;
    private Border _PART_LayoutBorder;
    private Border _PART_ContentLayoutBorder;
    private CancellationTokenSource _cancellationTokenSource = new();

    private readonly DynamicBarTransition _barTransition = new();

    private void SetPseudoclasses(bool isPress) {
        PseudoClasses.Set(":press", isPress);
    }

    private void OnLayoutPointerMoved(object sender, PointerEventArgs e) {
        if (BarState is not BarState.Collapsed) {
            return;
        }

        if (e.GetCurrentPoint(_PART_LayoutBorder).Properties.IsLeftButtonPressed) {
            var position = e.GetPosition(this);
            _offsetX = position.X - _startX;
            if (_offsetX > 0 || _offsetX < -15) {
                return;
            }

            _canOpenPanel = _offsetX < -5;
            _PART_LayoutBorder.Margin = new(0, 0, -_offsetX, 0);
        }
    }

    private void OnLayoutPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (BarState is not BarState.Collapsed) {
            return;
        }

        SetPseudoclasses(false);
        if (e.InitialPressMouseButton is MouseButton.Left) {
            _PART_LayoutBorder.Margin = new Thickness(0, 0, 0, 0);

            if (_offsetX is 0) {
                BarState = BarState.Expanded;
            }

            if (_canOpenPanel) {
                BarState = _canOpenPanel ? BarState.Expanded : BarState.Collapsed;
                _canOpenPanel = false;
            }

            _offsetX = 0;
        }
    }

    private void OnLayoutPointerPressed(object sender, PointerPressedEventArgs e) {
        if (BarState is not BarState.Collapsed)
            return;

        SetPseudoclasses(true);
        if (e.GetCurrentPoint(_PART_LayoutBorder).Properties.IsLeftButtonPressed) {
            _startX = e.GetPosition(this).X;
        }
    }

    private void OnLayoutPointerCaptureLost(object sender, PointerCaptureLostEventArgs e) {
        _PART_LayoutBorder.Margin = new Thickness(0, 0, 0, 0);
    }

    protected override async void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e) {
        base.OnLoaded(e);

        await Task.Delay(250);

        var task = _PART_LayoutBorder.Animate(OpacityProperty)
            .WithDuration(TimeSpan.FromSeconds(0.8))
            .From(0)
            .To(1)
            .RunAsync(_cancellationTokenSource.Token);

        var task1 = _PART_LayoutBorder.Animate(TranslateTransform.XProperty)
            .WithDuration(TimeSpan.FromSeconds(0.5))
            .From(25d)
            .To(-16d)
            .RunAsync(_cancellationTokenSource.Token);

        var taks2 = _PART_ContentLayoutBorder.Animate(TranslateTransform.XProperty)
            .WithEasing(new ExponentialEaseIn())
            .WithDuration(TimeSpan.FromSeconds(0.1))
            .From(-16d)
            .To(460d)
            .RunAsync(_cancellationTokenSource.Token);

        await Task.WhenAll(task, task1, taks2);
        _PART_ContentLayoutBorder.Opacity = 1;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        _PART_LayoutBorder = e.NameScope.Find<Border>("PART_LayoutBorder");
        _PART_ContentLayoutBorder = e.NameScope.Find<Border>("PART_ContentLayoutBorder");

        _PART_LayoutBorder.PointerMoved += OnLayoutPointerMoved;
        _PART_LayoutBorder.PointerPressed += OnLayoutPointerPressed;
        _PART_LayoutBorder.PointerReleased += OnLayoutPointerReleased;
        _PART_LayoutBorder.PointerCaptureLost += OnLayoutPointerCaptureLost;
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == BarStateProperty) {
            Cancel();
            var (oldValue, newValue) = change.GetOldAndNewValue<BarState>();

            _barTransition.OldState = oldValue;
            _barTransition.NewState = newValue;

            await _barTransition.Start(_PART_LayoutBorder, _PART_ContentLayoutBorder, false, _cancellationTokenSource.Token);
            return;
        }
    }

    private void Cancel() {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
    }
}

public enum BarState {
    Collapsed,
    Expanded,
    Hidden
}