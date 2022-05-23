
using Grpc.Core;
using MeterReader.gRPC;
using static MeterReader.gRPC.MeterReaderService;

namespace MeterReader.Services
{
    public class MeterReadingService : MeterReaderServiceBase
    {
        private readonly IReadingRepository _repository;

        public MeterReadingService(IReadingRepository repository, ILogger<MeterReadingService> logger)
        {
            _repository = repository;
        }

        public override async Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
        {
            if (request.Successful == ReadingStatus.Success)
            {
                foreach (var reading in request.Readings)
                {
                    var readingValue = new MeterReading()
                    {
                        CustomerId = reading.CustomerId,
                        Value = reading.ReadingValue,
                        ReadingDate = reading.ReadingTime.ToDateTime(),

                    };

                    _repository.AddEntity(readingValue);
                }
            }

            if (await _repository.SaveAllAsync())
            {
                return new StatusMessage()
                {
                    Notes = "Successfully added to the database.",
                    Status = ReadingStatus.Success
                };
            }

            return new StatusMessage()
            {
                Notes = "Failed to store readings in Database",
                Status = ReadingStatus.Success
            };            
        }
    }
}
