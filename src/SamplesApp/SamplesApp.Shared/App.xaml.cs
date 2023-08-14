using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using System.Globalization;
using Windows.UI.ViewManagement;
using Microsoft.Extensions.Logging;
using Uno;
using System.Diagnostics.CodeAnalysis;

#if !HAS_UNO
using Uno.Logging;
#endif

#if HAS_UNO_WINUI
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
#else
using LaunchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;
#endif

#if UNO_ISLANDS
using Windows.UI.Xaml.Markup;
using Uno.UI.XamlHost;
#endif

namespace SamplesApp
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed public partial class App : Application
#if UNO_ISLANDS
	, IXamlMetadataProvider, IXamlMetadataContainer, IDisposable
#endif
	{
#if HAS_UNO
		private static Uno.Foundation.Logging.Logger _log;
#else
		private static ILogger _log;
#endif

		private bool _wasActivated;
		private bool _isSuspended;

		static App()
		{
			ConfigureLogging();
		}

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			// Fix language for UI tests
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

#if __SKIA__
			ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(1024, 768);
#endif

			ConfigureFeatureFlags();

			AssertIssue1790ApplicationSettingsUsable();
			AssertApplicationData();

			this.InitializeComponent();
			this.Suspending += OnSuspending;
			this.Resuming += OnResuming;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected
#if HAS_UNO
			internal
#endif
			override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if __IOS__ && !__MACCATALYST__
			// requires Xamarin Test Cloud Agent
			Xamarin.Calabash.Start();

			LaunchiOSWatchDog();
#endif
			var activationKind =
#if HAS_UNO_WINUI
				e.UWPLaunchActivatedEventArgs.Kind
#else
				e.Kind
#endif
				;

			if (activationKind == ActivationKind.Launch)
			{
				AssertIssue8356();

				AssertIssue12936();

				AssertIssue12937();
			}

			var sw = Stopwatch.StartNew();
			var n = Windows.UI.Xaml.Window.Current.Dispatcher.RunIdleAsync(
				_ =>
				{
					Console.WriteLine("Done loading " + sw.Elapsed);
				});

#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif
			AssertInitialWindowSize();

			InitializeFrame(e.Arguments);

			AssertIssue8641NativeOverlayInitialized();

			ActivateMainWindow();

			ApplicationView.GetForCurrentView().Title = "Uno Samples";

#if __SKIA__ && DEBUG
			AppendRepositoryPathToTitleBar();
#endif

			HandleLaunchArguments(e);
		}

#if __SKIA__ && DEBUG
		private void AppendRepositoryPathToTitleBar()
		{
			var fullPath = Package.Current.InstalledLocation.Path;
			var srcSamplesApp = $"{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}SamplesApp";
			var repositoryPath = fullPath;
			if (fullPath.IndexOf(srcSamplesApp) is int index && index > 0)
			{
				repositoryPath = fullPath.Substring(0, index);
			}

			ApplicationView.GetForCurrentView().Title += $" ({repositoryPath})";
		}
#endif

#if __IOS__
		/// <summary>
		/// Launches a watchdog that will terminate the app if the dispatcher does not process
		/// messages within a specific time.
		///
		/// Restarting the app is required in some cases where either the test engine, or Xamarin.UITest stall
		/// while processing the events of the app.
		///
		/// See https://github.com/unoplatform/uno/issues/3363 for details
		/// </summary>
		private void LaunchiOSWatchDog()
		{
			if (!Debugger.IsAttached)
			{
				Console.WriteLine("Starting dispatcher WatchDog...");

				var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
				var timeout = TimeSpan.FromSeconds(240);

				Task.Run(async () =>
				{

					while (true)
					{
						var delayTask = Task.Delay(timeout);
						var messageTask = dispatcher.RunAsync(CoreDispatcherPriority.High, () => { }).AsTask();

						if (await Task.WhenAny(delayTask, messageTask) == delayTask)
						{
							ThreadPool.QueueUserWorkItem(
								_ =>
								{
									Console.WriteLine($"WatchDog detecting a stall in the dispatcher after {timeout}, terminating the app");
									throw new Exception($"Watchdog failed");
								});
						}

						await Task.Delay(TimeSpan.FromSeconds(5));
					}
				});
			}
		}
#endif

		protected
#if HAS_UNO
			internal
