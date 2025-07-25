﻿using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Media.Easings;
using WonderLab.Utilities;

namespace WonderLab.Media.Behaviors;

public sealed class ListBoxAnimationBehavior : Behavior<ListBoxItem> {
    protected override async void OnLoaded() {
        var compositionVisual = ElementComposition.GetElementVisual(AssociatedObject);
        compositionVisual.Opacity = 0;

        await Task.Delay(150);
        IChildIndexProvider childIndexProvider;

        if (AssociatedObject?.GetVisualParent() is IChildIndexProvider provider)
            childIndexProvider = provider;
        else
            throw new InvalidOperationException("The control must be a child of IChildIndexProvider");

        var delay = TimeSpan.FromMilliseconds(childIndexProvider.GetChildIndex(AssociatedObject) * 75);
        var xPoint = compositionVisual.Offset.X;
        var yPoint = compositionVisual.Offset.Y;
        var height = AssociatedObject.Bounds.Height + yPoint;

        var offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
            new((float)xPoint, (float)height * 2, 0),
            new((float)xPoint, (float)yPoint, 0), TimeSpan.FromSeconds(0.4), new WonderBackEaseOut() {
                Amplitude = Amplitude.Strong
            }, delay);

        var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(compositionVisual, 0, 1,
            TimeSpan.FromSeconds(0.6), new CubicEaseOut(), delay);

        offsetAni.Target = CompositionAnimationUtil.PROPERTY_OFFSET;
        opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

        var aniGroup = compositionVisual.Compositor.CreateAnimationGroup();

        aniGroup.Add(offsetAni);
        aniGroup.Add(opacityAni);

        compositionVisual.StartAnimationGroup(aniGroup);
    }
}

public sealed class ListBoxTagAnimationBehavior : Behavior<Control> {
    protected override async void OnLoaded() {
        var compositionVisual = ElementComposition.GetElementVisual(AssociatedObject);
        compositionVisual.Opacity = 0;

        await Task.Delay(150);
        var delay = TimeSpan.FromMilliseconds(300);
        var xPoint = compositionVisual.Offset.X;
        var yPoint = compositionVisual.Offset.Y;
        var height = AssociatedObject.Bounds.Height + yPoint;

        var offsetAni = CompositionAnimationUtil.CreateVector3Animation(compositionVisual,
            new((float)xPoint, (float)height * 2, 0),
            new((float)xPoint, (float)yPoint, 0), TimeSpan.FromSeconds(0.4), new WonderBackEaseOut() {
                Amplitude = Amplitude.Strong
            }, delay);

        var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(compositionVisual, 0, 1,
            TimeSpan.FromSeconds(0.6), new CubicEaseOut(), delay);

        offsetAni.Target = CompositionAnimationUtil.PROPERTY_OFFSET;
        opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

        var aniGroup = compositionVisual.Compositor.CreateAnimationGroup();

        aniGroup.Add(offsetAni);
        aniGroup.Add(opacityAni);
        compositionVisual.StartAnimationGroup(aniGroup);
    }
}