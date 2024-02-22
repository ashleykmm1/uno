#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Composition
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class ColorKeyFrameAnimation : global::Windows.UI.Composition.KeyFrameAnimation
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.UI.Composition.CompositionColorSpace InterpolationColorSpace
		{
			get
			{
				throw new global::System.NotImplementedException("The member CompositionColorSpace ColorKeyFrameAnimation.InterpolationColorSpace is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=CompositionColorSpace%20ColorKeyFrameAnimation.InterpolationColorSpace");
			}
			set
			{
				global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.ColorKeyFrameAnimation", "CompositionColorSpace ColorKeyFrameAnimation.InterpolationColorSpace");
			}
		}
		#endif
		// Forced skipping of method Windows.UI.Composition.ColorKeyFrameAnimation.InterpolationColorSpace.get
		// Forced skipping of method Windows.UI.Composition.ColorKeyFrameAnimation.InterpolationColorSpace.set
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void InsertKeyFrame( float normalizedProgressKey,  global::Windows.UI.Color value)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.ColorKeyFrameAnimation", "void ColorKeyFrameAnimation.InsertKeyFrame(float normalizedProgressKey, Color value)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void InsertKeyFrame( float normalizedProgressKey,  global::Windows.UI.Color value,  global::Windows.UI.Composition.CompositionEasingFunction easingFunction)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Composition.ColorKeyFrameAnimation", "void ColorKeyFrameAnimation.InsertKeyFrame(float normalizedProgressKey, Color value, CompositionEasingFunction easingFunction)");
		}
		#endif
	}
}