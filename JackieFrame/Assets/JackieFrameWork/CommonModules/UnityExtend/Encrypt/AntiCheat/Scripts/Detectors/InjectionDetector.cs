﻿#define DEBUG
#undef DEBUG

#define DEBUG_VERBOSE
#undef DEBUG_VERBOSE

#define DEBUG_PARANOID
#undef DEBUG_PARANOID

using System.IO;
using System.Reflection;
using AntiCheat.ObscuredTypes;
using Debug = UnityEngine.Debug;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
using System.Diagnostics;
#endif

namespace AntiCheat.Detectors
{
	/// <summary>
	/// Allows to detect foreign managed assemblies in your app.
	/// Just call InjectionDetector.StartDetection() to use it.
	/// </summary>
	/// You also may add it to the scene in editor through the<br/>
	/// "GameObject->Create Other->Code Stage->Anti-Cheat Toolkit->Injection Detector" menu.
	/// 
	/// It allows you to edit and store detector's settings in inspector.<br/>
	/// <strong>Please, keep in mind you still need to call InjectionDetector.StartDetection() to start detector!
	/// 
	/// Note #1: InjectionDetector works in conjunction with "Enable Injection Detector" option at the<br/>
	/// "Window->Anti-Cheat Toolkit->Options" window.<br/>
	/// Make sure you enabled it there before using detector at runtime.
	/// 
	/// Note #2: InjectionDetector is kinda experimental for now. It should work fine, but it <em>may</em> produce false positives.<br/>
	/// So, please, make sure you tested it on target platform before releasing your app to the public.</strong><br/>
	/// <em>I also would be very happy to know if it do false positives for you!</em>
	/// 
	/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly only PC (including WebPlayer), iOS and Android are supported.</strong>
	[AddComponentMenu("")] // sorry, but you shouldn't add it via Component menu, read above comment please
	public class InjectionDetector: MonoBehaviour
	{
		/// <summary>
		/// Injection Detector will be automatically disposed after firing callback if enabled 
		/// or it will just stop internal processes otherwise.
		/// </summary>
		public bool autoDispose = true;

		/// <summary>
		/// Allows to keep Injection Detector's game object on new level (scene) load.
		/// </summary>
		public bool keepAlive = true; 

		/// <summary>
		/// Callback to call on injection detection.
		/// </summary>
		public Action onInjectionDetected;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IPHONE || UNITY_ANDROID

		private const string COMPONENT_NAME = "Injection Detector";
		private static InjectionDetector instance;

		private bool running;
		private bool signaturesAreNotGenuine;
		private AllowedAssembly[] allowedAssemblies;
		private string[] hexTable;

#if UNITY_EDITOR
		private const string MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/" + COMPONENT_NAME;

		[MenuItem(MENU_PATH, false)]
		private static void AddToScene()
		{
			InjectionDetector component = (InjectionDetector)FindObjectOfType(typeof(InjectionDetector));
			if (component != null)
			{
				if (component.IsPlacedCorrectly())
				{
					if (EditorUtility.DisplayDialog("Remove " + COMPONENT_NAME + "?", COMPONENT_NAME + " already exists in scene and placed correctly. Dou you wish to remove it?", "Yes", "No"))
					{
						DestroyImmediate(component.gameObject);
					}
				}
				else if (component.MayBePlacedHere())
				{
					int dialogResult = EditorUtility.DisplayDialogComplex("Fix existing Game Object to work with " + COMPONENT_NAME + "?", COMPONENT_NAME+ " already exists in scene and placed onto empty Game Object \"" + component.name + "\".\nDo you wish to let component configure and use this Game Object further? Press Delete to remove component from scene at all.", "Fix", "Delete", "Cancel");

					switch (dialogResult)
					{
						case 0:
							component.FixCurrentGameObject();
							break;
						case 1:
							DestroyImmediate(component);
							break;
					}
				}
				else
				{
					int dialogResult = EditorUtility.DisplayDialogComplex("Move existing " + COMPONENT_NAME + " to own Game Object?", "Looks like " + COMPONENT_NAME + " component already exists in scene and placed incorrectly on Game Object \"" + component.name + "\".\nDo you wish to let component move itself onto separate configured Game Object \"" + COMPONENT_NAME + "\"? Press Delete to remove plugin from scene at all.", "Move", "Delete", "Cancel");
					switch (dialogResult)
					{
						case 0:
							GameObject go = new GameObject(COMPONENT_NAME);
							InjectionDetector newComponent = go.AddComponent<InjectionDetector>();

							EditorUtility.CopySerialized(component, newComponent);

							DestroyImmediate(component);
							break;
						case 1:
							DestroyImmediate(component);
							break;
					}
				}
			}
			else
			{
				GameObject go = new GameObject(COMPONENT_NAME);
				go.AddComponent<InjectionDetector>();
			}
		}

		private bool MayBePlacedHere()
		{
			return (gameObject.GetComponentsInChildren<Component>().Length == 2 && transform.childCount == 0 && transform.parent == null);
		}

