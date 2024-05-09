using Coordinator.Enums;

namespace Coordinator.Models
{
    public record NodeState(Guid TransactionId)
    {

        public Guid Id { get; set; }
        /// <summary>
        /// 1. aşamanın durumu
        /// </summary>
        public ReadyType IsReady { get; set; }

        /// <summary>
        /// ikinci aşamanın neticesinde işlemin başarıyla tamamlanıp tamamlanmadığı
        /// </summary>

        public TransactionState TransactionState { get; set; }
        public Node Node { get; set; }

    }
}
