using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SI.Tests.Fixtures;

/// <summary>
/// Interceptor de EF Core que simula un fallo de infraestructura (caída de red/BD)
/// en el momento exacto en que se va a confirmar la transacción (TransactionCommittingAsync),
/// después de que las entidades ya fueron modificadas y enviadas en el buffer de la transacción.
/// </summary>
public sealed class SimuladorFalloTransaccionInterceptor : DbTransactionInterceptor
{
    public const string MensajeFalloSimulado = "Fallo simulado de infraestructura antes de confirmar la transacción (CP-13 / RNF-04).";

    public bool FalloActivado { get; set; } = true;

    public override ValueTask<InterceptionResult> TransactionCommittingAsync(
        DbTransaction transaction,
        TransactionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        if (FalloActivado)
        {
            throw new InvalidOperationException(MensajeFalloSimulado);
        }

        return base.TransactionCommittingAsync(transaction, eventData, result, cancellationToken);
    }
}