#endif
			override async void OnActivated(IActivatedEventArgs e)
		{
			base.OnActivated(e);

			InitializeFrame();
			ActivateMainWindow();

			if (e.Kind == ActivationKind.Protocol)
			{
				var protocolActivatedEventArgs = (ProtocolActivatedEventArgs)e;
				var dlg = new MessageDialog(
					$"PreviousState - {e.PreviousExecutionState}, " +
					$"Uri - {protocolActivatedEventArgs.Uri}",
					"Application activated via protocol");
				if (ApiInformation.IsMethodPresent("Windows.UI.Popups.MessageDialog", nameof(MessageDialog.ShowAsync)))
				{
					await dlg.ShowAsync();
				}
			}
		}

		private void ActivateMainWindow()
		{
			Windows.UI.Xaml.Window.Current.Activate();
			_wasActivated = true;
			_isSuspended = false;
		}

#if !HAS_UNO_WINUI
		protected override void OnWindowCreated(global::Windows.UI.Xaml.WindowCreatedEventArgs args)
		{
			if (Current is null)
			{
				throw new InvalidOperationException("The Window should be created later in the application lifecycle.");
			}
		}
#endif

		private void InitializeFrame(string arguments = null)
		{
			Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				// Place the frame in the current Window
				Windows.UI.Xaml.Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				// When the navigation stack isn't restored navigate to the first page,
				// configuring the new page by passing required information as a navigation
				// parameter
				var startingPageType = typeof(MainPage);
				if (arguments != null)
				{
					rootFrame.Navigate(startingPageType, arguments);
				}
				else
				{
					rootFrame.Navigate(startingPageType);
				}
			}
		}

		private async void HandleLaunchArguments(LaunchActivatedEventArgs launchActivatedEventArgs)
		{
			Console.WriteLine($"HandleLaunchArguments: {launchActivatedEventArgs.Arguments}");

			if (launchActivatedEventArgs.Arguments is not { } args)
			{
				return;
			}

			if (HandleAutoScreenshots(args))
			{
				return;
			}

			if (await HandleRuntimeTests(args))
			{
				return;
			}

			if (TryNavigateToLaunchSample(args))
			{
				return;
			}

			if (!string.IsNullOrEmpty(args))
			{
				var dlg = new MessageDialog(args, "Launch arguments");
				await dlg.ShowAsync();
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception($"Failed to load Page {e.SourcePageType}: {e.Exception}");
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			_isSuspended = true;

			var deferral = e.SuspendingOperation.GetDeferral();

			Console.WriteLine($"OnSuspending (Deadline:{e.SuspendingOperation.Deadline})");

			deferral.Complete();
		}

		private void OnResuming(object sender, object e)
		{
			Console.WriteLine("OnResuming");

			AssertIssue10313ResumingAfterActivate();

			_isSuspended = false;
		}

		public static void ConfigureLogging()
		{
#if HAS_UNO
			System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, e) => _log?.Error("UnobservedTaskException", e.Exception);
			AppDomain.CurrentDomain.UnhandledException += (s, e) => _log?.Error("UnhandledException", (e.ExceptionObject as Exception) ?? new Exception("Unknown exception " + e.ExceptionObject));
#endif
			var factory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
			{
#if __WASM__
				builder.AddProvider(new Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#else
				builder.AddConsole();
#endif

#if !DEBUG
				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);
#else
				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Debug);
#endif

				// Runtime Tests control logging
				builder.AddFilter("Uno.UI.Samples.Tests", LogLevel.Information);

				builder.AddFilter("Uno.UI.Media", LogLevel.Information);

				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

				// RemoteControl and HotReload related
				builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

				// Display Skia related information
				builder.AddFilter("Uno.UI.Runtime.Skia", LogLevel.Debug);
				builder.AddFilter("Uno.UI.Skia", LogLevel.Debug);

				// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.PopupPanel", LogLevel.Debug );

				// Generic Xaml events
				// builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Media", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Shapes", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );
				// builder.AddFilter("Windows.UI.Xaml.Controls.TextBlock", LogLevel.Debug );

				// Layouter specific messages
				// builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );
				// builder.AddFilter("Windows.Storage", LogLevel.Debug );

				// Binding related messages
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

				// Binder memory references tracking
				// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

				// builder.AddFilter(ListView-related messages
				// builder.AddFilter("Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.ListView", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.GridView", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug ); //iOS
				// builder.AddFilter("Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug ); //iOS
				// builder.AddFilter("Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug ); //Android
				// builder.AddFilter("Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug ); //Android
				// builder.AddFilter("Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug ); //WASM
			});

			Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
#if HAS_UNO
			global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
			_log = Uno.Foundation.Logging.LogExtensionPoint.Factory.CreateLogger(typeof(App));
#else
			_log = Uno.Extensions.LogExtensionPoint.Log(typeof(App));
