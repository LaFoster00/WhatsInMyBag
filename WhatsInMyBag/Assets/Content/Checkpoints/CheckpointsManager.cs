using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DeadBody;
using GameEvents;
using UnityEngine;

namespace Content.Checkpoints
{
	public class CheckpointSaveData
	{
		public int CheckpointID;
		public Vector3 DeadBodyPosition;
		
		public List<GameobjectSaveData> PositionSaveDatas;
	}

	[Serializable]
	public class GameobjectSaveData
	{
		public string name;
		public Vector3 position;
		public Quaternion rotation;
	}

	public class CheckpointsManager : MonoBehaviour
	{
		public bool doLoad = true;
		public bool doSave = true;

		public bool onlySaveHigherIdCheckpoints = false;

		public Transform player;
		public Transform deadbody;

		public string savePath = "save.data";

		private Checkpoint[] _checkpoints;
		private Checkpoint _activeCheckpoint;
		private Vector3 _lastSavedDeadBodyPosition;

		private List<GameobjectSaveData> _positionSaveDatas = new List<GameobjectSaveData>();

		public static CheckpointsManager Instance { get; private set; }

		public string SaveFilePath => $"{Application.persistentDataPath}/{savePath}";

		private void Start()
		{
			Debug.Assert(player, "Must assign a player");
			Instance = this;
			_checkpoints = FindObjectsOfType<Checkpoint>();


			_checkpoints = _checkpoints.OrderBy(checkpoint => checkpoint.CheckPointID).ToArray();
			for (int i = 1; i < _checkpoints.Length; i++)
			{
				Debug.Assert(_checkpoints[i - 1].CheckPointID != _checkpoints[i].CheckPointID,
					$"Checkpoint IDs of {_checkpoints[i - 1].name} and {_checkpoints[i].name} are not unique (ID is {_checkpoints[i].CheckPointID})");
			}


			if (doLoad)
				DoLoad();
		}

		private void OnDisable()
		{
			if (doSave)
			{
				if (_activeCheckpoint)
				{
					Debug.Log($"Saving Checkpoint {_activeCheckpoint.checkpointID}...");
				}
				else
				{
					Debug.Log("No checkpoint to save");
				}

				DoSave();
			}
		}

		public void SetActiveCheckpoint(Checkpoint point)
		{
			if (point && 
			    (!_activeCheckpoint || point.CheckPointID > _activeCheckpoint.CheckPointID || (!onlySaveHigherIdCheckpoints)))
			{
				_activeCheckpoint = point;
				_lastSavedDeadBodyPosition = DeadBodyInteractable.Instance.transform.position;
				Debug.Log($"Reached Checkpoint {point.CheckPointID}");

				// save enemy positions
				int currIndex = 0;
				_positionSaveDatas = new List<GameobjectSaveData>();
				foreach (KeyValuePair<string, GameObject> pair in PositionSaveBehaviour.saveables)
				{
					var trans = pair.Value.transform;
					_positionSaveDatas.Add(new GameobjectSaveData()
						{name = pair.Key, position = trans.position, rotation = trans.rotation});
				}
			}
		}

		public void DoLoad()
		{
			Debug.Log("Loading last Checkpoint...");
			if (File.Exists(SaveFilePath))
			{
				CheckpointSaveData data = new CheckpointSaveData();

				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(SaveFilePath, FileMode.Open);
				string json = (string) bf.Deserialize(file);
				JsonUtility.FromJsonOverwrite(json, data);
				int currCheckpointId = data.CheckpointID;
				file.Close();


				// load positions
				_positionSaveDatas = data.PositionSaveDatas;
				if (_positionSaveDatas != null)
				{
					foreach (GameobjectSaveData saveData in _positionSaveDatas)
					{
						Transform trans = PositionSaveBehaviour.saveables[saveData.name].transform;

						trans.position = saveData.position;
						trans.rotation = saveData.rotation;
					}
				}


				// load checkpoint
				if (currCheckpointId != Int32.MaxValue)
				{
					if (_checkpoints.All(checkpoint => checkpoint.CheckPointID != currCheckpointId))
					{
						Debug.LogError($"Saved checkpoint id {currCheckpointId} is not in scene!");
						return;
					}

					var loadedCheckpoint =
						_checkpoints.Single(checkpoint => checkpoint.CheckPointID == currCheckpointId);
					_lastSavedDeadBodyPosition = data.DeadBodyPosition;

					// set player and dead body position
					player.position = loadedCheckpoint.transform.position;
					// get dead body root gameobject
					deadbody.position = data.DeadBodyPosition;
					
					
					
					
					foreach (var checkpoint in _checkpoints)
					{
						if (checkpoint.checkpointID <= currCheckpointId)
						{
							checkpoint.ActivateCheckpoint();
						}
					}

					Debug.Log($"Loading Checkpoint {currCheckpointId}");
				}
				else
				{
					Debug.Log("No Checkpoint was saved");
				}
			}
			else
			{
				Debug.Log("No Checkpoint was saved");
			}
		}

		public void DoSave()
		{
			CheckpointSaveData data = new CheckpointSaveData
			{
				CheckpointID = Int32.MaxValue, DeadBodyPosition = _lastSavedDeadBodyPosition + new Vector3(0,0.02f,0),
				PositionSaveDatas = _positionSaveDatas
			};

			if (_activeCheckpoint != null)
			{
				data.CheckpointID = _activeCheckpoint.CheckPointID;
				Debug.Log($"Saving Checkpoint {data.CheckpointID}");
			}

			WipeSave();
			string saveData = JsonUtility.ToJson(data, true);
			FileStream file = File.Create(SaveFilePath);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(file, saveData);
			file.Close();
		}

		public void WipeSave()
		{
			if (File.Exists(SaveFilePath))
			{
				File.Delete(SaveFilePath);
			}
		}
	}
}