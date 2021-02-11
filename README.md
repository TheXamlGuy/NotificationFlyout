# Notification Flyout #
A Notification Flyout that looks and feels exactly like the native Volume, Network, and Battery flyouts. Built with WPF and UWP with XamlIslands using the UWP Flyout control for the displaying of the flyout content.

# Demo #
[![](http://img.youtube.com/vi/8EoZ4pGWTkY/0.jpg)](http://www.youtube.com/watch?v=8EoZ4pGWTkY "Notification flyout demo")

# Getting started #
Some parts of this guide refers to the [Host a custom WinRT XAML control in a WPF app using XAML Islands](https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/host-custom-control-with-xaml-islands) article. If in doubt, refer to the article, or post an issue on this repro.

## Create a UWP project ##
1. In Visual Studio 2019, create a new UWP app project project. Make sure the target version and minimum version are both set to Windows 10, version 1903 (Build 18362) or a later release.
2. In the UWP app project, install the [Microsoft.Toolkit.Win32.UI.XamlApplication NuGet package](https://www.nuget.org/packages/Microsoft.Toolkit.Win32.UI.XamlApplication) (latest stable version).
3. Open the `App.xaml` file and replace the contents of this file with the following XAML. Replace `MyUWPApp` with the namespace of your UWP app project.
```xaml
<xaml:XamlApplication
    x:Class="MyUWPApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:xaml="using:Microsoft.Toolkit.Win32.UI.XamlHost"
    xmlns:local="using:MyUWPApp">
</xaml:XamlApplication>
```
4. Open the `App.xaml.cs` file and replace the contents of this file with the following code. Replace `MyUWPApp` with the namespace of your UWP app project.
```c#
namespace MyUWPApp
{
    public sealed partial class App : Microsoft.Toolkit.Win32.UI.XamlHost.XamlApplication
    {
        public App()
        {
            this.Initialize();
        }
    }
}
```
5. In the UWP app project, install the [NotificationFlyout.Uwp.UI.Controls](https://www.nuget.org/packages/NotificationFlyout.Uwp.UI.Controls/) NuGet package (latest stable version).
3. Open the `MainPage.xaml` file and replace the contents of this file with the following XAML. Replace `MyUWPApp` with the namespace of your UWP app project.
```xaml
<controls:NotificationFlyout
    x:Class="MyUWPApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="using:NotificationFlyout.Uwp.UI.Controls"
    IconSource="/Assets/Icon.ico"
    LightIconSource="/Assets/Icon-Light.ico">
    <Grid Width="400" Height="500">
        <Button
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Content="Hello World!" />
    </Grid>
</controls:NotificationFlyout>
```
5. In the UWP app project, add two icons (.ico) to the Assets folder.
6. Replace the values of `IconSource` and `LightIconSource` with the paths of the icons that you have added to your Assets folder.
7. Clean the UWP app project and then build it.

## Create a WPF project ##
1. In Visual Studio 2019, create a new WPF App (.NET Core) project.
2. In the WPF app project, install the [Microsoft.Toolkit.Wpf.UI.XamlHost](https://www.nuget.org/packages/Microsoft.Toolkit.Wpf.UI.XamlHost) and [NotificationFlyout.Wpf.UI.Controls](https://www.nuget.org/packages/NotificationFlyout.Wpf.UI.Controls/) NuGet packages (latest stable version).
3. Open the `App.xaml` file and replace the contents of this file with the following XAML. Replace `MyWPFApp` with the namespace of your WPF app project.
```xaml
<Application
    x:Class="MyWPFApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources />
</Application>
```
4. Delete the MainWindow.xaml file from the WPF app project.
5. In your WPF project, right-click the project node, select Add -> New Item, and then select Class. Name the class Program and click Add.
6. Replace the generated `Program` class with the following code and then save the file. Replace `MyUWPApp` with the namespace of your UWP app project, and replace `MyWPFApp` with the namespace of your WPF app project.
```c#
using NotificationFlyout.Wpf.UI.Controls;
using System;

namespace MyWPFApp
{
    public class Program
    {
        [STAThread()]
        public static void Main()
        {
            using (new MyUWPApp.App())
            {
                MyWPFApp.App app = new MyWPFApp.App();
                new NotificationFlyoutApplication
                {
                    Flyout = new MainPage()
                };
                app.Run();
            }
        }
    }
}
```
7. Clean the WPF app project and then build it.
8. Run the WPF app.

# Limitations and workarounds #
All limitions found in this [article](https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/xaml-islands#limitations-and-workarounds) will affect how you build an app using this control. We have of course have added a work around for when the Windows theme is changed by the user 🎉!

# Credits #
Attributions to and inspiration for the ta)skbar API from the [Ear Trumpet project!](https://github.com/File-New-Project/EarTrumpet)
