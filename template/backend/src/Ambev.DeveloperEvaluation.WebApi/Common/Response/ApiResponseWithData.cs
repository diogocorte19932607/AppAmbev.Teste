namespace Ambev.DeveloperEvaluation.WebApi.Common.Responses;

/// <summary>
/// Representa uma resposta de API com sucesso, mensagem e dados tipados.
/// </summary>
/// <typeparam name="T">Tipo dos dados retornados</typeparam>
public class ApiResponseWithData<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Succes { get; set; }

    /// <summary>
    /// Mensagem associada ao resultado da operação.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Dados retornados pela API.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Cria uma resposta de sucesso com dados e mensagem.
    /// </summary>
    /// <param name="data">Dados da resposta</param>
    /// <param name="message">Mensagem de sucesso</param>
    /// <returns>Instância de ApiResponseWithData com sucesso</returns>
    public static ApiResponseWithData<T> Success(T data, string message = "Operação realizada com sucesso.")
    {
        return new ApiResponseWithData<T>
        {
            Succes = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Cria uma resposta de erro (sem dados).
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <returns>Instância de ApiResponseWithData com erro</returns>
    public static ApiResponseWithData<T> Failure(string message)
    {
        return new ApiResponseWithData<T>
        {
            Succes = false,
            Message = message,
            Data = default!
        };
    }
}
