using System;
using UnityEngine.Serialization;

namespace Files
{
    [Serializable]
    public record SerializedPoint
    {
        public string name;
        public float time;
        public int generatedId;
    }
}