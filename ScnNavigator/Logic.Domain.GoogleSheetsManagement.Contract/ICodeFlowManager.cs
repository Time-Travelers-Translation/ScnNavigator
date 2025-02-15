namespace Logic.Domain.GoogleSheetsManagement.Contract
{
    public interface ICodeFlowManager
    {
        Task<HttpRequestMessage> CreateGetRequestAsync(string relativeUri);
        Task<HttpRequestMessage> CreatePostRequestAsync(string relativeUri);
    }
}
