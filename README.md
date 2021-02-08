## GIF
<html>
  <table style="width:100%">
    <tr>
      <th>SideContextMenuView</th>
      <th>SwipeActionContextMenuView (Move to delete)</th> 
      <th>SwipeActionContextMenuView (Autoclosing)</th>
    </tr>
    <tr>
      <td><img src="https://github.com/AndreiMisiukevich/ContextMenu/blob/master/files/1.gif?raw=true"></td>
      <td><img src="https://github.com/AndreiMisiukevich/ContextMenu/blob/master/files/2.gif?raw=true"></td>
      <td><img src="https://github.com/AndreiMisiukevich/ContextMenu/blob/master/files/3.gif?raw=true"></td>
    </tr>
  </table>
</html>

## Setup
* Available on NuGet: [ContextViewCell](http://www.nuget.org/packages/ContextViewCell) [![NuGet](https://img.shields.io/nuget/v/ContextViewCell.svg?label=NuGet)](https://www.nuget.org/packages/ContextViewCell)
* Add nuget package to your Xamarin.Forms .netStandard/PCL project and to your platform-specific projects (iOS and Android)
* Call ```ContextMenuViewRenderer.Preserve()``` in AppDelagate for **iOS** and MainActivity for **Android**

|Platform|Version|
| ------------------- | ------------------- |
|Xamarin.iOS|8.0+|
|Xamarin.Android|15+|


## Samples

All samples are here: https://github.com/AndreiMisiukevich/ContextMenu/tree/master/ContextMenuSample/ContextMenuSample/Pages

Collection view is supported!
**XAML:**
```xml
<CollectionView x:Name="CollectionView"
                    ItemsSource="{Binding Items}"
                    Margin="0, 5, 0, 0"
                    BackgroundColor="White">
        <CollectionView.ItemTemplate>
            <DataTemplate>
                <context:SideContextMenuView IsAutoCloseEnabled="true">
                        <context:SideContextMenuView.View>
                            <Frame BackgroundColor="#512DA8"
                                   Margin="15,5"
                                   Padding="20"
                                   WidthRequest="{Binding Source={x:Reference CollectionView}, Path=Width, Converter={StaticResource MenuFitWidthConverter}, ConverterParameter='70'}"
                                   CornerRadius="10">
                                <StackLayout Orientation="Horizontal" 
                                             HorizontalOptions="FillAndExpand"
                                                                                Opacity="{Binding IsMuted, Converter={StaticResource IsMutedToOpacityConverter}}"
                                             Spacing="10">
                                    <Frame HeightRequest="80"
                                           WidthRequest="80"
                                           CornerRadius="40"
                                           Padding="0"
                                           VerticalOptions="CenterAndExpand"
                                           IsClippedToBounds="true">
                                        <Image Source="{Binding AvatarUrl}" 
                                               Aspect="AspectFill"/>
                                    </Frame>
                                    <StackLayout VerticalOptions="CenterAndExpand"
                                                 HorizontalOptions="CenterAndExpand"
                                                 Spacing="0">
                                        <Label Text="Chat with:"
                                               FontAttributes="Bold"
                                               TextColor="White"/>
                                        <Label Text="{Binding Name}" 
                                               FontSize="Large"
                                               TextColor="White"/>
                                    </StackLayout>
                                </StackLayout>                             
                            </Frame>
                        </context:SideContextMenuView.View>
                        <context:SideContextMenuView.ContextTemplate>
                            <DataTemplate>
                                <Frame Margin="0, 5"
                                       Padding="0"
                                       CornerRadius="10"
                                       IsClippedToBounds="true"
                                       BackgroundColor="Gold">
                                    <StackLayout 
                                             Padding="10, 5"
                                             Orientation="Horizontal"
                                             Spacing="10"
                                             Margin="0, 5">
                                        <ImageButton Source="trash.png" HeightRequest="60" WidthRequest="60" VerticalOptions="CenterAndExpand" HorizontalOptions="EndAndExpand" 
                                                     Command="{Binding BindingContext.DeleteCommand, Source={x:Reference CollectionView}}"
                                                     CommandParameter="{Binding .}"/>
                                        <ImageButton Source="mute.png" HeightRequest="60" WidthRequest="60" VerticalOptions="CenterAndExpand" HorizontalOptions="EndAndExpand" 
                                                     Command="{Binding BindingContext.MuteCommand, Source={x:Reference CollectionView}}"
                                                     CommandParameter="{Binding .}"/>
                                    </StackLayout>
                                </Frame>
                            </DataTemplate>
                        </context:SideContextMenuView.ContextTemplate>  
                    </context:SideContextMenuView>
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
```
*Make sure your main view width equals list's width*
You can adjust it by binding

```xml
...
<CollectionView x:Name="Collection"
        <CollectionView.ItemTemplate>
            <DataTemplate>
                <context:SideContextMenuView>
                    <context:SideContextMenuView.View>
                        <ContentView WidthRequest="{Binding Source={x:Reference Collection}, Path=Width}">
                            ...
```


Check source code for more info, or ***just ask me =)***

## License
The MIT License (MIT) see [License file](LICENSE)

## Contribution
Feel free to create issues and PRs 😃

