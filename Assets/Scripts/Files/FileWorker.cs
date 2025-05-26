using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DTOs;
using Newtonsoft.Json;
using SFB;
using UnityEngine;

namespace Files
{
    public class FileWorker : ScriptableObject
    {
        private string _currentPath;
        private bool _isSaved;

        public PointsModel DataModel { get; set; }

        public void LoadData(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File not found");
            }

            _currentPath = path;
            var file = File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<List<SerializedPoint>>(file);

            if (!_isSaved)
            {
                // TODO: raise exception/show popup
            }

            DataModel.Points.Clear();
            var pointList = list.Select(p => new PointDTO
            {
                Id = p.generatedId,
                Name = p.name,
                Time = p.time,
            });
            DataModel.Points.AddRange(pointList);
            DataModel.NotifyAboutChanges();
        }

        public void SaveData()
        {
            if (string.IsNullOrEmpty(_currentPath))
            {
                _currentPath = StandaloneFileBrowser.SaveFilePanel("Save scene", "", "", "json");
                if (string.IsNullOrEmpty(_currentPath))
                    return;
            }

            var data = DataModel.Points
                .Select(p => new SerializedPoint
                {
                    name = p.Name,
                    time = p.Time,
                    generatedId = p.Id
                })
                .ToList();
            var str = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_currentPath, str);
            _isSaved = true;
        }

        public void CreateNew()
        {
            if (!_isSaved)
            {
                // TODO: raise exception/show popup
            }

            _currentPath = null;
            DataModel.Points.Clear();
            DataModel.NotifyAboutChanges();
            _isSaved = true;
        }
    }
}