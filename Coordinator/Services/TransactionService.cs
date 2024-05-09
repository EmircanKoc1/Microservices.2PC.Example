using Coordinator.Enums;
using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services
{
    public class TransactionService : ITransactionService
    {
        readonly TwoPhaseCommitContext _context;
        readonly IHttpClientFactory _httpClientFactory;
        readonly HttpClient _orderHttpClient;
        readonly HttpClient _stockHttpClient;
        readonly HttpClient _paymentHttpClient;


        public TransactionService(TwoPhaseCommitContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;

            _orderHttpClient = _httpClientFactory.CreateClient("OrderAPI");
            _stockHttpClient = _httpClientFactory.CreateClient("StockAPI");
            _paymentHttpClient = _httpClientFactory.CreateClient("PaymentAPI");
        }

        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new NodeState(transactionId)
                {
                    IsReady = ReadyType.Pending,
                    TransactionState = TransactionState.Pending,

                },

            });

            await _context.SaveChangesAsync();

            return transactionId;
        }
        public async Task PrepareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionId.Equals(transactionId))
                .ToListAsync();

            transactionNodes.ForEach(async transactionNode =>
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "OrderAPI" => _orderHttpClient.GetAsync("ready"),
                        "StockAPI" => _stockHttpClient.GetAsync("ready"),
                        "PaymentAPI" => _paymentHttpClient.GetAsync("ready"),

                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? ReadyType.Ready : ReadyType.UnReady;


                }
                catch (Exception)
                {

                    transactionNode.IsReady = ReadyType.UnReady;
                }

                await _context.SaveChangesAsync();


            });

        }

        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
            return (await _context.NodeStates
                .Where(ns => ns.TransactionId.Equals(transactionId))
                .ToListAsync()).TrueForAll(ns => ns.IsReady.Equals(ReadyType.Ready));

        }

        public Task<bool> CheckTransactionStateServiceAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                                   .Where(ns => ns.TransactionId.Equals(transactionId))
                                   .Include(ns => ns.Node)
                                   .ToListAsync();


            foreach (var transactionNode in transactionNodes)
            {

                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "OrderAPI" => _orderHttpClient.GetAsync("commit"),
                        "PaymentAPI" => _paymentHttpClient.GetAsync("commit"),
                        "StockAPI" => _stockHttpClient.GetAsync("commit"),
                    });


                    var result = bool.Parse(await response.Content.ReadAsStringAsync());

                    transactionNode.TransactionState = result ? TransactionState.Done : TransactionState.Abort;

                }
                catch (Exception)
                {

                    transactionNode.TransactionState = TransactionState.Abort;
                }


            }

        }



        public Task RollbackAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
