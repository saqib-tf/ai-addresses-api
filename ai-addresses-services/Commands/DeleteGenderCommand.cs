using ai_addresses_data.Entity;
using ai_addresses_data.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ai_addresses_services.Commands
{
    public record DeleteGenderCommand(int Id) : IRequest<bool>;

    public class DeleteGenderHandler : IRequestHandler<DeleteGenderCommand, bool>
    {
        private readonly IRepository<Gender> _genderRepository;

        public DeleteGenderHandler(IRepository<Gender> genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public async Task<bool> Handle(DeleteGenderCommand request, CancellationToken cancellationToken)
        {
            var gender = await _genderRepository.GetByIdAsync(request.Id);
            if (gender == null)
                return false;

            _genderRepository.Remove(gender);
            await _genderRepository.SaveChangesAsync();
            return true;
        }
    }
}