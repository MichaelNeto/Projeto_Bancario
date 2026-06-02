namespace BancoApi.Domain;

public enum TipoPessoa
{
    PF,
    PJ
}

public enum TipoConta
{
    CORRENTE,
    POUPANCA
}

public enum StatusCliente
{
    ATIVO,
    BLOQUEADO,
    INATIVO
}

public enum TipoTransacao
{
    PIX_ENVIADO,
    PIX_RECEBIDO,
    DEPOSITO_RECEBIDO
}

public enum StatusTransacao
{
    Processando,
    Concluido,
    Falhou
}

public enum StatusCredito
{
    APROVADO,
    NEGADO
}

public enum TipoChavePix
{
    CPF,
    EMAIL,
    TELEFONE,
    ALEATORIA
}
