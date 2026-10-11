using System;
using Newtonsoft.Json;

namespace Source.Scripts.SaveSystem
{
    public class DataSerializer<T>
    {
        private readonly ISaveLoadService _saveLoadService;

        public DataSerializer(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService != null ? saveLoadService : throw new ArgumentNullException(nameof(saveLoadService));
        }

        public bool TryLoadData(out T data)
        {
            data = default;
            string jsonData = _saveLoadService.GetSavedInfo();

            if (jsonData == null)
                return false;

            data = JsonConvert.DeserializeObject<SerializableT>(jsonData).Content;
            return true;
        }

        public void SaveData(T dataThatWillBeSaved)
        {
            SerializableT serializableT = new SerializableT()
            {
                Content = dataThatWillBeSaved,
            };
            string data = JsonConvert.SerializeObject(
                serializableT,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
                );

            _saveLoadService.SaveInfo(data);
        }

        [Serializable]
        private class SerializableT
        {
            public T Content;
        }
    }
}