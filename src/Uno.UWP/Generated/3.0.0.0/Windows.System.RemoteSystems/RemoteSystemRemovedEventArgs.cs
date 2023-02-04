#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.System.RemoteSystems
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class RemoteSystemRemovedEventArgs 
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  string RemoteSystemId
		{
			get
			{
				throw new global::System.NotImplementedException("The member string RemoteSystemRemovedEventArgs.RemoteSystemId is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=string%20RemoteSystemRemovedEventArgs.RemoteSystemId");
			}
		}
		#endif
		// Forced skipping of method Windows.System.RemoteSystems.RemoteSystemRemovedEventArgs.RemoteSystemId.get
	}
}
