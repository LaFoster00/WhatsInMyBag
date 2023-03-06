using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameEvents;
using UnityEngine;

namespace Content.Onboarding.Scripts
{
	public class PlayedIntroEvent : GameEvent
	{
		public string name;
	}

	/// <summary>
	/// actual entry we want to save
	/// </summary>
	[Serializable]
	public class SaveEntry
	{
		public string name;
		public bool value;
	}

	/// <summary>
	/// Exists just to serialize a list...
	/// </summary>
	[Serializable]
	public class SaveData
	{
		public List<SaveEntry> data;
	}

	public class SaveDoneIntros : MonoBehaviour
	{
		public bool doLoad = true;
		public bool doSave = true;

		public bool canPlayIntros = true;

		public string savePath = "playedIntros.data";

		private Dictionary<string, bool> _playedIntros;

		public static SaveDoneIntros Instance { get; private set; }

		public string SaveFilePath => $"{Application.persistentDataPath}/{savePath}";

		private void Start()
		{
			Instance = this;
			_playedIntros = new Dictionary<string, bool>();


			if (doLoad)
				DoLoad();

			GameEventManager.AddListener<PlayedIntroEvent>(OnPlayedIntro);
		}

		private void OnDisable()
		{
			GameEventManager.RemoveListener<PlayedIntroEvent>(OnPlayedIntro);

			if (doSave)
				DoSave();
		}

		public bool CanPlayIntro(string name)
		{
			return canPlayIntros && (!_playedIntros.ContainsKey(name) || !_playedIntros[name]);
		}

		private void DoSave()
		{
			List<SaveEntry> data = new List<SaveEntry>();
			foreach (var entry in _playedIntros)
			{
				data.Add(new SaveEntry {name = entry.Key, value = entry.Value});
			}

			WipeSave();
			string saveData = JsonUtility.ToJson(new SaveData() {data = data}, true);
			FileStream file = File.Create(SaveFilePath);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(file, saveData);
			file.Close();
		}

		private void DoLoad()
		{
			if (File.Exists(SaveFilePath))
			{
				SaveData data = new SaveData();

				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(SaveFilePath, FileMode.Open);
				string json = (string) bf.Deserialize(file);
				JsonUtility.FromJsonOverwrite(json, data);
				file.Close();

				foreach (var saveData in data.data)
				{
					_playedIntros[saveData.name] = saveData.value;
				}
			}
		}

		private void OnPlayedIntro(PlayedIntroEvent e)
		{
			if (!_playedIntros.ContainsKey(e.name) || _playedIntros[e.name] == false)
			{
				_playedIntros[e.name] = true;
				
				if (doSave)
					DoSave();
			}
		}

		public void WipeSave()
		{
			if (File.Exists(SaveFilePath))
			{
				File.Delete(SaveFilePath);
			}
		}

		public void DeleteCachedData()
		{
			_playedIntros.Clear();
		}

		public static bool HasPlacedIntro(string introName)
		{
			return Instance._playedIntros.ContainsKey(introName) && Instance._playedIntros[introName];
		}
	}
}