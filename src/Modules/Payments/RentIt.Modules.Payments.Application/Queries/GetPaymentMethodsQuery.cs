using MediatR;
using RentIt.Modules.Payments.Domain.Enums;

namespace RentIt.Modules.Payments.Application.Queries;

public record GetPaymentMethodsQuery(Guid UserId) : IRequest<List<PaymentMethodDto>>;

public record PaymentMethodDto(
    string Provider, 
    string MethodType, 
    string Last4, 
    int? ExpiryMonth, 
    int? ExpiryYear);
