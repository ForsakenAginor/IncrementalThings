namespace Source.Scripts.SaveSystem
{
    public interface IDataSaveLoadService
    {
        public bool IsLoaded { get; }

        public bool IsInited { get; }

        public void Init(SaveData saveData, IDataSaveLoadService[] dependentSystems = null);

        public void Save();

        public void Load();
    }
}