#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.ApplicationModel.Contacts
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class PinnedContactManager 
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.System.User User
		{
			get
			{
				throw new global::System.NotImplementedException("The member User PinnedContactManager.User is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=User%20PinnedContactManager.User");
			}
		}
		#endif
		// Forced skipping of method Windows.ApplicationModel.Contacts.PinnedContactManager.User.get
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  bool IsPinSurfaceSupported( global::Windows.ApplicationModel.Contacts.PinnedContactSurface surface)
		{
			throw new global::System.NotImplementedException("The member bool PinnedContactManager.IsPinSurfaceSupported(PinnedContactSurface surface) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=bool%20PinnedContactManager.IsPinSurfaceSupported%28PinnedContactSurface%20surface%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  bool IsContactPinned( global::Windows.ApplicationModel.Contacts.Contact contact,  global::Windows.ApplicationModel.Contacts.PinnedContactSurface surface)
		{
			throw new global::System.NotImplementedException("The member bool PinnedContactManager.IsContactPinned(Contact contact, PinnedContactSurface surface) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=bool%20PinnedContactManager.IsContactPinned%28Contact%20contact%2C%20PinnedContactSurface%20surface%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncOperation<bool> RequestPinContactAsync( global::Windows.ApplicationModel.Contacts.Contact contact,  global::Windows.ApplicationModel.Contacts.PinnedContactSurface surface)
		{
			throw new global::System.NotImplementedException("The member IAsyncOperation<bool> PinnedContactManager.RequestPinContactAsync(Contact contact, PinnedContactSurface surface) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncOperation%3Cbool%3E%20PinnedContactManager.RequestPinContactAsync%28Contact%20contact%2C%20PinnedContactSurface%20surface%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncOperation<bool> RequestPinContactsAsync( global::System.Collections.Generic.IEnumerable<global::Windows.ApplicationModel.Contacts.Contact> contacts,  global::Windows.ApplicationModel.Contacts.PinnedContactSurface surface)
		{
			throw new global::System.NotImplementedException("The member IAsyncOperation<bool> PinnedContactManager.RequestPinContactsAsync(IEnumerable<Contact> contacts, PinnedContactSurface surface) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncOperation%3Cbool%3E%20PinnedContactManager.RequestPinContactsAsync%28IEnumerable%3CContact%3E%20contacts%2C%20PinnedContactSurface%20surface%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncOperation<bool> RequestUnpinContactAsync( global::Windows.ApplicationModel.Contacts.Contact contact,  global::Windows.ApplicationModel.Contacts.PinnedContactSurface surface)
		{
			throw new global::System.NotImplementedException("The member IAsyncOperation<bool> PinnedContactManager.RequestUnpinContactAsync(Contact contact, PinnedContactSurface surface) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncOperation%3Cbool%3E%20PinnedContactManager.RequestUnpinContactAsync%28Contact%20contact%2C%20PinnedContactSurface%20surface%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void SignalContactActivity( global::Windows.ApplicationModel.Contacts.Contact contact)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.ApplicationModel.Contacts.PinnedContactManager", "void PinnedContactManager.SignalContactActivity(Contact contact)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Foundation.IAsyncOperation<global::Windows.ApplicationModel.Contacts.PinnedContactIdsQueryResult> GetPinnedContactIdsAsync()
		{
			throw new global::System.NotImplementedException("The member IAsyncOperation<PinnedContactIdsQueryResult> PinnedContactManager.GetPinnedContactIdsAsync() is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=IAsyncOperation%3CPinnedContactIdsQueryResult%3E%20PinnedContactManager.GetPinnedContactIdsAsync%28%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public static global::Windows.ApplicationModel.Contacts.PinnedContactManager GetDefault()
		{
			throw new global::System.NotImplementedException("The member PinnedContactManager PinnedContactManager.GetDefault() is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=PinnedContactManager%20PinnedContactManager.GetDefault%28%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public static global::Windows.ApplicationModel.Contacts.PinnedContactManager GetForUser( global::Windows.System.User user)
		{
			throw new global::System.NotImplementedException("The member PinnedContactManager PinnedContactManager.GetForUser(User user) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=PinnedContactManager%20PinnedContactManager.GetForUser%28User%20user%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public static bool IsSupported()
		{
			throw new global::System.NotImplementedException("The member bool PinnedContactManager.IsSupported() is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=bool%20PinnedContactManager.IsSupported%28%29");
		}
		#endif
	}
}
