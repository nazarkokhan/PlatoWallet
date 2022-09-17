namespace PlatipusWallet.Api.Application.Requests.Admin;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Results.External;

public record CreateUserRequest() : IRequest<IResult<BaseResponse>>
{
    public class Handler : IRequestHandler<CreateUserRequest, IResult<BaseResponse>>
    {
        public async Task<IResult<BaseResponse>> Handle(
            CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            var result = (BaseResponse) default;

            return ResultFactory.Success<BaseResponse>(result);
        }
    }
}
