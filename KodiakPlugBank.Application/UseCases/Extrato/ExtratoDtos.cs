namespace KodiakPlugBank.Application.UseCases.Extrato;

public record CriarExtratoRequest(
    int IdPagador,
    string? AccountHash,
    bool? Today,
    string? DateStart,
    string? DateEnd,
    Core.PlugBank.StatementType? StatementType,
    string? CardNumber);

public record CriarExtratoResponse(string? UniqueId, string? Type);
