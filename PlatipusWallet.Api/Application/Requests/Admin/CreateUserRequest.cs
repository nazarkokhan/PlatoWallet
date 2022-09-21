namespace PlatipusWallet.Api.Application.Requests.Admin;

using System.Threading;
using System.Threading.Tasks;
using Base;
using MediatR;
using Results.Common.Result;
using Results.Common.Result.WithData;

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
