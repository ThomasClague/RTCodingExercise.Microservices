
namespace Contracts.Plates
{
    public record CreatePlateRequest
    {
        public CreatePlateCommand Command { get; init; }

        public CreatePlateRequest(CreatePlateCommand command)
        {
            Command = command;
        }
    }
} 