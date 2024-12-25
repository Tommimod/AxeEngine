namespace AxeEngine
{
    public interface IChunk
    {
        void Add(int actorId, object property);
        object Get(int actorId);
    }
}