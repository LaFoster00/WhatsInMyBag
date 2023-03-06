using System.IO;
using UnityEngine;

namespace Content.Checkpoints
{
	public class CheckpointSaveFileWiper : MonoBehaviour
	{
		
		public string savePath = "save.data";
		public string SaveFilePath => $"{Application.persistentDataPath}/{savePath}";
		
		public void WipeSave()
		{
			if (File.Exists(SaveFilePath))
			{
				File.Delete(SaveFilePath);
			}
		}
	}
}