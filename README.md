# OpenSilver Migration Examples

## HttpClient vs HttpWebRequest

This is an example of the most common functionality that requires a complete change. `WebRequest` functionality is not currently supported and are not recommended for use. It is necessary to rewrite the network request code to use `HttpClient`. An added benefit of the migration is the ability to use asynchronous functions.

## Workaround for `WritableBitmap`

The customer stored an array of pixels in a database and used the `WriteableBitmap.Pixels` property to write color information and display the image.

At the time of migration, the **OpenSilver** library lacked an implementation of the `WriteableBitmap` class. Therefore, to display the image, we used the ability to create a `BitmapImage` element from the **OpenSilver** library and pass it an image in a known format using the `Base64` representation of the data.

To simplify the implementation, the **BMP** format was chosen. The description of this format is available from MSDN. It was necessary to correctly generate the structure of the **BMP** file in memory and fill the structure with color data. The the **BMP** file created in memory is set to `BitmapImage` using the `SetSource` function:

```csharp
bitmapImage.SetSource(string.Format("data:image/bmp;base64, {0}", base64ContentOfFile));
```
The customer stored rather small images in the database, so the implemented approach did not affect performance.
Currently, an implementation of `WriteableBitmap` is available in the **OpenSilver** library.

## Upload Files

The `OpenFileDialog` implementation was missing in the **OpenSilver** library at the time of migration.
In order to allow the user to select a file and upload its contents to the server, a JS-based file opening function has been implemented.

Note: the file upload function implementation is currently available in the **OpenSilver** library (`OpenSilver.Controls.OpenFileDialog` class).

## Shapes Removing

The client used graphical primitives in some controls (e.g., to indicate the sorting direction or to draw arrows to indicate the presence of popup elements).

But the performance of drawing graphical primitives in **OpenSilver** is much lower than in **Silverlight**.
 
Therefore, in order to improve performance when rendering application pages, it was necessary to get rid of primitives. This was made possible by the large number of Unicode characters that can be used instead of primitives.
In some cases, `Border` elements have been used to replace `Rectangle` and `Ellipse` elements.

Now it is difficult to estimate how much this optimization has contributed to the overall reduction in page rendering time (as it was not the only optimization), but since it is easy to do the element replacement described, we have not given up on this optimization.

## Fix and Optimize Rectangle Fill

The colour picker element used the gradient fill in the `Rectangle` element to display the colour map. However, gradient fills for primitives do not work in **OpenSilver** library.

As part of the task of optimizing the page rendering time, it was decided to avoid using graphic primitives as much as possible. So we replaced the `Rectangle` element with the `Border` element.

This also solved the problem of displaying the colour map, as gradient fill works for the `Border` element in the **OpenSilver** library.

## Replace Animation With Gif

A complex animation of the loading process looked wrong during the migration. The customer preferred not to use such an animation and offered a solution in the form of an animated GIF file.

Currently, the animation is partially fixed in **OpenSilver** (as it was not visible at all at the time of migration), 
but nevertheless there is still no full compatibility with **Silverlight**.

## PropertyGrid Optimization

The customer used a `PropertyGrid` with a large number of elements to display the properties of different objects. In addition, the `PropertyGrid` could group properties into categories.

Whenever the active object or grouping changed, the entire content of the `PropertyGrid` was regenerated. As a result, this element was very slow in the migrated application.

To optimize it, we came up with an approach where UI elements were cached and only the `DataContext` of cached elements was replaced. This was possible because the same properties used for different objects had the same value type. The resulting solution significantly accelerated the process of building the `PropertyGrid`.

However, the time for the initial filling with properties is comparable to the time of filling time for a non-optimized `PropertyGrid`, because the UI elements are not yet in the cache. Nevertheless, the approach is considered successful because the application users work with many elements (often switching between them), so the optimization works.

## Remove Custom Cursor

The customer used a non-standard cursor to indicate the drag'n'drop operation. The cursor was implemented as an image on the canvas. Performance analysis showed that redrawing the canvas with the cursor every time the mouse was moved was a costly operation.

Since it was necessary to perform some calculations while moving the mouse, it became obvious that the non-standard cursor should be optimized. However, we could not find a suitable solution.

So we suggested that the customer drop the non-standard cursor altogether and use the standard cursor. The customer agreed and the drag'n'drop operation became faster.

## Fix Close Dialog In Loaded Event Handler

If we call the `Close()` method of the `ChildWindow` successor in the `Loaded` event handler, we will get a displayed but non-working dialog box in the migrated application. This happens because this window is not fully initialized.

To resolve this situation, the window should only be closed from the `Loaded`event handler using the `Dispatcher`:

```csharp
Dispatcher.BeginInvoke(() => Close());
```

## Objects Instantiation

The problem to solve was to speed up the creation of a large number of objects. To do this, instead of using `Activator.CreateInstance(...)`, the **&quot;Creating objects using Linq Expressions&quot;** approach was used.

The new approach works about twice as fast. Although we didn't get a significant enough speed-up (because objects are created quickly even using `Activator.CreateInstance(...)`), the solution we got still allows us to speed up the creation of a large number of objects.

## Optimize Items Search

The customer implemented a search for items in a large list. When the search criteria changed, he re-created the entire collection of items associated with the `ItemsSource` in the `ItemsControl` element. This was inefficient and slow.

To optimize the performance of the list during the search, we decided not to delete or add items, but to adjust the visibility of the items using the `Visibility` property. So we set the value of the `Visibility` property of the root item element to `Visibility.Visible` if it matched the search criteria, and to `Visibility.Collapsed` if it did not.

The performance of the list has increased manifold.
 
## ObservableCollection Management

The client used `ObservableCollection` to store user models. To display an up-to-date ordered list of users, he subscribed to the `CollectionChanged` event on the model collection and when that collection changed, he created (or recreated) a new collection of view models to assign as `ItemsSource` to the `ItemsControl` element.

This approach worked fast enough in **Silverlight**, but was inefficient for **OpenSilver**. There were unnecessary operations to remove and add user view models that were present in both the old and new sets. As a result, the list was updated very slowly.

The optimised solution uses `NotifyCollectionChangedAction` enumeration to understand what is happening with the items. It only adds and removes only the required items from the list, leaving the other items unchanged. This approach has significantly improved the performance of the user list.

