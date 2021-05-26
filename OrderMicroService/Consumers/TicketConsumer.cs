using MassTransit;
using SharedModels;
using System.Threading.Tasks;

namespace OrderMicroService.Consumers
{
    public class TicketConsumer : IConsumer<TicketShared>
    {
        public async Task Consume(ConsumeContext<TicketShared> context)
        {
            var data = context.Message;
        }
    }
}
