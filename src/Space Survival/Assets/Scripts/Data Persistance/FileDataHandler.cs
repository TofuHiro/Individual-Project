using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    string dataDirPath;
    string dataFileName;
    readonly string encryptionKey = "TofuIsCute";

    public FileDataHandler(string _dataDirPath, string _dataFileName)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
    }

    /// <summary>
    /// Deserializes game save file from json format to gamedata class format
    /// </summary>
    /// <returns></returns>
    public GameData Load()
    {
        //Save file directory
        string _fullPath = dataDirPath + "/" + dataFileName;
        GameData _loadedData = null;

        if (File.Exists(_fullPath)) {
            try {
                string _dataToLoad;
                //'Using' blocks closes connections to file after use
                using FileStream _stream = new FileStream(_fullPath, FileMode.Open);
                using StreamReader _reader = new StreamReader(_stream);
                //Get file data
                _dataToLoad = _reader.ReadToEnd();
                //And deserialize from json format
                _loadedData = JsonUtility.FromJson<GameData>(_dataToLoad);
            }
            catch (Exception e) {
                Debug.LogError("Error occured when trying to load game data from file: " + _fullPath + "\n" + e);
            }
        }

        return _loadedData;
    }

    /// <summary>
    /// Serializes gamedata to json format
    /// </summary>
    /// <param name="_gameData"></param>
    public void Save(GameData _gameData)
    {
        string _fullPath = dataDirPath + "/" + dataFileName;

        try {
            //Create initial directory to file
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));

            //Serialized game data in json format
            string _dataToStore = JsonUtility.ToJson(_gameData, true);

            //'Using' blocks closes connections to file after use
            using FileStream _stream = new FileStream(_fullPath, FileMode.Create);
            using StreamWriter _writer = new StreamWriter(_stream);
            _writer.Write(_dataToStore);
        }
        catch (Exception e) {
            Debug.LogError("Error occured when trying to save game data to file: " + _fullPath + "\n" + e);
        }
    }
}
