#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Xaml.Media
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class TimelineMarkerRoutedEventArgs : global::Windows.UI.Xaml.RoutedEventArgs
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Xaml.Media.TimelineMarker Marker
		{
			get
			{
				throw new global::System.NotImplementedException("The member TimelineMarker TimelineMarkerRoutedEventArgs.Marker is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=TimelineMarker%20TimelineMarkerRoutedEventArgs.Marker");
			}
			set
			{
				global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Xaml.Media.TimelineMarkerRoutedEventArgs", "TimelineMarker TimelineMarkerRoutedEventArgs.Marker");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public TimelineMarkerRoutedEventArgs() : base()
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Xaml.Media.TimelineMarkerRoutedEventArgs", "TimelineMarkerRoutedEventArgs.TimelineMarkerRoutedEventArgs()");
		}
		#endif
		// Forced skipping of method Windows.UI.Xaml.Media.TimelineMarkerRoutedEventArgs.TimelineMarkerRoutedEventArgs()
		// Forced skipping of method Windows.UI.Xaml.Media.TimelineMarkerRoutedEventArgs.Marker.get
		// Forced skipping of method Windows.UI.Xaml.Media.TimelineMarkerRoutedEventArgs.Marker.set
	}
}