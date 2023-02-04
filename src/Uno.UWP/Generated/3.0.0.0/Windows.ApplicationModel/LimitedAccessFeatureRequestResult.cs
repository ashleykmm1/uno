#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.ApplicationModel
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class LimitedAccessFeatureRequestResult 
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::System.DateTimeOffset? EstimatedRemovalDate
		{
			get
			{
				throw new global::System.NotImplementedException("The member DateTimeOffset? LimitedAccessFeatureRequestResult.EstimatedRemovalDate is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=DateTimeOffset%3F%20LimitedAccessFeatureRequestResult.EstimatedRemovalDate");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  string FeatureId
		{
			get
			{
				throw new global::System.NotImplementedException("The member string LimitedAccessFeatureRequestResult.FeatureId is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=string%20LimitedAccessFeatureRequestResult.FeatureId");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.ApplicationModel.LimitedAccessFeatureStatus Status
		{
			get
			{
				throw new global::System.NotImplementedException("The member LimitedAccessFeatureStatus LimitedAccessFeatureRequestResult.Status is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=LimitedAccessFeatureStatus%20LimitedAccessFeatureRequestResult.Status");
			}
		}
		#endif
		// Forced skipping of method Windows.ApplicationModel.LimitedAccessFeatureRequestResult.FeatureId.get
		// Forced skipping of method Windows.ApplicationModel.LimitedAccessFeatureRequestResult.Status.get
		// Forced skipping of method Windows.ApplicationModel.LimitedAccessFeatureRequestResult.EstimatedRemovalDate.get
	}
}
