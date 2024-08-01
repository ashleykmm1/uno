﻿#nullable enable

using System;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Uno.Foundation.Logging;
using Uno.UI;
using Uno.UI.DataBinding;
using Uno.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.System;

#if __ANDROID__
using Android.Views;
using _View = Android.Views.View;
#elif __IOS__
using UIKit;
using _View = UIKit.UIView;
#elif __MACOS__
using AppKit;
using _View = AppKit.NSView;
#else
using _View = Microsoft.UI.Xaml.FrameworkElement;
#endif

#if HAS_UNO_WINUI
#else
using WindowSizeChangedEventArgs = Windows.UI.Core.WindowSizeChangedEventArgs;
#endif

namespace Microsoft.UI.Xaml.Controls
{
	public partial class ComboBox : Selector
	{

		private bool _areItemTemplatesForwarded;

		private TextBox? m_tpEditableTextPart;
		private ContentPresenter? m_tpContentPresenterPart;

		private IPopup? _popup;
		private Popup? m_tpPopupPart;
		private Border? _popupBorder;
		private ContentPresenter? _contentPresenter;
		private TextBlock? _placeholderTextBlock;
		private ContentPresenter? m_tpHeaderContentPresenterPart;

		private DateTime m_timeSinceLastCharacterReceived;
		private string m_searchString = "";
		private int m_searchResultIndex = -1;
		private int m_indexForcedToUnselectedVisual = -1;
		private int m_indexForcedToSelectedVisual = -1;

		private bool _wasPointerPressed;

		/// <summary>
		/// The 'inline' parent view of the selected item within the dropdown list. This is only set if SelectedItem is a view type.
		/// </summary>
		private ManagedWeakReference? _selectionParentInDropdown;

		public ComboBox()
		{
			ResourceResolver.ApplyResource(this, LightDismissOverlayBackgroundProperty, "ComboBoxLightDismissOverlayBackground", isThemeResourceExtension: true, isHotReloadSupported: true);

			DefaultStyleKey = typeof(ComboBox);

			PrepareState();
		}

		internal bool IsSearchResultIndexSet()
		{
			return m_searchResultIndexSet;
		}

		internal int GetSearchResultIndex()
		{
			return m_searchResultIndex;
		}

		protected override DependencyObject GetContainerForItemOverride() => new ComboBoxItem { IsGeneratedContainer = true };

		protected override bool IsItemItsOwnContainerOverride(object item) => item is ComboBoxItem;

		protected override void OnApplyTemplate()
		{
			if (IsEditable)
			{
				DisableEditableMode();
			}

			base.OnApplyTemplate();

			if (_popup is Popup oldPopup)
			{
				oldPopup.CustomLayouter = null;
			}

			_popup = this.GetTemplateChild("Popup") as IPopup;
			m_tpPopupPart = _popup as Popup;
			_popupBorder = this.GetTemplateChild("PopupBorder") as Border;
			_contentPresenter = this.GetTemplateChild("ContentPresenter") as ContentPresenter;
			m_tpContentPresenterPart = _contentPresenter;
			_placeholderTextBlock = this.GetTemplateChild("PlaceholderTextBlock") as TextBlock;

			if (IsEditable)
			{
				m_tpEditableTextPart = this.GetTemplateChild("EditableText") as TextBox;
			}

			if (_popup is Popup popup)
			{
				//TODO Uno specific: Ensures popup does not take focus when opened.
				//This can be removed when the actual ComboBox code is fully ported
				//from WinUI.
				if (_popupBorder is { } border)
				{
					border.AllowFocusOnInteraction = false;
				}

				popup.CustomLayouter = new DropDownLayouter(this, popup);

				popup.IsLightDismissEnabled = true;

				popup.BindToEquivalentProperty(this, nameof(LightDismissOverlayMode));
				popup.BindToEquivalentProperty(this, nameof(LightDismissOverlayBackground));
			}

			//Initialize header visibility
			UpdateHeaderPresenterVisibility();

			UpdateContentPresenter();
			UpdateDescriptionVisibility(true);

			if (_contentPresenter != null)
			{
				_contentPresenter.SynchronizeContentWithOuterTemplatedParent = false;

				var thisRef = (this as IWeakReferenceProvider).WeakReference;
				_contentPresenter.DataContextChanged += (snd, evt) =>
				{
					if (thisRef.Target is ComboBox that)
					{
						// The ContentPresenter will automatically clear its local DataContext
						// on first load.
						//
						// When there's no selection, this will cause this ContentPresenter to
						// received the same DataContext as the ComboBox itself, which could
						// lead to strange result or errors.
						//
						// See comments in ContentPresenter.ResetDataContextOnFirstLoad() method.
						// Fixed in this PR: https://github.com/unoplatform/uno/pull/1465

						if (evt.NewValue != null && that.SelectedItem == null && that._contentPresenter != null)
						{
							that._contentPresenter.DataContext = null; // Remove problematic inherited DataContext
						}
					}
				};
			}

			if (IsEditable)
			{
				SetupEditableMode();
				CreateEditableContentPresenterTextBlock();
			}

			//if (IsInline)
			//{
			//	ForceApplyInlineLayoutUpdate();
			//}

			ChangeVisualState(false);
		}


