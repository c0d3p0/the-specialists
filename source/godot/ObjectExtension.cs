using System.Text;
using SC = System.Collections;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public static class ObjectExtension
{
	public static T TryToCall<T>(this Godot.Object gdobj, Godot.Object caller,
			string methodName, T defaultValue, params object[] args)
	{
		T response = defaultValue;
		int length = args != null ? args.Length : 0;
		object[] newArgs = new object[length + 1];
		System.Array.Copy(args, 0, newArgs, 0, length);
		Optional optional = new Optional();
		newArgs[length] = optional;
		caller.Call(methodName, newArgs);

		if(!optional.IsDataReceived())
		{
			if(OS.IsDebugBuild())
			{
				StringBuilder msg = new StringBuilder(methodName);
				msg.Append("() called by ");
				msg.Append(caller.GetType().ToString()).Append(" from ");
				msg.Append(gdobj.GetType().ToString());
				msg.Append(" could not return a value!");
				GD.PushWarning(msg.ToString());
			}
		}
		else
			response = optional.Get<T>();

		// This fix abusive usage of Game.Objects causing a lot of memory leaks.
		optional.Free();
		return response;
	}

	public static T Call<T>(this Godot.Object gdobj, Godot.Object caller,
			string methodName, params object[] args)
	{
		T response = default(T);
		int length = args != null ? args.Length : 0;
		object[] newArgs = new object[length + 1];
		System.Array.Copy(args, 0, newArgs, 0, length);
		Optional optional = new Optional();
		newArgs[length] = optional;
		caller.Call(methodName, newArgs);

		if(!optional.IsDataReceived())
		{
			if(OS.IsDebugBuild())
			{
				StringBuilder msg = new StringBuilder(methodName);
				msg.Append("() called by ");
				msg.Append(caller.GetType().ToString()).Append(" from ");
				msg.Append(gdobj.GetType().ToString());
				msg.Append(" could not return a value!");
				GD.PushWarning(msg.ToString());
			}
		}
		else
			response = optional.Get<T>();

		// This fix abusive usage of Game.Objects causing a lot of memory leaks.
		optional.Free();
		return response;
	}

	public static T EmitSignal<T>(this Godot.Object gdobj, Node emitter,
			string signal, params object[] args)
	{
		T response = default(T);
		int length = args != null ? args.Length : 0;
		object[] newArgs = new object[length + 1];
		System.Array.Copy(args, 0, newArgs, 0, length);
		Optional optional = new Optional();
		newArgs[length] = optional;
		emitter.EmitSignal(signal, newArgs);

		if(!optional.IsDataReceived())
		{
			if(OS.IsDebugBuild())
			{
				StringBuilder msg = new StringBuilder(signal);
				msg.Append(" emitted by ");
				msg.Append(emitter.Name).Append(" could not return a value!");
				GD.PushWarning(msg.ToString());
			}
		}
		else
			response = optional.Get<T>();

		// This fix abusive usage of Game.Objects causing a lot of memory leaks.
		optional.Free();
		return response;
	}

	public static T EmitSafeSignal<T>(this Godot.Object gdobj, Node emitter,
			string signal, T defaultResponse, params object[] args)
	{
		if(emitter == null)
		{
			GD.PushWarning("Emmiter is null!");
			return defaultResponse;
		}
		else
			return EmitSignal<T>(gdobj, emitter, signal, args);
	}

	public static Godot.Collections.Array CreateSingleBind(
			this Godot.Object gdobj, object bind)
	{
		Godot.Collections.Array binds = new Godot.Collections.Array();
		binds.Add(bind);
		return binds;
	}

	public static T GetRandomItem<T>(this Godot.Object gdobj,
			Array<T> list, RandomNumberGenerator rng)
	{
		if(list != null && list.Count > 0)
		{
			rng.Randomize();
			int randomIndex = rng.RandiRange(0, list.Count - 1);
			return list[randomIndex];
		}
		
		return default(T);
	}

	public static T GetRandomItem<T>(this Godot.Object gdobj,
			T[] array, RandomNumberGenerator rng)
	{
		if(array != null && array.Length > 0)
		{
			rng.Randomize();
			int randomIndex = rng.RandiRange(0, array.Length - 1);
			return array[randomIndex];
		}

		return default(T);
	}

	public static int RandiRange(this Godot.Object gdobj,
			RandomNumberGenerator rng, int from, int to)
	{
		rng.Randomize();
		return rng.RandiRange(from, to);
	}

	public static float RandfRange(this Godot.Object gdobj,
			RandomNumberGenerator rng, float from, float to)
	{
		rng.Randomize();
		return rng.RandfRange(from, to);
	}

	public static Array<T> ConvertToGenericArray<T>(
			this Godot.Object gdobj, Node caller, Array array)
	{
		Array<T> genericArray = new Array<T>();

		if(array != null)
		{
			SC.IEnumerator it = array.GetEnumerator();

			while(it.MoveNext())
				genericArray.Add((T) it.Current);
		}
		
		return genericArray;
	}

	public static Array ConvertToDefaultArray<T>(
			this Godot.Object gdobj, Array<T> array)
	{
		Array defaultArray = new Array();

		if(defaultArray != null)
		{
			SCG.IEnumerator<T> it = array.GetEnumerator();

			while(it.MoveNext())
				defaultArray.Add(it.Current);
		}
		
		return defaultArray;
	}

	public static Dictionary<TK, TV> ConvertToGenericMap<TK, TV>(
			this Godot.Object gdobj, Dictionary map)
	{
		Dictionary<TK, TV> genericMap = new Dictionary<TK, TV>();

		if(map != null)
		{
			SC.IDictionaryEnumerator it = map.GetEnumerator();

			while(it.MoveNext())
				genericMap.Add((TK) it.Entry.Key, (TV) it.Entry.Value);
		}
		
		return genericMap;
	}

	public static Dictionary ConvertToDefaultMap<TK, TV>(
			this Godot.Object gdobj, Dictionary<TK, TV> map)
	{
		Dictionary defaultMap = new Dictionary();

		if(map != null)
		{
			SCG.IEnumerator<SCG.KeyValuePair<TK, TV>> it = map.GetEnumerator();

			while(it.MoveNext())
				defaultMap.Add(it.Current.Key, it.Current.Value);
		}
		
		return defaultMap;
	}

	public static TK[] ConvertKeysToArray<TK, TV>(
			this Godot.Object gdobj, Dictionary<TK, TV> map)
	{
		if(map != null)
		{
			int index = 0;
			TK[] array = new TK[map.Count];
			SCG.IEnumerator<SCG.KeyValuePair<TK, TV>> it = map.GetEnumerator();

			while(it.MoveNext())
				array[index++] = it.Current.Key;

			return array;
		}
		
		return new TK[0];
	}

	public static void CreateFolder(this Godot.Object gdobj, string folderPath)
	{
		char separator = System.IO.Path.DirectorySeparatorChar;
		Directory d = new Directory();
		string finalFolderPath = new StringBuilder(GetExecutableFolder()).
				Append(separator).Append(folderPath).ToString();
		if(!d.DirExists(finalFolderPath))
			d.MakeDir(finalFolderPath);
		
		d.Dispose();
	}

	public static string GetFilePath(this Godot.Object gdobj, string fileName,
			string fileExtension, string folderName)
	{
		char separator = System.IO.Path.DirectorySeparatorChar;
		return new StringBuilder(GetExecutableFolder()).Append(separator).
				Append(folderName).Append(separator).Append(fileName).Append('.').
				Append(fileExtension).ToString();
	}

	public static string GetExecutableFolder()
	{
		char separator = System.IO.Path.DirectorySeparatorChar;
		return OS.GetExecutablePath().Substring(0,
				OS.GetExecutablePath().LastIndexOf(separator));
	}

	public static string CreateString(this Godot.Object gdobj, params object[] stringParams)
	{
		if(stringParams != null && stringParams.Length > 0)
		{
			StringBuilder sb = new StringBuilder();

			for(int i = 0; i < stringParams.Length; i++)
				sb.Append(stringParams[i]);

			return sb.ToString();
		}

		return null;
	}

	public static void PrintDebug(this Godot.Object gdobj, string data)
	{
		if(OS.IsDebugBuild())
			GD.Print(data);
	}

	public static void PrintErrorDebug(this Godot.Object gdobj, string data)
	{
		if(OS.IsDebugBuild())
			GD.PrintErr(data);
	}

	public static void PushWarningDebug(this Godot.Object gdobj, string data)
	{
		if(OS.IsDebugBuild())
			GD.PushWarning(data);
	}

	public static void PushErrorDebug(this Godot.Object gdobj, string data)
	{
		if(OS.IsDebugBuild())
			GD.PushError(data);
	}
}