#endif
		}

		static void ConfigureFeatureFlags()
		{
#if __IOS__
			Uno.UI.FeatureConfiguration.CommandBar.AllowNativePresenterContent = true;
			WinRTFeatureConfiguration.Focus.EnableExperimentalKeyboardFocus = true;
			Uno.UI.FeatureConfiguration.DatePicker.UseLegacyStyle = true;
			Uno.UI.FeatureConfiguration.TimePicker.UseLegacyStyle = true;
#endif
#if __SKIA__
			Uno.UI.FeatureConfiguration.ToolTip.UseToolTips = true;
#endif
		}

		public static string GetDisplayScreenScaling(string displayId)
			=> (DisplayInformation.GetForCurrentView().LogicalDpi * 100f / 96f).ToString(CultureInfo.InvariantCulture);

		public static string RunTest(string metadataName)
		{
			try
			{
				Console.WriteLine($"Initiate Running Test {metadataName}");

				var testId = Interlocked.Increment(ref _testIdCounter);

				_ = Windows.UI.Xaml.Window.Current.Dispatcher.RunAsync(
					CoreDispatcherPriority.Normal,
					async () =>
					{
						try
						{
#if __IOS__ || __ANDROID__
							var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
							if (statusBar != null)
							{
								_ = Windows.UI.Xaml.Window.Current.Dispatcher.RunAsync(
									Windows.UI.Core.CoreDispatcherPriority.Normal,
									async () => await statusBar.HideAsync()
								);
							}
#endif

#if __ANDROID__
							Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
							Uno.UI.FeatureConfiguration.ScrollViewer.AndroidScrollbarFadeDelay = TimeSpan.Zero;
#endif

#if HAS_UNO
							// Disable the TextBox caret for new instances
							Uno.UI.FeatureConfiguration.TextBox.HideCaret = true;
#endif

							var t = SampleControl.Presentation.SampleChooserViewModel.Instance.SetSelectedSample(CancellationToken.None, metadataName);
							var timeout = Task.Delay(30000);

							await Task.WhenAny(t, timeout);

							if (!(t.IsCompleted && !t.IsFaulted))
							{
								throw new TimeoutException();
							}

							ImmutableInterlocked.Update(ref _doneTests, lst => lst.Add(testId));
						}
						catch (Exception e)
						{
							Console.WriteLine($"Failed to run test {metadataName}, {e}");
						}
						finally
						{
#if HAS_UNO
							// Restore the caret for new instances
							Uno.UI.FeatureConfiguration.TextBox.HideCaret = false;
#endif
						}
					}
				);

				return testId.ToString();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed Running Test {metadataName}, {e}");
				return "";
			}
		}

#if __IOS__
		[Foundation.Export("runTest:")] // notice the colon at the end of the method name
		public Foundation.NSString RunTestBackdoor(Foundation.NSString value) => new Foundation.NSString(RunTest(value));

		[Foundation.Export("isTestDone:")] // notice the colon at the end of the method name
		public Foundation.NSString IsTestDoneBackdoor(Foundation.NSString value) => new Foundation.NSString(IsTestDone(value).ToString());

		[Foundation.Export("getDisplayScreenScaling:")] // notice the colon at the end of the method name
		public Foundation.NSString GetDisplayScreenScalingBackdoor(Foundation.NSString value) => new Foundation.NSString(GetDisplayScreenScaling(value).ToString());
