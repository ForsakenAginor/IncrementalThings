using R3;

namespace Source.Scripts.Utility
{
    public interface IResource
    {
        public Observable<int> ResourceAmount { get; }

        public int Maximum { get; }
    }
}