		private void FixCurrentGameObject()
		{
			gameObject.name = COMPONENT_NAME;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			tag = "Untagged";
			gameObject.layer = 0;
			gameObject.isStatic = true;
		}
#endif
		/// <summary>
		/// Allows to reach public properties to set up them from code.
		/// </summary>
		public static InjectionDetector Instance
		{
			get
			{
				if (instance == null)
				{
					InjectionDetector detector = (InjectionDetector)FindObjectOfType(typeof(InjectionDetector));
					if (detector == null)
					{
						GameObject go = new GameObject(COMPONENT_NAME);
						detector = go.AddComponent<InjectionDetector>();
					}
					return detector;
				}
				return instance;
			}
		}

		// preventing direct instantiation =P
		private InjectionDetector() { }

		/// <summary>
		/// Use it to stop and completely dispose Injection Detector.
		/// </summary>
		public static void Dispose()
		{
			Instance.DisposeInternal();
		}

		private void DisposeInternal()
		{
			StopMonitoringInternal();
			instance = null;
			Destroy(gameObject);
		}

		private void Awake()
		{
			if (instance != null)
			{
				Debuger.LogWarning("[ACT] Only one " + COMPONENT_NAME +" instance allowed!");
				Destroy(this);
				return;
			}

			if (!IsPlacedCorrectly())
			{
				Debuger.LogWarning("[ACT] " + COMPONENT_NAME + " placed in scene incorrectly and will be auto-destroyed! Please, use \"GameObject->Create Other->Code Stage->Anti-Cheat Toolkit->" + COMPONENT_NAME + "\" menu to correct this!");
				Destroy(this);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		private bool IsPlacedCorrectly()
		{
			return (name == COMPONENT_NAME &&
					GetComponentsInChildren<Component>().Length == 2 &&
					transform.childCount == 0);
		}

		private void OnLevelWasLoaded(int index)
		{
			if (!keepAlive)
			{
				Dispose();
			}
		}

		private void OnDisable()
		{
			StopMonitoringInternal();
		}

		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

		/// <summary>
		/// Starts foreign assemblies injection detection.
		/// </summary>
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(Action callback)
		{
			if (Instance.running)
			{
				Debuger.LogWarning("[ACT] " + COMPONENT_NAME + " already running!");
				return;
			}

			Instance.StartDetectionInternal(callback);
		}

		private void StartDetectionInternal(Action callback)
		{
#if UNITY_EDITOR
			if (!EditorPrefs.GetBool("ACTDIDEnabled", false))
			{
				Debuger.LogWarning("[ACT] " + COMPONENT_NAME + " is not enabled in Anti-Cheat Toolkit Options!\nPlease, check readme for details.");
				DisposeInternal();
				return;
			}
#if !DEBUG && !DEBUG_VERBOSE && !DEBUG_PARANOID
			if (Application.isEditor)
			{
				Debuger.LogWarning("[ACT] " + COMPONENT_NAME + " does not work in editor.");
				DisposeInternal();
				return;
			}
#else
			Debuger.LogWarning("[ACT] " + COMPONENT_NAME + " works in debug mode. There WILL BE false positives in editor, it's fine!");
#endif
#endif
			onInjectionDetected = callback;

			if (allowedAssemblies == null)
			{
				LoadAndParseAllowedAssemblies();
			}

			if (signaturesAreNotGenuine)
			{
				InjectionDetected();
				return;
			}

			if (!FindInjectionInCurrentAssemblies())
			{
				// listening for new assemblies
				AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
				running = true;
			}
			else
			{
				InjectionDetected();
			}
		}

		/// <summary>
		/// Stops injection detection. 
		/// </summary>
		public static void StopMonitoring()
		{
			Instance.StopMonitoringInternal();
		}

		private void StopMonitoringInternal()
		{
			if (running)
			{
				onInjectionDetected = null;
				AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
				running = false;
			}
		}

		private void InjectionDetected()
		{
			if (onInjectionDetected != null) onInjectionDetected();
			if (autoDispose)
			{
				Dispose();
			}
			else
			{
				StopMonitoringInternal();
			}
		}

		private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Debuger.Log("[ACT] New assembly loaded: " + args.LoadedAssembly.FullName);
#endif
			if (!AssemblyAllowed(args.LoadedAssembly))
			{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
				Debuger.Log("[ACT] Injected Assembly found:\n" + args.LoadedAssembly.FullName);
#endif
				InjectionDetected();
			}
		}

		private bool FindInjectionInCurrentAssemblies()
		{
			bool result = false;
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Stopwatch stopwatch = Stopwatch.StartNew();
#endif
			foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
			{
#if DEBUG_VERBOSE	
				stopwatch.Stop();
				Debuger.Log("[ACT] Currenly loaded assembly:\n" + ass.FullName);
				stopwatch.Start();
#endif
				if (!AssemblyAllowed(ass))
				{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
					stopwatch.Stop();
					Debuger.Log("[ACT] Injected Assembly found:\n" + ass.FullName + "\n" + GetAssemblyHash(ass));
					stopwatch.Start();
#endif
					result = true;
					break;
				}
			}
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			stopwatch.Stop();
			Debuger.Log("[ACT] Loaded assemblies scan duration: " + stopwatch.ElapsedMilliseconds + " ms.");
#endif
			return result;
		}

		private bool AssemblyAllowed(Assembly ass)
		{

#if !UNITY_WEBPLAYER
			string assemblyName = ass.GetName().Name;
#else
			string fullname = ass.FullName;
			string assemblyName = fullname.Substring(0, fullname.IndexOf(", ", StringComparison.Ordinal));
#endif

			int hash = GetAssemblyHash(ass);
			
			bool result = false;
			for (int i = 0; i < allowedAssemblies.Length; i++)
			{
				AllowedAssembly allowedAssembly = allowedAssemblies[i];

				if (allowedAssembly.name == assemblyName)
				{
					if (Array.IndexOf(allowedAssembly.hashes, hash) != -1)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		private void LoadAndParseAllowedAssemblies()
		{
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			Debuger.Log("[ACT] Starting LoadAndParseAllowedAssemblies()");
			Stopwatch sw = Stopwatch.StartNew();
#endif
			TextAsset assembliesSignatures = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (assembliesSignatures == null)
			{
				signaturesAreNotGenuine = true;
				return;
			}
			
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debuger.Log("[ACT] Creating separator array and opening MemoryStream");
			sw.Start();
#endif

			string[] separator = {":"};

			MemoryStream ms = new MemoryStream(assembliesSignatures.bytes);
			BinaryReader br = new BinaryReader(ms);
			
			int count = br.ReadInt32();
			
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debuger.Log("[ACT] Allowed assemblies count from MS: " + count);
			sw.Start();
#endif

			allowedAssemblies = new AllowedAssembly[count];

			for (int i = 0; i < count; i++)
			{
				string line = br.ReadString();
#if (DEBUG_PARANOID)
				sw.Stop();
				Debuger.Log("[ACT] Line: " + line);
				sw.Start();
#endif
				line = ObscuredString.EncryptDecrypt(line, "Elina");
#if (DEBUG_PARANOID)
				sw.Stop();
				Debuger.Log("[ACT] Line decrypted : " + line);
				sw.Start();
#endif
				string[] strArr = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				int stringsCount = strArr.Length;
#if (DEBUG_PARANOID)
				sw.Stop();
				Debuger.Log("[ACT] stringsCount : " + stringsCount);
				sw.Start();
#endif
				if (stringsCount > 1)
				{
					string assemblyName = strArr[0];

					int[] hashes = new int[stringsCount - 1];
					for (int j = 1; j < stringsCount; j++)
					{
						hashes[j - 1] = int.Parse(strArr[j]);
					}

					allowedAssemblies[i] = (new AllowedAssembly(assemblyName, hashes));
				}
				else
				{
					signaturesAreNotGenuine = true;
					br.Close();
					ms.Close();
#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
					sw.Stop();
#endif
					return;
				}
			}
			br.Close();
			ms.Close();
			Resources.UnloadAsset(assembliesSignatures);

#if DEBUG || DEBUG_VERBOSE || DEBUG_PARANOID
			sw.Stop();
			Debuger.Log("[ACT] Allowed Assemblies parsing duration: " + sw.ElapsedMilliseconds + " ms.");
#endif

			hexTable = new string[256];
			for (int i = 0; i < 256; i++)
			{
				hexTable[i] = i.ToString("x2");
			}
		}

		private int GetAssemblyHash(Assembly ass)
		{
			string hashInfo;

#if !UNITY_WEBPLAYER
			AssemblyName assName = ass.GetName();
			byte[] bytes = assName.GetPublicKeyToken();
			if (bytes.Length == 8)
			{
				hashInfo = assName.Name + PublicKeyTokenToString(bytes);
			}
			else
			{
				hashInfo = assName.Name;
			}
#else
			string fullName = ass.FullName;

			string assemblyName = fullName.Substring(0, fullName.IndexOf(", ", StringComparison.Ordinal));
			int tokenIndex = fullName.IndexOf("PublicKeyToken=", StringComparison.Ordinal) + 15;
			string token = fullName.Substring(tokenIndex, fullName.Length - tokenIndex);
			if (token == "null") token = "";
			hashInfo = assemblyName + token;
#endif

			// Jenkins hash function (http://en.wikipedia.org/wiki/Jenkins_hash_function)
			int result = 0;
			int len = hashInfo.Length;

			for (int i = 0; i < len; ++i)
			{
				result += hashInfo[i];
				result += (result << 10);
				result ^= (result >> 6);
			}
			result += (result << 3);
			result ^= (result >> 11);
			result += (result << 15);

			return result;
		}

#if !UNITY_WEBPLAYER
		private string PublicKeyTokenToString(byte[] bytes)
		{
			string result = "";

			// AssemblyName.GetPublicKeyToken() returns 8 bytes
			for (int i = 0; i < 8; i++)
			{
				result += hexTable[bytes[i]];
			}

			return result;
		}
#endif

		private class AllowedAssembly
		{
			public readonly string name;
			public readonly int[] hashes;

			public AllowedAssembly(string name, int[] hashes)
			{
				this.name = name;
				this.hashes = hashes;
			}
		}
#endif
	}
}
