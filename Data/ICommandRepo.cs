using SixMinApi.models;

namespace SixMinApi.Data
{
    public interface ICommandRepo
    {
        Task SaveChangesAsync();
        Task<IEnumerable<Command>> GetAllCommandsAsync();
        Task<Command?> GetCommandByIdAsync(int id);
        Task CreateCommandAsync(Command command);
        //void UpdateCommand(Command command);
        void DeleteCommand(Command command);
    }
}