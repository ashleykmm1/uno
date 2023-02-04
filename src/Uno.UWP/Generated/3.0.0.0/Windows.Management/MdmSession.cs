#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Management
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class MdmSession 
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::System.Collections.Generic.IReadOnlyList<global::Windows.Management.MdmAlert> Alerts
		{
			get
			{
				throw new global::System.NotImplementedException("The member IReadOnlyList<MdmAlert> MdmSession.Alerts is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IReadOnlyList%3CMdmAlert%3E%20MdmSession.Alerts");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::System.Exception ExtendedError
		{
			get
			{
				throw new global::System.NotImplementedException("The member Exception MdmSession.ExtendedError is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=Exception%20MdmSession.ExtendedError");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  string Id
		{
			get
			{
				throw new global::System.NotImplementedException("The member string MdmSession.Id is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=string%20MdmSession.Id");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Management.MdmSessionState State
		{
			get
			{
				throw new global::System.NotImplementedException("The member MdmSessionState MdmSession.State is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=MdmSessionState%20MdmSession.State");
			}
		}
		#endif
		// Forced skipping of method Windows.Management.MdmSession.Alerts.get
		// Forced skipping of method Windows.Management.MdmSession.ExtendedError.get
		// Forced skipping of method Windows.Management.MdmSession.Id.get
		// Forced skipping of method Windows.Management.MdmSession.State.get
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncAction AttachAsync()
		{
			throw new global::System.NotImplementedException("The member IAsyncAction MdmSession.AttachAsync() is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncAction%20MdmSession.AttachAsync%28%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void Delete()
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.Management.MdmSession", "void MdmSession.Delete()");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncAction StartAsync()
		{
			throw new global::System.NotImplementedException("The member IAsyncAction MdmSession.StartAsync() is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncAction%20MdmSession.StartAsync%28%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncAction StartAsync( global::System.Collections.Generic.IEnumerable<global::Windows.Management.MdmAlert> alerts)
		{
			throw new global::System.NotImplementedException("The member IAsyncAction MdmSession.StartAsync(IEnumerable<MdmAlert> alerts) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncAction%20MdmSession.StartAsync%28IEnumerable%3CMdmAlert%3E%20alerts%29");
		}
		#endif
	}
}
