using MassTransit;
using SharedModels;
using System.Threading.Tasks;

namespace OrderMicroService.Consumers
{
    public class TicketConsumer : IConsumer<Ticket>
    {
        public async Task Consume(ConsumeContext<Ticket> context)
        {
            var data = context.Message;
        }
    }
}
