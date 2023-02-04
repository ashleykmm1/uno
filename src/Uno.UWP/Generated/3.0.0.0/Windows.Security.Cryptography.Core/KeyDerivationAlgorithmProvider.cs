#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Security.Cryptography.Core
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class KeyDerivationAlgorithmProvider 
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  string AlgorithmName
		{
			get
			{
				throw new global::System.NotImplementedException("The member string KeyDerivationAlgorithmProvider.AlgorithmName is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=string%20KeyDerivationAlgorithmProvider.AlgorithmName");
			}
		}
		#endif
		// Forced skipping of method Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.AlgorithmName.get
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Security.Cryptography.Core.CryptographicKey CreateKey( global::Windows.Storage.Streams.IBuffer keyMaterial)
		{
			throw new global::System.NotImplementedException("The member CryptographicKey KeyDerivationAlgorithmProvider.CreateKey(IBuffer keyMaterial) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=CryptographicKey%20KeyDerivationAlgorithmProvider.CreateKey%28IBuffer%20keyMaterial%29");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public static global::Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider OpenAlgorithm( string algorithm)
		{
			throw new global::System.NotImplementedException("The member KeyDerivationAlgorithmProvider KeyDerivationAlgorithmProvider.OpenAlgorithm(string algorithm) is not implemented. For more information, visit https://aka.platform.uno/notimplemented?m=KeyDerivationAlgorithmProvider%20KeyDerivationAlgorithmProvider.OpenAlgorithm%28string%20algorithm%29");
		}
		#endif
	}
}
