using Source.Scripts.SaveSystem;
using Zenject;

namespace Source.Scripts.DI.Services.Boot
{
    public class SaveDataProvider
    {
        private readonly DataSerializer<SaveData> _dataSerializer;
        private readonly SaveData _data;

        [Inject]
        public SaveDataProvider(DataSerializer<SaveData> dataSerializer)
        {
            _dataSerializer = dataSerializer;
            _data = _dataSerializer.TryLoadData(out var loadedData) ? loadedData : new SaveData();
        }

        public SaveData PlayerSavedData => _data;

        public void Save()
        {
            _dataSerializer.SaveData(_data);
        }
    }
}