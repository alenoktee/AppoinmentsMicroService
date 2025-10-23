using MediatR;

namespace Appointments.Application.Results.Queries.GetResultXml;

public record GetResultXmlQuery(Guid Id) : IRequest<byte[]>;