		// Selects the next item in the list.
		private void SelectNext(ref int index)
		{
			var count = Items.Count;
			if (count > 0)
			{
				int internalSelectedIndex = index + 1;
				if (internalSelectedIndex <= count - 1)
				{
					SelectItemHelper(ref internalSelectedIndex, 1);
					if (internalSelectedIndex != -1)
					{
						index = internalSelectedIndex;
					}
				}
			}
		}

		// Selects the previous item in the list.
		private void SelectPrev(ref int index)
		{
			var count = Items.Count;
			if (count > 0)
			{
				int internalSelectedIndex = index - 1;
				if (internalSelectedIndex >= 0)
				{
					SelectItemHelper(ref internalSelectedIndex, -1);
					if (internalSelectedIndex != -1)
					{
						index = internalSelectedIndex;
					}
				}
			}
		}

		// Given a direction, searches through list for next available item to select.
		private void SelectItemHelper(ref int index, int increment)
		{
			var items = Items;
			var count = items.Count;
			bool isSelectable = false;

			for (; index > -1 && index < count; index += increment)
			{
				var item = items[index];
				isSelectable = IsSelectableHelper(item);
				if (isSelectable)
				{
					var container = ContainerFromIndex(index);
					isSelectable = IsSelectableHelper(container);
					if (isSelectable)
					{
						break;
					}
				}
			}

			if (!isSelectable)
			{
				// If no selectable item was found, set index to -1 so selection will not be updated.
				index = -1;
			}
		}

		private protected override void OnLoaded()
		{
			base.OnLoaded();

			UpdateDropDownState();

			if (_popup != null)
			{
				_popup.Closed += OnPopupClosed;
				_popup.Opened += OnPopupOpened;
			}

			if (XamlRoot is null)
			{
				throw new InvalidOperationException("XamlRoot must be set on Loaded");
			}

			XamlRoot.Changed += OnXamlRootChanged;
		}

		private protected override void OnUnloaded()
		{
			base.OnUnloaded();

			if (_popup != null)
			{
				_popup.Closed -= OnPopupClosed;
				_popup.Opened -= OnPopupOpened;
			}

			if (XamlRoot is not null)
			{
				XamlRoot.Changed -= OnXamlRootChanged;
			}
		}

		protected virtual void OnDropDownClosed(object e)
		{
			DropDownClosed?.Invoke(this, null!);
		}

		protected virtual void OnDropDownOpened(object e)
		{
			DropDownOpened?.Invoke(this, null!);
		}

		private void OnXamlRootChanged(object sender, XamlRootChangedEventArgs e)
		{
			IsDropDownOpen = false;
		}

		private void OnPopupOpened(object? sender, object e)
		{
			IsDropDownOpen = true;
		}

		private void OnPopupClosed(object? sender, object e)
		{
			IsDropDownOpen = false;
		}

