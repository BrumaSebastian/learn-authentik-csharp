
namespace src.Utils.AuthModels
{
    public struct Param <T> (string name, T value)
    {
        public string Name { get; init; } = nameof(T);
        public T Value { get; set; }
    }
}