namespace Ecierge.Uno.Navigation;

using Microsoft.Xaml.Interactivity;

public abstract partial class NavigateRouteActionBase : DependencyObject
{
    #region Route

    /// <summary>
    /// Route Dependency Property
    /// </summary>
    public static readonly DependencyProperty RouteProperty =
        DependencyProperty.Register(nameof(Route), typeof(string), typeof(NavigateRouteActionBase),
            new((string?)null));

    /// <summary>
    /// Gets or sets the Route property. This dependency property
    /// indicates the route to navigate to.
    /// </summary>
    public string? Route
    {
        get { return (string?)GetValue(RouteProperty); }
        set { SetValue(RouteProperty, value); }
    }

    #endregion

    #region NavigationData

    /// <summary>
    /// NavigationData Dependency Property
    /// </summary>
    public static readonly DependencyProperty NavigationDataProperty =
        DependencyProperty.Register(nameof(NavigationData), typeof(INavigationData), typeof(NavigateRouteActionBase),
            new((INavigationData?)null));

    /// <summary>
    /// Gets or sets the NavigationData property. This dependency property
    /// indicates the route navigation data.
    /// </summary>
    public INavigationData? NavigationData
    {
        get { return (INavigationData?)GetValue(NavigationDataProperty); }
        set { SetValue(NavigationDataProperty, value); }
    }

    #endregion
}

public partial class NavigateRootRouteActionBase : NavigateRouteActionBase, IAction
{
    public object Execute(object sender, object parameter)
    {
        if (sender is FrameworkElement element)
        {
            var navigationRegion = element.FindNavigationRegion();
            if (navigationRegion is null) return NavigationResponse.Failed;
            return navigationRegion.Navigator.RootNavigator.NavigateRouteAsync(sender, Route!, NavigationData);
        }
        return NavigationResponse.Failed;
    }
}

public abstract partial class NavigateTargetRouteActionBase : NavigateRouteActionBase
{
    #region Target

    /// <summary>
    /// Target Dependency Property
    /// </summary>
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(FrameworkElement), typeof(NavigateSegmentActionBase), new((FrameworkElement?)null));

    /// <summary>
    /// Gets or sets the Target property. This dependency property
    /// indicates the FrameworkElement that has a navigation region.
    /// </summary>
    public FrameworkElement? Target
    {
        get { return (FrameworkElement?)GetValue(TargetProperty); }
        set { SetValue(TargetProperty, value); }
    }

    #endregion
}

public partial class NavigateLocalRouteAction : NavigateTargetRouteActionBase, IAction
{
    public object Execute(object sender, object parameter)
    {
        var target = Target ?? sender;

        if (target is FrameworkElement element)
        {
            var navigationRegion = element.FindNavigationRegion();
            if (navigationRegion is null) return NavigationResponse.Failed;
            return navigationRegion.Navigator.RootNavigator.NavigateRouteAsync(sender, Route!, NavigationData);
        }
        return NavigationResponse.Failed;
    }
}

public partial class NavigateNestedRouteAction : NavigateTargetRouteActionBase, IAction
{
    public object Execute(object sender, object parameter)
    {
        var target = Target ?? sender;
        if (target is FrameworkElement element)
        {
            var navigationRegion = element.FindNavigationRegion();
            if (navigationRegion is null) return NavigationResponse.Failed;
            return navigationRegion.Navigator.ChildNavigator!.NavigateRouteAsync(sender, Route!, NavigationData);
        }
        return NavigationResponse.Failed;
    }
}