		private void UpdateDescriptionVisibility(bool initialization)
		{
			if (initialization && Description == null)
			{
				// Avoid loading DescriptionPresenter element in template if not needed.
				return;
			}

			var descriptionPresenter = this.FindName("DescriptionPresenter") as ContentPresenter;
			if (descriptionPresenter != null)
			{
				descriptionPresenter.Visibility = Description != null ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		internal override void OnSelectedItemChanged(object oldSelectedItem, object selectedItem, bool updateItemSelectedState)
		{
			if (oldSelectedItem is _View view)
			{
				// Ensure previous SelectedItem is put back in the dropdown list if it's a view
				RestoreSelectedItem(view);
			}

			base.OnSelectedItemChanged(
				oldSelectedItem: oldSelectedItem,
				selectedItem: selectedItem,
				updateItemSelectedState: updateItemSelectedState);

			UpdateContentPresenter();

			if (updateItemSelectedState)
			{
				TryUpdateSelectorItemIsSelected(oldSelectedItem, false);
				TryUpdateSelectorItemIsSelected(selectedItem, true);
			}
		}

		protected override void OnPointerCanceled(PointerRoutedEventArgs e)
		{
			base.OnPointerCanceled(e);
			m_bIsPressed = false;
			m_IsPointerOverMain = false;

			UpdateVisualState();
		}

		internal override void OnItemClicked(int clickedIndex, VirtualKeyModifiers modifiers)
		{
			base.OnItemClicked(clickedIndex, modifiers);
			IsDropDownOpen = false;
		}

		private void UpdateContentPresenter()
		{
			if (_contentPresenter == null) return;

			if (SelectedItem != null)
			{
				var item = GetSelectionContent();
				var itemView = item as _View;

				if (itemView != null)
				{
#if __ANDROID__
					var comboBoxItem = itemView.FindFirstParentOfView<ComboBoxItem>();
#else
					var comboBoxItem = itemView.FindFirstParent<ComboBoxItem>();
#endif
					if (comboBoxItem != null)
					{
						// Keep track of the former parent, so we can put the item back when the dropdown is shown
						_selectionParentInDropdown = (itemView.GetVisualTreeParent() as IWeakReferenceProvider)?.WeakReference;
					}
				}
				else
				{
					_selectionParentInDropdown = null;
				}

				var displayMemberPath = DisplayMemberPath;
				if (string.IsNullOrEmpty(displayMemberPath))
				{
					_contentPresenter.Content = item;
				}
				else
				{
					var b = new BindingPath(displayMemberPath, item) { DataContext = item };
					_contentPresenter.Content = b.Value;
				}

				if (itemView != null && itemView.GetVisualTreeParent() != _contentPresenter)
				{
					// Item may have been put back in list, reattach it to _contentPresenter
					_contentPresenter.AddChild(itemView);
				}
				if (!_areItemTemplatesForwarded)
				{
					SetContentPresenterBinding(ContentPresenter.ContentTemplateProperty, nameof(ItemTemplate));
					SetContentPresenterBinding(ContentPresenter.ContentTemplateSelectorProperty, nameof(ItemTemplateSelector));

					_areItemTemplatesForwarded = true;
				}
			}
			else
			{
				_contentPresenter.Content = _placeholderTextBlock;
				if (_areItemTemplatesForwarded)
				{
					_contentPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
					_contentPresenter.ClearValue(ContentPresenter.ContentTemplateSelectorProperty);

					_areItemTemplatesForwarded = false;
				}
			}

			void SetContentPresenterBinding(DependencyProperty targetProperty, string sourcePropertyPath)
			{
				_contentPresenter?.SetBinding(targetProperty, new Binding(sourcePropertyPath) { RelativeSource = RelativeSource.TemplatedParent });
			}
		}

		private object? GetSelectionContent()
		{
			return SelectedItem is ComboBoxItem cbi ? cbi.Content : SelectedItem;
		}

		private void RestoreSelectedItem()
		{
			var selection = GetSelectionContent();
			if (selection is _View selectionView)
			{
				RestoreSelectedItem(selectionView);
			}
		}

		/// <summary>
		/// Restore SelectedItem (or former SelectedItem) view to its position in the dropdown list.
		/// </summary>
		private void RestoreSelectedItem(_View selectionView)
		{
			var dropdownParent = _selectionParentInDropdown?.Target as FrameworkElement;
#if __ANDROID__
			var comboBoxItem = dropdownParent?.FindFirstParentOfView<ComboBoxItem>();
#else
			var comboBoxItem = dropdownParent?.FindFirstParent<ComboBoxItem>();
#endif

			// Sanity check, ensure parent is still valid (ComboBoxItem may have been recycled)
			if (dropdownParent != null
				&& comboBoxItem?.Content == selectionView
				&& selectionView.GetVisualTreeParent() != dropdownParent)
			{
				dropdownParent.AddChild(selectionView);
			}
		}

		private protected override void OnIsEnabledChanged(IsEnabledChangedEventArgs e)
		{
			base.OnIsEnabledChanged(e);

			UpdateVisualState(true);
		}

		partial void OnIsDropDownOpenChangedPartial(bool oldIsDropDownOpen, bool newIsDropDownOpen)
		{
			if (_popup != null)
			{
				// This method will load the itempresenter children
#if __ANDROID__
				SetItemsPresenter((_popup.Child as ViewGroup).FindFirstChild<ItemsPresenter>());
#elif __IOS__ || __MACOS__
				SetItemsPresenter(_popup.Child.FindFirstChild<ItemsPresenter>());
#endif

				_popup.IsOpen = newIsDropDownOpen;
			}

			var args = new RoutedEventArgs() { OriginalSource = this };
			if (newIsDropDownOpen)
			{
				// Force a refresh of the popup's ItemPresenter
				Refresh();

				OnDropDownOpened(args);

				RestoreSelectedItem();

				var index = SelectedIndex;
				index = index == -1 ? 0 : index;
				if (ContainerFromIndex(index) is ComboBoxItem container)
				{
					container.Focus(FocusState.Programmatic);
				}
			}
			else
			{
				OnDropDownClosed(args);
				UpdateContentPresenter();
			}

			UpdateDropDownState();
			ChangeVisualState(true);
		}

		protected override void OnKeyDown(KeyRoutedEventArgs args)
		{
			base.OnKeyDown(args);

			if (!args.Handled)
			{
				args.Handled = TryHandleKeyDown(args, null);
			}

			// Temporary as Uno doesn't yet implement CharacterReceived event.
			UnoOnCharacterReceived(args);
		}

		internal bool TryHandleKeyDown(KeyRoutedEventArgs args, ComboBoxItem? focusedContainer)
		{
			if (!IsEnabled)
			{
				return false;
			}

			if (args.Key == VirtualKey.Enter ||
				args.Key == VirtualKey.Space)
			{
				if (IsDropDownOpen)
				{
					// If we got a space while in searching mode, we shouldn't close the dropdown.
					if (!(args.Key == VirtualKey.Space && IsInSearchingMode()) && SelectedIndex > -1)
					{
						IsDropDownOpen = false;
						return true;
					}
				}
				else
				{
					IsDropDownOpen = true;
					return true;
				}
			}
			else if (args.Key == VirtualKey.Escape)
			{
				if (IsDropDownOpen)
				{
					IsDropDownOpen = false;
					return true;
				}
			}
			else if (args.Key == VirtualKey.Down)
			{
				if (IsDropDownOpen)
				{
					return TryMoveKeyboardFocus(+1, focusedContainer);
				}
				else
				{
					if (IsIndexValid(SelectedIndex + 1))
					{
						SelectedIndex = SelectedIndex + 1;
						return true;
					}
				}
			}
			else if (args.Key == VirtualKey.Up)
			{
				if (IsDropDownOpen)
				{
					return TryMoveKeyboardFocus(-1, focusedContainer);
				}
				else
				{
					if (IsIndexValid(SelectedIndex - 1))
					{
						SelectedIndex = SelectedIndex - 1;
						return true;
					}
				}
			}
			else if (args.Key == VirtualKey.Tab)
			{
				if (_popup is { } p)
				{
					p.IsOpen = false;
				}
				// Don't handle. Let VisualTree.RootElement deal with focus management
			}
			return false;
		}

		// Uno TODO: This should override OnCharacterReceived when it's implemented.
		private void UnoOnCharacterReceived(KeyRoutedEventArgs args)
		{
			if (IsTextSearchEnabled)
			{
				if (args.UnicodeKey is not { } keyCode)
				{
					return;
				}

				ProcessSearch(keyCode);
			}
		}

		private bool HasSearchStringTimedOut()
		{
			const int timeOutInMilliseconds = 1000;

			var now = DateTime.UtcNow;

			return (now - m_timeSinceLastCharacterReceived).TotalMilliseconds > timeOutInMilliseconds;
		}

		private string TryGetStringValue(object @object/*, PropertyPathListener pathListener*/)
		{
			object spBoxedValue;
			object spObject = @object;

			if (spObject is ICustomPropertyProvider spObjectPropertyAccessor)
			{
				//if (pathListener != null)
				//{
				//	// Our caller has provided us with a PropertyPathListener. By setting the source of the listener, we can pull a value out.
				//	// This is our boxedValue, which we effectively ToString below.
				//	pathListener.SetSource(spObject));
				//	spBoxedValue = pathListener.GetValue();
				//}
				//else
				{
					// No PathListener specified, but this object implements
					// ICustomPropertyProvider. Call .ToString on the object:
					return spObjectPropertyAccessor.GetStringRepresentation();
				}
			}
			else
			{
				// Try to get the string value by unboxing the object itself.
				spBoxedValue = spObject;
			}

			if (spBoxedValue != null)
			{
				if (spBoxedValue is IStringable spStringable)
				{
					// We've set a BoxedValue. If it is castable to a string, try to ToString it.
					return spStringable.ToString();
				}
				else
				{
					return spBoxedValue.ToString()!;
					// We've set a BoxedValue, but we can't directly ToString it. Try to get a string out of it.
				}
			}
			else
			{
				// If we haven't found a BoxedObject and it's not Stringable, try one last time to get a string out.
				return @object.ToString()!;
			}
		}

		private bool TryMoveKeyboardFocus(int offset, ComboBoxItem? focusedContainer)
		{
			var focusedIndex = SelectedIndex;
			if (focusedContainer != null)
			{
				focusedIndex = IndexFromContainer(focusedContainer);
			}

			var index = focusedIndex + offset;
			if (!IsIndexValid(index))
			{
				return false;
			}

			var container = ContainerFromIndex(index);
			if (container is not ComboBoxItem item)
			{
				return false;
			}

			item.StartBringIntoView(new BringIntoViewOptions()
			{
				AnimationDesired = false
			});
			item.Focus(FocusState.Keyboard);
			return true;
		}

		private bool IsIndexValid(int index) => index >= 0 && index < NumberOfItems;

		/// <summary>
		/// Stretches the opened Popup horizontally, and uses the VerticalAlignment
		/// of the first child for positioning.
		/// </summary>
		/// <remarks>
		/// This is required by some apps trying to emulate the native iPhone look for ComboBox.
		/// The standard popup layouter works like on Windows, and doesn't stretch to take the full size of the screen.
		/// </remarks>
		public bool IsPopupFullscreen { get; set; }

		private void UpdateDropDownState()
		{
			var state = IsDropDownOpen ? "Opened" : "Closed";
			VisualStateManager.GoToState(this, state, true);
		}

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new ComboBoxAutomationPeer(this);
		}

		public LightDismissOverlayMode LightDismissOverlayMode
		{
			get
			{
				return (LightDismissOverlayMode)this.GetValue(LightDismissOverlayModeProperty);
			}
			set
			{
				this.SetValue(LightDismissOverlayModeProperty, value);
			}
		}

		public static DependencyProperty LightDismissOverlayModeProperty { get; } =
		DependencyProperty.Register(
			"LightDismissOverlayMode", typeof(LightDismissOverlayMode),
			typeof(ComboBox),
			new FrameworkPropertyMetadata(default(LightDismissOverlayMode)));

		/// <summary>
		/// Sets the light-dismiss colour, if the overlay is enabled. The external API for modifying this is to override the PopupLightDismissOverlayBackground, etc, static resource values.
		/// </summary>
		internal Brush LightDismissOverlayBackground
		{
			get { return (Brush)GetValue(LightDismissOverlayBackgroundProperty); }
			set { SetValue(LightDismissOverlayBackgroundProperty, value); }
		}

		internal static DependencyProperty LightDismissOverlayBackgroundProperty { get; } =
			DependencyProperty.Register("LightDismissOverlayBackground", typeof(Brush), typeof(ComboBox), new FrameworkPropertyMetadata(null));

		private class DropDownLayouter : Popup.IDynamicPopupLayouter
		{
			private ManagedWeakReference _combo;
			private ManagedWeakReference _popup;

			private ComboBox? Combo => _combo.Target as ComboBox;
			private Popup? Popup => _popup.Target as Popup;

			public DropDownLayouter(ComboBox combo, Popup popup)
			{
				_combo = (combo as IWeakReferenceProvider).WeakReference;
				_popup = (popup as IWeakReferenceProvider).WeakReference;
			}

			/// <inheritdoc />
			public Size Measure(Size available, Size visibleSize)
			{
				var popup = Popup;
				var combo = Combo;

				if (!(popup?.Child is FrameworkElement child) || combo == null)
				{
					return new Size();
				}

				// Inject layouting constraints
				// Note: Even if this is ugly (as we should never alter properties of a random child like this),
				//		 it's how UWP behaves (but it does that only if the child is a Border, otherwise everything is messed up).
				//		 It sets (at least) those properties :
				//			MinWidth
				//			MinHeight
				//			MaxWidth
				//			MaxHeight

				if (combo.IsPopupFullscreen)
				{
					// In full screen mode, we want the popup to stretch horizontally, so we set MinWidth and MaxWidth to the available width.
					// However, we don't want it to stretch vertically.
					// We want the height to exactly show all the items, i.e, combo.ActualHeight * combo.Items.Count. However, we want to limit that by
					// both MaxDropDownHeight and visible height (quite similar to non-fullscreen mode).
					// This also allows the child to set MinHeight and provide a VerticalAlignment
					var maxHeight = Math.Min(visibleSize.Height, Math.Min(combo.MaxDropDownHeight, combo.ActualHeight * combo.Items.Count));

					child.MinWidth = available.Width;
					child.MaxWidth = available.Width;
					child.MinHeight = combo.ActualHeight;
					child.MaxHeight = maxHeight;
				}
				else
				{
					// Set the popup child as max 9 x the height of the combo
					// (UWP seams to actually limiting to 9 visible items ... which is not necessarily the 9 x the combo height)
					var maxHeight = Math.Min(visibleSize.Height, Math.Min(combo.MaxDropDownHeight, combo.ActualHeight * _itemsToShow));

					child.MinHeight = combo.ActualHeight;
					child.MinWidth = combo.ActualWidth;
					child.MaxHeight = maxHeight;
					child.MaxWidth = visibleSize.Width;

					if (UsesManagedLayouting)
					// This is a breaking change for Android/iOS in some specialized cases (see ComboBox_VisibleBounds sample), and
					// since the layouting on those platforms is not yet as aligned with UWP as on WASM/Skia, and in particular
					// virtualizing panels aren't used in the ComboBox yet (#556 and #1133), we skip it for now
					{
#pragma warning disable CS0162 // Unreachable code detected
						child.HorizontalAlignment = HorizontalAlignment.Left;
						child.VerticalAlignment = VerticalAlignment.Top;
#pragma warning restore CS0162 // Unreachable code detected
					}
				}

				child.Measure(visibleSize);

				return child.DesiredSize;
			}

			private const int _itemsToShow = 9;

			/// <inheritdoc />
			public void Arrange(Size finalSize, Rect visibleBounds, Size desiredSize)
			{
				var popup = Popup;
				var combo = Combo;

				if (!(popup?.Child is FrameworkElement child) || combo == null)
				{
					return;
				}

				if (combo.IsPopupFullscreen)
				{
					Point getChildLocation()
					{
						switch (child.VerticalAlignment)
						{
							default:
							case VerticalAlignment.Top:
								return new Point();
							case VerticalAlignment.Bottom:
								return new Point(0, finalSize.Height - child.DesiredSize.Height);
						}
					}

					var childSize = new Size(finalSize.Width, Math.Min(finalSize.Height, child.DesiredSize.Height));
					var finalRect = new Rect(getChildLocation(), childSize);

					if (this.Log().IsEnabled(LogLevel.Debug))
					{
						this.Log().Debug($"FullScreen Layout for dropdown (desired: {desiredSize} / available: {finalSize} / visible: {visibleBounds} / finalRect: {finalRect} )");
					}

					child.Arrange(finalRect);

					return;
				}

				var comboRect = combo.GetAbsoluteBoundsRect();
				var frame = new Rect(comboRect.Location, desiredSize.AtMost(visibleBounds.Size));

				// On windows, the popup is Y-aligned accordingly to the selected item in order to keep
				// the selected at the same place no matter if the drop down is open or not.
				// For instance if selected is:
				//  * the first option: The drop-down appears below the combobox
				//  * the last option: The dop-down appears above the combobox
				// However this would requires us to determine the actual location of the SelectedItem container's
				// which might not be ready at this point (we could try a 2-pass arrange), and to scroll into view to make it visible.
				// So for now we only rely on the SelectedIndex and make a highly improvable vertical alignment based on it.

				var itemsCount = combo.NumberOfItems;
				var selectedIndex = combo.SelectedIndex;
				if (selectedIndex < 0 && itemsCount > 0)
				{
					selectedIndex = itemsCount / 2;
				}

				var unoPlacement = Uno.UI.Xaml.Controls.ComboBox.GetDropDownPreferredPlacement(combo);
				var winUIPlacement = popup.DesiredPlacement;
				if (unoPlacement == DropDownPlacement.Auto)
				{
					// If the Uno placement is Auto, we use the WinUI placement
					unoPlacement = winUIPlacement switch
					{
						PopupPlacementMode.Auto => DropDownPlacement.Auto,
						PopupPlacementMode.Bottom => DropDownPlacement.Below,
						PopupPlacementMode.Top => DropDownPlacement.Above,
						_ => DropDownPlacement.Centered
					};
				}

				var stickyThreshold = Math.Max(1, Math.Min(4, (itemsCount / 2) - 1));
				switch (unoPlacement)
				{
					case DropDownPlacement.Below:
						frame.Y = comboRect.Bottom;
						break;
					case DropDownPlacement.Auto when selectedIndex >= 0 && selectedIndex < stickyThreshold:
						frame.Y = comboRect.Top;
						break;
					case DropDownPlacement.Above:
						frame.Y = comboRect.Top - frame.Height;
						break;
					case DropDownPlacement.Auto when
							selectedIndex >= 0 && selectedIndex >= itemsCount - stickyThreshold
							// As we don't scroll into view to the selected item, this case seems awkward if the selected item
							// is not directly visible (i.e. without scrolling) when the drop-down appears.
							// So if we detect that we should had to scroll to make it visible, we don't try to appear above!
							&& (itemsCount <= _itemsToShow && frame.Height < (combo.ActualHeight * _itemsToShow) - 3):
						frame.Y = comboRect.Bottom - frame.Height;
						break;
					case DropDownPlacement.Centered:
					case DropDownPlacement.Auto: // For now we don't support other alignments than top/bottom/center, but on UWP auto can also be 2/3 - 1/3
					default:
						// Try to appear centered
						frame.Y = comboRect.Top - (frame.Height / 2.0) + (comboRect.Height / 2.0);
						break;
				}

				frame.X += popup.HorizontalOffset;
				frame.Y += popup.VerticalOffset;

				// Make sure that the popup does not appears out of the viewport
				if (frame.Left < visibleBounds.Left)
				{
					frame.X = visibleBounds.X;
				}
				else if (frame.Right > visibleBounds.Width)
				{
					// On UWP, the popup is just aligned to the right on the window if it overflows on right
					// Note: frame.Width is already at most visibleBounds.Width
					frame.X = visibleBounds.Width - frame.Width;
				}
				if (frame.Top < visibleBounds.Top)
				{
					frame.Y = visibleBounds.Y;
				}
				else if (frame.Bottom > visibleBounds.Height)
				{
					// On UWP, the popup always let 1 px free at the bottom
					// Note: frame.Height is already at most visibleBounds.Height
					frame.Y = visibleBounds.Height - frame.Height - 1;
				}

				if (this.Log().IsEnabled(LogLevel.Debug))
				{
					this.Log().Debug($"Layout the combo's dropdown at {frame} (desired: {desiredSize} / available: {finalSize} / visible: {visibleBounds} / selected: {selectedIndex} of {itemsCount})");
				}

#if __ANDROID__
				// Check whether the status bar is translucent
				// If so, we may need to compensate for the origin location
				// TODO: Adjust for multiwindow #13827
				var isTranslucent = Window.CurrentSafe!.IsStatusBarTranslucent();
				var allowUnderStatusBar = FeatureConfiguration.ComboBox.AllowPopupUnderTranslucentStatusBar;
				if (isTranslucent && allowUnderStatusBar)
				{
					var offset = visibleBounds.Location;
					frame.X -= offset.X;
					frame.Y -= offset.Y;
				}
#endif

				child.Arrange(frame);
			}
		}
	}
}
