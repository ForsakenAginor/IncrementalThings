namespace Source.Scripts.SaveSystem
{
    public interface ISaveLoadService
    {
        public string GetSavedInfo();

        public void SaveInfo(string value);
    }
}