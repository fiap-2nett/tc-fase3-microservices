namespace HelpDesk.ProducerService.Api.Contracts
{
    /// <summary>
    /// Represents the cancel ticket request.
    /// </summary>
    public sealed class CancelTicketRequest
    {
        /// <summary>
        /// Gets or sets the cancellation reason.
        /// </summary>
        public string CancellationReason { get; set; }
    }
}
