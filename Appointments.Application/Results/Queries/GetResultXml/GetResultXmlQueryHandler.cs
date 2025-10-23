using Appointments.Application.Exceptions;
using Appointments.Domain.Dtos;
using Appointments.Domain.Entities;
using Appointments.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Text;
using System.Xml.Serialization;

namespace Appointments.Application.Results.Queries.GetResultXml;

public class GetResultXmlQueryHandler : IRequestHandler<GetResultXmlQuery, byte[]>
{
    private readonly IResultsRepository _resultsRepository;
    private readonly IMapper _mapper;

    public GetResultXmlQueryHandler(IResultsRepository resultsRepository, IMapper mapper)
    {
        _resultsRepository = resultsRepository;
        _mapper = mapper;
    }

    public async Task<byte[]> Handle(GetResultXmlQuery request, CancellationToken cancellationToken)
    {
        var result = await _resultsRepository.GetByIdAsync(request.Id);

        if (result is null)
        {
            throw new NotFoundException(nameof(Result), request.Id);
        }

        var resultDto = _mapper.Map<ResultXmlDto>(result);

        var xmlSerializer = new XmlSerializer(typeof(ResultXmlDto));

        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, resultDto);
            var xmlString = stringWriter.ToString();
            return Encoding.UTF8.GetBytes(xmlString);
        }
    }
}