#endif

		public static bool IsTestDone(string testId) => int.TryParse(testId, out var id) ? _doneTests.Contains(id) : false;

		/// <summary>
		/// Assert that ApplicationData.Current.[LocalFolder|RoamingFolder] is usable in the constructor of App.xaml.cs on all platforms.
		/// </summary>
		/// <seealso href="https://github.com/unoplatform/uno/issues/1741"/>
		public void AssertIssue1790ApplicationSettingsUsable()
		{
			void AssertIsUsable(Windows.Storage.ApplicationDataContainer container)
			{
				const string issue1790 = nameof(issue1790);

				container.Values.Remove(issue1790);
				container.Values.Add(issue1790, "ApplicationData.Current.[LocalFolder|RoamingFolder] is usable in the constructor of App.xaml.cs on this platform.");

				Assert.IsTrue(container.Values.ContainsKey(issue1790));
			}

			AssertIsUsable(Windows.Storage.ApplicationData.Current.LocalSettings);
			AssertIsUsable(Windows.Storage.ApplicationData.Current.RoamingSettings);
		}

		/// <summary>
		/// Assert that the App Title was found in manifest and loaded from resources
		/// </summary>
		public void AssertIssue12936()
		{
			//ApplicationView Title is currently not supported on iOS
#if !__IOS__
			var title = ApplicationView.GetForCurrentView().Title;

			Assert.IsFalse(string.IsNullOrEmpty(title), "App Title is empty.");

			Assert.IsFalse(title.Contains("ms-resource:"), $"'{title}' wasn't found in resources.");
#endif
		}

		/// <summary>
		/// Assert that ApplicationModel Package properties were found in the manifest and loaded from resources 
		/// </summary>
		public void AssertIssue12937()
		{
			//The ApplicationModel Package properties are currently are currently supported on Skia
#if __SKIA__
			var description = Package.Current.Description;
			var publisherName = Package.Current.PublisherDisplayName;

			Assert.IsFalse(string.IsNullOrEmpty(description), "Description isn't in manifest.");

			Assert.IsFalse(string.IsNullOrEmpty(publisherName), "PublisherDisplayName isn't in manifest.");

			Assert.IsFalse(description.Contains("ms-resource:"), $"'{description}' wasn't found in resources.");

			Assert.IsFalse(publisherName.Contains("ms-resource:"), $"'{publisherName}' wasn't found in resources.");
#endif
		}

		/// <summary>
		/// Assert that Application Title is getting its value from manifest
		/// </summary>
		public void AssertIssue8356()
		{
#if __SKIA__
			Uno.UI.RuntimeTests.Tests.Windows_UI_ViewManagement_ApplicationView.Given_ApplicationView.StartupTitle = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title;
#endif
		}

		/// <summary>
		/// Assert that the native overlay layer for Skia targets is initialized in time for UI to appear.
		/// </summary>
		public void AssertIssue8641NativeOverlayInitialized()
		{
#if __SKIA__
			if (Uno.UI.Xaml.Core.CoreServices.Instance.InitializationType == Uno.UI.Xaml.Core.InitializationType.IslandsOnly)
			{
				return;
			}
			// Temporarily add a TextBox to the current page's content to verify native overlay is available
			Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;
			var textBox = new TextBox();
			textBox.XamlRoot = rootFrame.XamlRoot;
			var textBoxView = new TextBoxView(textBox);
			ApiExtensibility.CreateInstance<IOverlayTextBoxViewExtension>(textBoxView, out var textBoxViewExtension);
			Assert.IsTrue(textBoxViewExtension.IsOverlayLayerInitialized(rootFrame.XamlRoot));
#endif
		}

		public void AssertInitialWindowSize()
		{
#if !__SKIA__ // Will be fixed as part of #8341
			Assert.IsTrue(global::Windows.UI.Xaml.Window.Current.Bounds.Width > 0);
			Assert.IsTrue(global::Windows.UI.Xaml.Window.Current.Bounds.Height > 0);
#endif
		}

		/// <summary>
		/// Verifies that ApplicationData are available immediately after the application class is created
		/// and the data are stored in proper application specific lcoations.
		/// </summary>
		public void AssertApplicationData()
		{
#if __SKIA__
			var appName = Package.Current.Id.Name;
			var publisher = string.IsNullOrEmpty(Package.Current.Id.Publisher) ? "" : "Uno Platform";

			AssertForFolder(ApplicationData.Current.LocalFolder);
			AssertForFolder(ApplicationData.Current.RoamingFolder);
			AssertForFolder(ApplicationData.Current.TemporaryFolder);
			AssertForFolder(ApplicationData.Current.LocalCacheFolder);
			AssertSettings(ApplicationData.Current.LocalSettings);
			AssertSettings(ApplicationData.Current.RoamingSettings);

			void AssertForFolder(StorageFolder folder)
			{
				AssertContainsIdProps(folder);
				AssertCanCreateFile(folder);
			}

			void AssertSettings(ApplicationDataContainer container)
			{
				var key = Guid.NewGuid().ToString();
				var value = Guid.NewGuid().ToString();

				container.Values[key] = value;
				Assert.IsTrue(container.Values.ContainsKey(key));
				Assert.AreEqual(value, container.Values[key]);
				container.Values.Remove(key);
			}

			void AssertContainsIdProps(StorageFolder folder)
			{
				Assert.IsTrue(folder.Path.Contains(appName, StringComparison.Ordinal));
				Assert.IsTrue(folder.Path.Contains(publisher, StringComparison.Ordinal));
			}

			void AssertCanCreateFile(StorageFolder folder)
			{
				var filename = Guid.NewGuid() + ".txt";
				var path = Path.Combine(folder.Path, filename);
				var expectedContent = "Test";
				try
				{
					File.WriteAllText(path, expectedContent);
					var actualContent = File.ReadAllText(path);

					Assert.AreEqual(expectedContent, actualContent);
				}
				finally
				{
					File.Delete(path);
				}
			}
#endif
		}
	}
}
